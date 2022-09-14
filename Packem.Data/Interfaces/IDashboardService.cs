using FluentResults;
using Packem.Data.Enums;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IDashboardService
    {
        Task<Result<DashboardInventoryFlowGetModel>> GetDashboardInventoryFlowAsync(AppState state, int customerLocationId, int customerFacilityId, LastDayFilterEnum dateFilter);
        Task<Result<DashboardQueuesGetModel>> GetDashboardQueuesAsync(AppState state, int customerLocationId, int customerFacilityId, int days);
        Task<Result<IEnumerable<DashboardTopSalesOrdersGetModel>>> GetDashboardTopSalesOrdersAsync(AppState state, int customerLocationId, int customerFacilityId, LastDayFilterEnum dateFilter);
        Task<Result<IEnumerable<DashboardLowStockGetModel>>> GetDashboardLowStockAsync(AppState state, int customerId);
        Task<Result<DashboardOperationsGetModel>> GetDashboardOperationsAsync(AppState state, int customerLocationId, int customerFacilityId, int days);
    }
}