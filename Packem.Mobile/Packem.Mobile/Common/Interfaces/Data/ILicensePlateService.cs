using Packem.Domain.Models;
using Packem.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Mobile.Common.Interfaces.Data
{
    public interface ILicensePlateService
    {
        Task<HttpResponseWrapper<IEnumerable<LicensePlateLookupDeviceGetModel>>> GetLicensePlateLookupDeviceAsync(CustomerDeviceTokenAuthModel state, string searchText, bool barcodeSearch = false);
        Task<HttpResponseWrapper<LicensePlateKnownAssignmentDeviceGetModel>> GetLicensePlateKnownAssignmentDeviceAsync(CustomerDeviceTokenAuthModel state, int licensePlateId);
        Task<HttpResponseWrapper<LicensePlateGetModel>> EditLicensePlateUnknownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateUnknownToPalletizedEditModel model);
        Task<HttpResponseWrapper<LicensePlateGetModel>> EditLicensePlateKnownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateKnownToPalletizedEditModel model);
    }
}