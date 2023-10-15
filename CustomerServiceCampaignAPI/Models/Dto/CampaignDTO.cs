using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models.Dto
{
    public class CampaignDTO
    {
        public int Id { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
