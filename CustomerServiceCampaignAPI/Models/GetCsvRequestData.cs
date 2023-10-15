using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models
{
    public class GetCsvRequestData
    {
        [Required]
        public int campaignId { get; set; }
        [Required]
        public DateTime CurrentDate { get; set; }
    }
}
