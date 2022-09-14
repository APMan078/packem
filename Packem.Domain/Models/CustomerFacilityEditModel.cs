namespace Packem.Domain.Models
{
    public class CustomerFacilityEditModel
    {
        public int? CustomerFacilityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}