using FluentResults;
using Packem.Domain.Models;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IInventoryService
    {
        Task<Result<InventoryDropDownGetModel>> GetInventoryDropDownAsync(AppState state, int customerId);
        Task<Result<InventoryGetModel>> CreateInventoryAsync(AppState state, InventoryCreateModel model);
    }
}