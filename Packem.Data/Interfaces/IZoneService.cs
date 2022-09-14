using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IZoneService
    {
        Task<Result<ZoneGetModel>> CreateZoneAsync(AppState state, ZoneCreateModel model);
        Task<Result<ZoneGetModel>> EditZoneAsync(AppState state, ZoneEditModel model);
        Task<Result<ZoneGetModel>> DeleteZoneAsync(AppState state, ZoneDeleteModel model);
        Task<Result<IEnumerable<ZoneGetModel>>> GetZoneByCustomerLocationIdAsync(AppState state, int customerLocationId);
        Task<Result<ZoneGetModel>> GetZoneAsync(AppState state, int zoneId);
        Task<Result<IEnumerable<ZoneLookupGetModel>>> GetZoneLookupAsync(AppState state, int customerFacilityId, string searchText);
        Task<Result<IEnumerable<ZoneInventoryGetModel>>> GetZoneInventoryDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int inventoryId);
        Task<Result<IEnumerable<ZoneLookupDeviceGetModel>>> GetZoneLookupDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, string searchText);
        Task<Result<IEnumerable<ZoneLookupItemQuantityGetModel>>> GetZoneLookupItemQuantityDeviceAsync(CustomerDeviceTokenAuthModel state, int customerFacilityId, int itemId, string searchText);
    }
}