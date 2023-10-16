using System.Net;
using System.Xml.Serialization;

namespace CustomerServiceCampaignAPI.Models.CustomerAPIData
{
    public class Person
    {
        public string Name { get; set; }
        public string SSN { get; set; }
        public DateTime? DOB { get; set; }
        public Address Home { get; set; }
        public Address Office { get; set; }
        public Person Spouse { get; set; }
        public string[] FavoriteColors { get; set; }
        public long? Age { get; set; }
    }
}
