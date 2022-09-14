using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Data.Enums;

namespace Packem.Data.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public DashboardService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<DashboardInventoryFlowGetModel>> GetDashboardInventoryFlowAsync(AppState state, int customerLocationId, int customerFacilityId, LastDayFilterEnum dateFilter)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                DateTime currenteDate = DateTime.Now;
                if (!dateFilter.IsValueExistInEnum())
                {
                    return Result.Fail($"{nameof(LastDayFilterEnum)} not found.");
                }
                else
                {
                    if (dateFilter == LastDayFilterEnum.YTD)
                    {
                        currenteDate = DateTime.Now.AddDays(-365);
                    }
                    else if (dateFilter == LastDayFilterEnum.MTD)
                    {
                        currenteDate = DateTime.Now.AddDays(-30);
                    }
                    else if (dateFilter == LastDayFilterEnum.WTD)
                    {
                        currenteDate = DateTime.Now.AddDays(-7);
                    }
                }

                // inbound, putaway
                var pa = await _context.PutAwayBins
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.Bin.Zone.CustomerFacilityId == customerFacilityId
                        && x.ReceivedDateTime >= currenteDate)
                    .AsNoTracking()
                    .GroupBy(g => g.ReceivedDateTime.Date)
                    .Select(x => new
                    {
                        Date = x.Key,
                        Qty = x.Sum(z => z.Qty)
                    })
                    .ToListAsync();

                // outbound, picking
                var ol = await _context.OrderLineBins
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.Bin.Zone.CustomerFacilityId == customerFacilityId
                        && x.PickDateTime >= currenteDate)
                    .AsNoTracking()
                    .GroupBy(g => g.PickDateTime.Date)
                    .Select(x => new
                    {
                        Date = x.Key,
                        Qty = x.Sum(z => z.Qty)
                    })
                    .ToListAsync();


                var labels = new List<DateTime>();
                foreach (var x in pa)
                {
                    labels.Add(x.Date);
                }
                foreach (var x in ol)
                {
                    labels.Add(x.Date);
                }
                labels = labels.Distinct().OrderBy(x => x.Date).ToList();


                var poData = new List<int>();
                var olData = new List<int>();
                foreach (var x in labels)
                {
                    if (pa.Any(z => z.Date == x))
                    {
                        poData.Add(pa.SingleOrDefault(z => z.Date == x).Qty);
                    }
                    else
                    {
                        poData.Add(0);
                    }

                    if (ol.Any(z => z.Date == x))
                    {
                        olData.Add(ol.SingleOrDefault(z => z.Date == x).Qty);
                    }
                    else
                    {
                        olData.Add(0);
                    }
                }

                var dataSet = new DashboardInventoryFlowGetModel.DataSet
                {
                    Data = poData,
                    Label = "Inbound (Stocked)"
                };

                var dataSet2 = new DashboardInventoryFlowGetModel.DataSet
                {
                    Data = olData,
                    Label = "Outbound (Picked)"
                };

                var dataSets = new List<DashboardInventoryFlowGetModel.DataSet>();
                dataSets.Add(dataSet);
                dataSets.Add(dataSet2);

                var model = new DashboardInventoryFlowGetModel();
                model.Labels = labels.Select(x => x.ToString("MMMM dd, yyyy")).ToList();
                model.DataSets = dataSets;

                return Result.Ok(model);

            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<DashboardQueuesGetModel>> GetDashboardQueuesAsync(AppState state, int customerLocationId, int customerFacilityId, int days)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                DateTime currenteDate = DateTime.Now;

                if (days.IsNegative())
                {
                    return Result.Fail($"Days cannot be negative.");
                }
                else if (days.IsZero())
                {
                    return Result.Fail($"Days cannot be zero.");
                }

                currenteDate = DateTime.Now.AddDays(-days);

                var po = await _context.PurchaseOrders
                    .Include(x => x.CustomerLocation)
                        .ThenInclude(x => x.CustomerFacilities)
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId
                        && x.Status != PurchaseOrderStatusEnum.Closed
                        && x.OrderDate >= currenteDate)
                    .AsNoTracking()
                    .Select(x => x.Remaining)
                    .ToListAsync();

                var pa = await _context.PutAways
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                        && x.Remaining != 0)
                    .AsNoTracking()
                    .Select(x => x.Remaining)
                    .ToListAsync();

                var ol = await _context.OrderLines
                    .Include(x => x.SaleOrder)
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.SaleOrder.CustomerFacilityId == customerFacilityId
                        && x.Remaining != 0)
                    .AsNoTracking()
                    .Select(x => x.Remaining)
                    .ToListAsync();

                var t = await _context.Transfers
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerLocation.CustomerFacilities.Any(z => z.CustomerFacilityId == customerFacilityId)
                        && x.Remaining != 0)
                    .AsNoTracking()
                    .Select(x => x.Remaining)
                    .ToListAsync();

                var model = new DashboardQueuesGetModel();
                model.Expected = po.Sum();
                model.PurchaseOrders = po.Count;
                model.PutAway = pa.Sum();
                model.Pick = ol.Sum();
                model.Transfer = t.Sum();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<DashboardTopSalesOrdersGetModel>>> GetDashboardTopSalesOrdersAsync(AppState state, int customerLocationId, int customerFacilityId, LastDayFilterEnum dateFilter)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                DateTime currenteDate = DateTime.Now;
                if (!dateFilter.IsValueExistInEnum())
                {
                    return Result.Fail($"{nameof(LastDayFilterEnum)} not found.");
                }
                else
                {
                    if (dateFilter == LastDayFilterEnum.YTD)
                    {
                        currenteDate = DateTime.Now.AddDays(-365);
                    }
                    else if (dateFilter == LastDayFilterEnum.MTD)
                    {
                        currenteDate = DateTime.Now.AddDays(-30);
                    }
                    else if (dateFilter == LastDayFilterEnum.WTD)
                    {
                        currenteDate = DateTime.Now.AddDays(-7);
                    }
                }

                IEnumerable<DashboardTopSalesOrdersGetModel> model = await _context.SaleOrders
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId
                        && x.OrderDate >= currenteDate)
                    .AsNoTracking()
                    .Select(x => new DashboardTopSalesOrdersGetModel
                    {
                        SaleOrderId = x.SaleOrderId,
                        SaleOrderNo = x.SaleOrderNo,
                        Customer = x.OrderCustomer.Name,
                        Total = x.OrderLines
                            .Where(z => z.PerUnitPrice != null && z.Received != 0)
                            .Select(z => z.Received * z.PerUnitPrice.Value).Sum(),
                        Units = x.OrderQty,
                        SKUs = x.OrderLines.Select(x => x.Item.SKU).Count()
                    })
                    .Where(x => x.Total != 0)
                    .OrderBy(x => x.Total)
                    .Take(5)
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<DashboardLowStockGetModel>>> GetDashboardLowStockAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var items = await _context.Inventories
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Receives)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.Receipts)
                    .Include(x => x.Item)
                        .ThenInclude(x => x.OrderLines)
                    .Where(x => x.CustomerId == customerId
                        && x.Item.Threshold != null
                        && x.Item.Threshold >= x.QtyOnHand)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToListAsync();

                var model = new List<DashboardLowStockGetModel>();
                foreach (var x in items)
                {
                    var dto = new DashboardLowStockGetModel();
                    dto.ItemId = x.Item.ItemId;
                    dto.SKU = x.Item.SKU;
                    dto.Description = x.Item.Description;

                    var expect = 0;
                    foreach (var z in x.Item.Receives)
                    {
                        expect += z.Remaining;
                    }

                    foreach (var z in x.Item.Receipts)
                    {
                        expect += z.Remaining;
                    }

                    dto.Expect = expect;
                    dto.OnHand = x.QtyOnHand;
                    dto.Sold = x.Item.OrderLines.Sum(z => z.Received);
                    dto.Backorder = dto.Expect + dto.OnHand;

                    model.Add(dto);
                }

                return Result.Ok(model.AsEnumerable());
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<DashboardOperationsGetModel>> GetDashboardOperationsAsync(AppState state, int customerLocationId, int customerFacilityId, int days)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                DateTime currenteDate = DateTime.Now;

                if (days.IsNegative())
                {
                    return Result.Fail($"Days cannot be negative.");
                }
                else if (days.IsZero())
                {
                    return Result.Fail($"Days cannot be zero.");
                }

                currenteDate = DateTime.Now.AddDays(-days);

                var users = await _context.Users
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.IsActive
                        && (x.RoleId.Value == RoleEnum.OpsManager.ToInt() || x.RoleId.Value == RoleEnum.Operator.ToInt()))
                    .AsNoTracking()
                    .ToListAsync();

                var devices = await _context.CustomerDevices
                    .Where(x => x.IsActive
                        && x.CustomerLocationId == customerLocationId)
                    .AsNoTracking()
                    .ToListAsync();

                var bins = await _context.Bins
                    .Include(x => x.Zone)
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.Zone.CustomerFacilityId == customerFacilityId)
                    .AsNoTracking()
                    .ToListAsync();

                var binUse = await _context.InventoryBins
                    .Include(x => x.Bin)
                        .ThenInclude(x => x.Zone)
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.Bin.Zone.CustomerFacilityId == customerFacilityId)
                    .AsNoTracking()
                    .Select(x => x.BinId)
                    .Distinct()
                    .ToListAsync();

                var so = await _context.SaleOrders
                    .Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerFacilityId == customerFacilityId
                        && x.PickingStatus == PickingStatusEnum.Complete
                        && x.OrderDate >= currenteDate)
                    .AsNoTracking()
                    .ToListAsync();

                var model = new DashboardOperationsGetModel();
                model.Operators = users.Where(x => x.RoleId == RoleEnum.Operator.ToInt()).ToList().Count;
                model.OpsManagers = users.Where(x => x.RoleId == RoleEnum.OpsManager.ToInt()).ToList().Count;
                model.ActiveDevices = devices.Count;
                model.RegisteredBins = bins.Count;
                model.BinsInUse = binUse.Count;

                var utilization = (decimal)0;
                if(binUse.Count > 0)
                {
                    utilization = Math.Round((decimal)binUse.Count / bins.Count * 100, 1);
                }
                
                if (utilization.IsInteger())
                {
                    model.Utilization = $"{(int)utilization}%";
                }
                else
                {
                    model.Utilization = $"{utilization}%";
                }

                model.SalesOrders = so.Count;
                model.SalesOrdersUnits = so.Sum(x => x.OrderQty);

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }
    }
}