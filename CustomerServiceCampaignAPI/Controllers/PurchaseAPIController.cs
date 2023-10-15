using CustomerServiceCampaignAPI.Data;
using CustomerServiceCampaignAPI.Models.dto;
using CustomerServiceCampaignAPI.Models;
using Microsoft.AspNetCore.Mvc;
using CustomerServiceCampaignAPI.Models.Dto;
using Microsoft.Extensions.Logging;
using System.Xml;
using System.Xml.Linq;

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
        public ActionResult<IEnumerable<PurchaseDTO>> GetPurchases()
        {
            return Ok(_db.Purchases.ToList());
        }

        [HttpGet("{id:int}", Name = "GetPurchase")]
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
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("/savePurchase")]
        public async Task<ActionResult<PurchaseDTO>> CreateAgent([FromBody] PurchaseDTO purchaseDTO)
        {
            if (purchaseDTO == null)
            {
                return BadRequest(purchaseDTO);
            }
            if (purchaseDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            if (_db.Agents.FirstOrDefault(u => u.Id == purchaseDTO.AgentId) == null)
            {
                ModelState.AddModelError("CustomError", "Agent does not exist!");
                return BadRequest(ModelState);
            }
            Campaign campaign = _db.Campaigns.FirstOrDefault(u => u.Id == purchaseDTO.CampaignId);
            if (campaign == null)
            {
                ModelState.AddModelError("CustomError", "Campaign does not exist!");
                return BadRequest(ModelState);
            }
            if (purchaseDTO.PurchaseDate.Date < campaign.StartDate.Date || purchaseDTO.PurchaseDate.Date > campaign.EndDate.Date)
            {
                ModelState.AddModelError("CustomError", "Campaing is not active!");
                return BadRequest(ModelState);
            }

            XmlNode customerXmlData = await GetCustomerData(purchaseDTO.CustomerId);

            if (customerXmlData == null)
            {
                ModelState.AddModelError("CustomError", "Customer does not exist!");
                return BadRequest(ModelState);
            }

            int purchasesCount = _db.Purchases
                        .Where(p => p.AgentId == purchaseDTO.AgentId && p.CampaignId == purchaseDTO.CampaignId && p.PurchaseDate.Date == purchaseDTO.PurchaseDate.Date)
                        .Count();
            if (purchasesCount >= 5)
            {
                ModelState.AddModelError("CustomError", "You passed your daily limit!");
                return BadRequest(ModelState);
            }

            Purchase model = new()
            {
                Id = purchaseDTO.Id,
                AgentId = purchaseDTO.AgentId,
                CustomerId = purchaseDTO.CustomerId,
                CampaignId = purchaseDTO.CampaignId,
                Price = purchaseDTO.Price,
                Discount = purchaseDTO.Discount,
                PriceWithDiscount = purchaseDTO.Price * (100 - purchaseDTO.Discount) / 100,
                PurchaseDate = purchaseDTO.PurchaseDate
            };

            _db.Purchases.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetCampaign", new { id = purchaseDTO.Id }, purchaseDTO);
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

        [HttpDelete("{id:int}", Name = "DeletePurchase")]
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
        }
    }
}
