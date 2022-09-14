using FluentResults;
using Packem.Domain.Models;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IReceiptService
    {
        Task<Result<ReceiptGetModel>> CreateReceiptAsync(AppState state, ReceiptCreateModel model);
        Task<Result<ReceiptQueueGetModel>> GetReceiptQueueAsync(AppState state, int customerLocationId, int customerFacilityId);
        Task<Result<ReceiptGetModel>> CreateReceiptDeviceAsync(CustomerDeviceTokenAuthModel state, ReceiptDeviceCreateModel model);
    }
}