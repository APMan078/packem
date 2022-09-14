using System.Collections.Generic;

namespace Packem.Domain.Models
{
    public class PutAwayPalletDeviceCreateModel
    {
        public class PutAway
        {
            public int? PutAwayId { get; set; }
        }

        public PutAwayPalletDeviceCreateModel()
        {
            PutAways = new List<PutAway>();
        }

        public int? UserId { get; set; }
        public int? CustomerFacilityId { get; set; }
        public IEnumerable<PutAway> PutAways { get; set; }
        public BinGetCreateModel BinGetCreate { get; set; }
    }
}