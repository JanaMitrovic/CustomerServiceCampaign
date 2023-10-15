using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int CustomerId { get; set; }
        public int Price { get; set; }
        public int PriceWithDiscount { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
