using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models.dto
{
    public class AgentDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }



    }
}
