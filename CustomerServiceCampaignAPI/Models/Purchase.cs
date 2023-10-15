using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int AgentId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int CampaignId { get; set; }
        [Required]
        public int Price { get; set; }
        public int Discount { get; set; }
        [Required]
        public int PriceWithDiscount { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
    }
}
