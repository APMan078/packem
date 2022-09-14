using FluentResults;
using Packem.Domain.Models;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IAdjustBinQuantityService
    {
        Task<Result<CreateAdjustBinQuantityGetModel>> GetCreateAdjustBinQuantityAsync(AppState state, int itemId, int customerLocationId, int binId);
        Task<Result<AdjustBinQuantityGetModel>> CreateAdjustBinQuantityAsync(AppState state, AdjustBinQuantityCreateModel model);
    }
}