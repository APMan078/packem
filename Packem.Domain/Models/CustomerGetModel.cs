namespace Packem.Domain.Models
{
    public class CustomerGetModel
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PointOfContact { get; set; }
        public string ContactEmail { get; set; }
        public bool IsActive { get; set; }
        public int DefaultThreshold { get; set; }
    }
}