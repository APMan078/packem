using Packem.Domain.Common.Enums;
using Packem.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Packem.Domain.Entities
{
    public partial class LicensePlate : ISoftDelete
    {
        public LicensePlate()
        {
            LicensePlateItems = new HashSet<LicensePlateItem>();
        }

        public int LicensePlateId { get; set; }
        public int? CustomerId { get; set; }
        public int? PalletId { get; set; }
        public string LicensePlateNo { get; set; } // uniqueness, customer level
        public LicensePlateTypeEnum LicensePlateType { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
        public bool Printed { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool Palletized { get; set; } // if true, LP will now show to putaway
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
        public virtual Pallet Pallet { get; set; }
        public virtual ICollection<LicensePlateItem> LicensePlateItems { get; set; }
        public virtual PutAway PutAway { get; set; }
    }
}