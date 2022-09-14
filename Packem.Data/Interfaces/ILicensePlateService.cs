using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ILicensePlateService
    {
        Task<Result<string>> GetGenerateLicensePlateNoAsync(AppState state, int customerId);
        Task<Result<LicensePlateGetModel>> CreateLicensePlateUnknownAsync(AppState state, LicensePlateUnknownCreateModel model);
        Task<Result<LicensePlateGetModel>> CreateLicensePlateKnownAsync(AppState state, LicensePlateKnownCreateModel model);
        Task<Result<IEnumerable<LicensePlateLookupLPNoGetModel>>> GetLicensePlateLookupByLicensePlateNoAsync(AppState state, int customerId, string searchText);
        Task<Result<LicensePlateKnownAssignmentGetModel>> GetLicensePlateKnownAssignmentAsync(AppState state, int customerId, int licensePlateId);
        Task<Result<LicensePlateGetModel>> EditLicensePlateKnownAssignmentAsync(AppState state, LicensePlateKnownAssignmentEditModel model);
        Task<Result<IEnumerable<LicensePlateHistoryGetModel>>> GetLicensePlateHistoryAsync(AppState state, int customerId);
        Task<Result<IEnumerable<LicensePlateLookupDeviceGetModel>>> GetLicensePlateLookupDeviceAsync(CustomerDeviceTokenAuthModel state, string searchText, bool barcodeSearch = false);
        Task<Result<LicensePlateKnownAssignmentDeviceGetModel>> GetLicensePlateKnownAssignmentDeviceAsync(CustomerDeviceTokenAuthModel state, int licensePlateId);
        Task<Result<LicensePlateGetModel>> EditLicensePlateUnknownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateUnknownToPalletizedEditModel model);
        Task<Result<LicensePlateGetModel>> EditLicensePlateKnownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateKnownToPalletizedEditModel model);
    }
}