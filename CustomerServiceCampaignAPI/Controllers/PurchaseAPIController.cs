﻿using CustomerServiceCampaignAPI.Data;
using CustomerServiceCampaignAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using CustomerServiceCampaignAPI.Models.CustomerAPIData;

namespace CustomerServiceCampaignAPI.Controllers
{
    [Route("api/purchaseAPI")]
    [ApiController]
    public class PurchaseAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PurchaseAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Purchase>> GetPurchases()
        {
            return Ok(_db.Purchases.ToList());
        }

        [HttpGet("{id:int}", Name = "GetPurchase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Purchase> GetPurchase(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError("CustomError", "Id cannot be 0!");
                return BadRequest(ModelState);
            }

            var purchase = _db.Purchases.FirstOrDefault(u => u.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("/createPurchase")]
        public async Task<ActionResult<Purchase>> CreatePurchase([FromBody] Purchase purchase)
        {
            //Check if puchase object is not null
            if (purchase == null)
            {
                ModelState.AddModelError("CustomError", "Purchase is not passed!");
                return BadRequest(ModelState);
            }
            //Id is autogenerated
            if (purchase.Id > 0)
            {
                ModelState.AddModelError("CustomError", "Purchase Id is autogenerated!");
                return BadRequest(ModelState);
            }
            //Check if agent with passed Id exists
            if (_db.Agents.FirstOrDefault(u => u.Id == purchase.AgentId) == null)
            {
                ModelState.AddModelError("CustomError", "Agent does not exist!");
                return BadRequest(ModelState);
            }
            //Check if campaign with passed Id exists
            Campaign campaign = _db.Campaigns.FirstOrDefault(u => u.Id == purchase.CampaignId);
            if (campaign == null)
            {
                ModelState.AddModelError("CustomError", "Campaign does not exist!");
                return BadRequest(ModelState);
            }
            //If campaign exists - Check if Purchase Date is withing campaign week
            if (purchase.PurchaseDate.Date < campaign.StartDate.Date || purchase.PurchaseDate.Date > campaign.EndDate.Date)
            {
                ModelState.AddModelError("CustomError", "Campaing is not active!");
                return BadRequest(ModelState);
            }
            //Check if customer with passed Id exists - loyal customer are save in Customer service api
            XmlElement customerXmlData = await GetCustomerData(purchase.CustomerId);
            if (customerXmlData == null)
            {
                ModelState.AddModelError("CustomError", "Customer does not exist!");
                return BadRequest(ModelState);
            }
            //Check agent daily limit - 5 purchases for each day of a campaign
            int purchasesCount = _db.Purchases
                        .Where(p => p.AgentId == purchase.AgentId && p.CampaignId == purchase.CampaignId && p.PurchaseDate.Date == purchase.PurchaseDate.Date)
                        .Count();
            if (purchasesCount >= 5)
            {
                ModelState.AddModelError("CustomError", "You passed your daily limit!");
                return BadRequest(ModelState);
            }

            Purchase model = new()
            {
                AgentId = purchase.AgentId,
                CustomerId = purchase.CustomerId,
                CampaignId = purchase.CampaignId,
                Price = purchase.Price,
                Discount = purchase.Discount,
                //Calculate price with desired discount
                PriceWithDiscount = purchase.Price * (100 - purchase.Discount) / 100,
                PurchaseDate = purchase.PurchaseDate
            };

            _db.Purchases.Add(model);
            _db.SaveChanges();

            return CreatedAtAction(nameof(CreatePurchase), model);
        }

        //Function for getting customer data from Customer service api
        //Returns null if customer does not exist
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<XmlElement> GetCustomerData(int customerId)
        {
            string soapApiUrl = "https://www.crcind.com/csp/samples/SOAP.Demo.cls?soap_method=FindPerson&id=" + customerId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PostAsync(soapApiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        string xmlResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(xmlResponse);

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xmlResponse);

                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("ns", "http://tempuri.org");

                        XmlNode findPersonResult = xmlDoc.SelectSingleNode("//ns:FindPersonResult", nsmgr);

                        if (findPersonResult is XmlElement)
                        {
                            return (XmlElement)findPersonResult;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            
            return null;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/getCsvReport")]
        public IActionResult GetCsvReport([FromBody] GetCsvRequestData request)
        {
            //Check if campaign with passed Id exists
            var campaign = _db.Campaigns.FirstOrDefault(u => u.Id == request.campaignId);
            if (campaign == null)
            {
                ModelState.AddModelError("CustomError", "Campaign does not exist!");
                return BadRequest(ModelState);
            }
            //You can only get a report if current date is at least one month after campaign end
            TimeSpan timeDifference = request.CurrentDate.Date - campaign.EndDate.Date;
            int daysDifference = (int)timeDifference.TotalDays;
            if (daysDifference <= 30)
            {
                ModelState.AddModelError("CustomError", "You can get report one mont after campaign end!");
                return BadRequest(ModelState);
            }
            //Get all purchases for selected campaign  
            var purchases = _db.Purchases.Where(p => p.CampaignId == request.campaignId).ToList();

            if (purchases.Count == 0)
            {
                return NotFound("No purchase data found.");
            }

            //Put purchases data into .csv file
            var csvData = ToCsv(purchases);

            //Send .csv file as a response
            Response.Headers.Add("Content-Disposition", "attachment; filename=successfulPurchases.csv");
            return File(new System.Text.UTF8Encoding().GetBytes(csvData), "text/csv");
        }

        //Function that gets puchases list and created .csv file
        private string ToCsv(List<Purchase> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,AgentName,AgentSurname,AgentEmail,CampaignName,Price,Discount,PriceWithDiscount,PurchaseDate,CustomerId");

            foreach (var purchase in data)
            {
                Agent agent = _db.Agents.FirstOrDefault(a => a.Id ==  purchase.AgentId);
                Campaign campaign = _db.Campaigns.FirstOrDefault(c => c.Id == purchase.CampaignId);
                csv.AppendLine($"{purchase.Id},{agent.Name},{agent.Surname},{agent.Email},{campaign.CampaignName},{purchase.Price},{purchase.Discount},{purchase.PriceWithDiscount},{purchase.PurchaseDate},{purchase.CustomerId}");
            }

            return csv.ToString();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/showReportData")]
        public async Task<IActionResult> ShowReportData(IFormFile file)
        {
            //Check if file is uploaded
            if (file == null)
            {
                ModelState.AddModelError("CustomError", "File is not uploaded!");
                return BadRequest(ModelState);
            }
            //Check if file is empty
            if (file.Length == 0)
            {
                ModelState.AddModelError("CustomError", "File is empty!");
                return BadRequest(ModelState);
            }
            //Extract data from a file
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var response = new List<CsvResponseData>();

                //Define .csv parser
                var csvParser = new TextFieldParser(streamReader);
                csvParser.SetDelimiters(new string[] { "," });
                //Extract header line with atribute names
                string[] fields = csvParser.ReadFields();
                //Fill the response
                while (!csvParser.EndOfData)
                {
                    string[] data = csvParser.ReadFields();

                    CsvResponseData element = new CsvResponseData();
                    element.Id = int.Parse(data[0]);
                    element.AgentName = data[1];
                    element.AgentSurname = data[2];
                    element.AgentEmail = data[3];
                    element.CampaignName = data[4];
                    element.Price = int.Parse(data[5]);
                    element.Discount = int.Parse(data[6]);
                    element.PriceWithDiscount = int.Parse(data[7]);
                    element.PurchaseDate = DateTime.Parse(data[8]);

                    //Get customer data and extract it
                    XmlElement customerData = await GetCustomerData(int.Parse(data[9]));
                    if(customerData != null)
                    {
                        Person customer = ExtractCustomerData(customerData);
                        element.Customer = customer;
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Customer does not exist!");
                        return BadRequest(ModelState);
                    }

                    response.Add(element);
                }


                return Ok(response);
            }
        }

        //Function for extracting customer data from xml
        [ApiExplorerSettings(IgnoreApi = true)]
        private Person ExtractCustomerData(XmlElement findPersonResult)
        {
            Person person = new Person
            {
                Name = GetElementValue(findPersonResult, "Name"),
                SSN = GetElementValue(findPersonResult, "SSN"),
                DOB = DateTime.Parse(GetElementValue(findPersonResult, "DOB")),
                Age = long.Parse(GetElementValue(findPersonResult, "Age")),
                Home = new Address
                {
                    Street = GetElementValue(findPersonResult, "Home/Street"),
                    City = GetElementValue(findPersonResult, "Home/City"),
                    State = GetElementValue(findPersonResult, "Home/State"),
                    Zip = GetElementValue(findPersonResult, "Home/Zip")
                },
                Office = new Address
                {
                    Street = GetElementValue(findPersonResult, "Office/Street"),
                    City = GetElementValue(findPersonResult, "Office/City"),
                    State = GetElementValue(findPersonResult, "Office/State"),
                    Zip = GetElementValue(findPersonResult, "Office/Zip")
                },
            };

            return person;
        }

        //Function for extracting element from xml by its tag name
        private string GetElementValue(XmlElement parent, string elementName)
        {
            var element = parent.GetElementsByTagName(elementName);
            if (element.Count > 0)
            {
                return element[0].InnerText;
            }
            return null;
        }

        [HttpDelete("{id:int}", Name = "DeletePurchase")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeletePurchase(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError("CustomError", "Id cannot be 0!");
                return BadRequest(ModelState);
            }
            var purchase = _db.Purchases.FirstOrDefault(a => a.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            _db.Purchases.Remove(purchase);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
