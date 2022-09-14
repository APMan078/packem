using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface ILotService
    {
        Task<Result<LotGetModel>> CreateLotAsync(AppState state, LotCreateModel model);
        Task<Result<IEnumerable<LotLookupGetModel>>> GetLotLookupByItemIdAsync(AppState state, int itemId, string searchText);
        Task<Result<IEnumerable<LotLookupGetModel>>> GetLotLookupByItemIdDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText);
        Task<Result<LotLookupGetModel>> GetLotByItemIdAndBinIdAsync(AppState state, int itemId, int binId);
        Task<Result<LotGetModel>> CreateLotDeviceAsync(CustomerDeviceTokenAuthModel state, LotCreateModel model);
    }
}