using Packem.Domain.Common.Interfaces;
using System;

namespace Packem.Domain.Entities
{
    public partial class ErrorLog : ISoftDelete
    {
        public int ErrorLogId { get; set; }
        public int? UserId { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Date { get; set; }
        public bool Deleted { get; set; }

        public virtual User User { get; set; }
    }
}
