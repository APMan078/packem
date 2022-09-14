using FluentResults;
using Packem.Domain.Common.Enums;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IOrderCustomerAddressService
    {
        Task<Result<OrderCustomerAddressGetModel>> CreateOrderCustomerAddressAsync(AppState state, OrderCustomerAddressCreateModel model);
        Task<Result<OrderCustomerAddressGetModel>> EditOrderCustomerAddressAsync(AppState state, OrderCustomerAddressEditModel model);
        Task<Result<OrderCustomerAddressGetModel>> DeleteOrderCustomerAddressAsync(AppState state, OrderCustomerAddressDeleteModel model);
        Task<Result<IEnumerable<OrderCustomerAddressGetModel>>> GetOrderCustomerAddressesAsync(AppState state, int orderCustomerId, OrderCustomerAddressType? addressType = null);
        Task<Result<OrderCustomerAddressGetModel>> GetOrderCustomerAddressAsync(AppState state, int orderCustomerAddressId);
    }
}