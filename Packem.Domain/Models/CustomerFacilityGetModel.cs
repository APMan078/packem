namespace Packem.Domain.Models
{
    public class CustomerFacilityGetModel
    {
        public int CustomerId { get; set; }
        public int CustomerFacilityId { get; set; }
        public int CustomerLocationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
