using CustomerServiceCampaignAPI.Data;
using CustomerServiceCampaignAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Formats.Asn1;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;

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

        /*[HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<PurchaseDTO>> GetPurchases()
        {
            return Ok(_db.Purchases.ToList());
        }*/

        /*[HttpGet("{id:int}", Name = "GetPurchase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PurchaseDTO> GetPurchase(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var purchase = _db.Purchases.FirstOrDefault(u => u.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }*/

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/savePurchase")]
        public async Task<ActionResult<Purchase>> CreatePurchase([FromBody] Purchase purchase)
        {
            if (purchase == null)
            {
                return BadRequest(purchase);
            }
            if (purchase.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            if (_db.Agents.FirstOrDefault(u => u.Id == purchase.AgentId) == null)
            {
                ModelState.AddModelError("CustomError", "Agent does not exist!");
                return BadRequest(ModelState);
            }
            Campaign campaign = _db.Campaigns.FirstOrDefault(u => u.Id == purchase.CampaignId);
            if (campaign == null)
            {
                ModelState.AddModelError("CustomError", "Campaign does not exist!");
                return BadRequest(ModelState);
            }
            if (purchase.PurchaseDate.Date < campaign.StartDate.Date || purchase.PurchaseDate.Date > campaign.EndDate.Date)
            {
                ModelState.AddModelError("CustomError", "Campaing is not active!");
                return BadRequest(ModelState);
            }

            XmlNode customerXmlData = await GetCustomerData(purchase.CustomerId);

            if (customerXmlData == null)
            {
                ModelState.AddModelError("CustomError", "Customer does not exist!");
                return BadRequest(ModelState);
            }

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
                Id = purchase.Id,
                AgentId = purchase.AgentId,
                CustomerId = purchase.CustomerId,
                CampaignId = purchase.CampaignId,
                Price = purchase.Price,
                Discount = purchase.Discount,
                PriceWithDiscount = purchase.Price * (100 - purchase.Discount) / 100,
                PurchaseDate = purchase.PurchaseDate
            };

            _db.Purchases.Add(model);
            _db.SaveChanges();

            /*Models.Dto.Purchase response = new()
            {
                Id = model.Id,
                AgentId = model.AgentId,
                CustomerId = model.CustomerId,
                CampaignId = model.CampaignId,
                Price = model.Price,
                Discount = model.Discount,
                PriceWithDiscount = model.PriceWithDiscount,
                PurchaseDate = model.PurchaseDate
            };*/

            return CreatedAtAction(nameof(CreatePurchase), model);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<XmlNode> GetCustomerData(int customerId)
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


                        return findPersonResult;
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
            var campaign = _db.Campaigns.FirstOrDefault(u => u.Id == request.campaignId);
            if (campaign == null)
            {
                ModelState.AddModelError("CustomError", "Campaign does not exist!");
                return BadRequest(ModelState);
            }

            TimeSpan timeDifference = request.CurrentDate.Date - campaign.EndDate.Date;
            int daysDifference = (int)timeDifference.TotalDays;
            if (daysDifference <= 30)
            {
                ModelState.AddModelError("CustomError", "You can get report one mont after campaign end!");
                return BadRequest(ModelState);
            }
                        
            var purchases = _db.Purchases.Where(p => p.CampaignId == request.campaignId).ToList();

            if (purchases.Count == 0)
            {
                return NotFound("No purchase data found.");
            }

            var csvData = ToCsv(purchases);

            Response.Headers.Add("Content-Disposition", "attachment; filename=successfulPurchases.csv");
            return File(new System.Text.UTF8Encoding().GetBytes(csvData), "text/csv");
        }

        private string ToCsv(List<Purchase> data)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,AgentId,CustomerId,CampaignId,Price,Discount,PriceWithDiscount,PurchaseDate");

            foreach (var purchase in data)
            {
                csv.AppendLine($"{purchase.Id},{purchase.AgentId},{purchase.CustomerId},{purchase.CampaignId},{purchase.Price},{purchase.Discount},{purchase.PriceWithDiscount},{purchase.PurchaseDate}");
            }

            return csv.ToString();
        }


    /*[HttpDelete("{id:int}", Name = "DeletePurchase")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeletePurchase(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var purchase = _db.Purchases.FirstOrDefault(a => a.Id == id);
        if (purchase == null)
        {
            return NotFound();
        }

        _db.Purchases.Remove(purchase);
        _db.SaveChanges();
        return NoContent();
    }*/
}
}
