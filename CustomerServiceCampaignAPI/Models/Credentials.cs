using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models
{
    public class Credentials
    {
        [Required]
        public string Email { get; set; }
    }
}
