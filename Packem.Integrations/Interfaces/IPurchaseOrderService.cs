using FluentResults;
using Packem.Integrations.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Integrations.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<Result<PurchaseOrderGetModel>> CreatePurchaseOrderAsync(PurchaseOrderCreateModel model);
        Task<Result<PurchaseOrderGetModel>> GetPurchaseOrderDetailAsync(int purchaseOrderId);
    }
}