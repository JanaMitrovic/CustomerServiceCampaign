using CustomerServiceCampaignAPI.Models.CustomerAPIData;
using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models
{
    public class CsvResponseData
    {
        public int Id { get; set; }
        public string AgentName { get; set; }
        public string AgentSurname { get; set; }
        public string AgentEmail { get; set; }
        public string CampaignName { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int PriceWithDiscount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Person Customer { get; set; }
    }
}
