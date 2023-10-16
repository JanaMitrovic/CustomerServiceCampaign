using System.Xml.Serialization;

namespace CustomerServiceCampaignAPI.Models.CustomerAPIData
{
    //[XmlRoot("FindPersonResult", Namespace = "http://tempuri.org")]
    public class FindPersonResponse
    {
        [XmlElement(Namespace = "http://tempuri.org")]
        public Person FindPersonResult { get; set; }
    }
}
