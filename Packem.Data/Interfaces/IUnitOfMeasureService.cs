using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IUnitOfMeasureService
    {
        Task<Result<IEnumerable<UnitOfMeasureGetModel>>> CreateUnitOfMeasureForCustomerAsync(AppState state, UnitOfMeasureForCustomerCreateModel model);
        Task<Result<UnitOfMeasureGetModel>> CreateCustomUnitOfMeasureForCustomerAsync(AppState state, CustomUnitOfMeasureForCustomerCreateModel model);
        Task<Result<UnitOfMeasureGetModel>> DeleteCustomerUnitOfMeasureAsync(AppState state, CustomerUnitOfMeasureDeleteModel model);
        Task<Result<IEnumerable<UnitOfMeasureGetModel>>> GetDefaultUnitOfMeasuresAsync(AppState state, string searchText);
        Task<Result<IEnumerable<UnitOfMeasureGetModel>>> GetCustomerUnitOfMeasuresAsync(AppState state, int customerId, string searchText);
    }
}