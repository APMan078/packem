using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IOrderCustomerService
    {
        Task<Result<OrderCustomerGetModel>> CreateOrderCustomerAsync(AppState state, OrderCustomerCreateModel model);
        Task<Result<OrderCustomerGetModel>> EditOrderCustomerAsync(AppState state, OrderCustomerEditModel model);
        Task<Result<OrderCustomerGetModel>> DeleteOrderCustomerAsync(AppState state, OrderCustomerDeleteModel model);
        Task<Result<IEnumerable<OrderCustomerGetModel>>> GetOrderCustomersAsync(AppState state, int customerId);
        Task<Result<OrderCustomerGetModel>> GetOrderCustomerAsync(AppState state, int orderCustomerId);
        Task<Result<IEnumerable<OrderCustomerManagementGetModel>>> GetOrderCustomerManagementAsync(AppState state, int customerId);
        Task<Result<OrderCustomerDetailGetModel>> GetOrderCustomerDetailAsync(AppState state, int orderCustomerId);
    }
}