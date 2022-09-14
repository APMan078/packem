namespace Packem.Domain.Models
{
    public class VendorGetModel
    {
        public int VendorId { get; set; }
        public int CustomerId { get; set; }
        public string VendorNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string ZipPostalCode { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
    }
}