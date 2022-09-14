using Packem.Domain.Models;

namespace Packem.Mobile.Models
{
    public class MobileState
    {
        public CustomerDeviceTokenAuthModel DeviceState { get; set; }
        public CustomerFacility Facility { get; set; }
        public string UserToken { get; set; }
        public AppState AppState { get; set; }
    }
}