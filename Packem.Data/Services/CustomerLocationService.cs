using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.ExtensionMethods;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Entities;
using Packem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class CustomerLocationService : ICustomerLocationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;
        private readonly IStateService _stateService;

        public CustomerLocationService(ApplicationDbContext context,
            IExceptionService exceptionService,
            IStateService stateService)
        {
            _context = context;
            _exceptionService = exceptionService;
            _stateService = stateService;
        }

        public async Task<Result<CustomerLocationGetModel>> CreateCustomerLocationAsync(AppState state, CustomerLocationCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                int? customerId;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId;
                }
                else
                {
                    customerId = model.CustomerId;
                }

                var customerExist = await _context.Customers
                    .AnyAsync(x => x.CustomerId == customerId);

                if (!customerExist)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerLocationCreateModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerLocations
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.CustomerId == customerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerLocationCreateModel.Name)} already exists.");
                    }
                }

                //var stateResult = _stateService.GetState(model.State.ToInt());

                //if (stateResult.IsFailed)
                //{
                //    return Result.Fail(stateResult.Errors.ToErrorString());
                //}

                var entity = new CustomerLocation
                {
                    CustomerId = customerId,
                    Name = model.Name,
                    //StateId = model.State
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerLocationGetModel
                {
                    CustomerLocationId = entity.CustomerLocationId,
                    CustomerId = entity.CustomerId.Value,
                    Name = entity.Name,
                    //State = entity.StateId
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerLocationGetModel>> EditCustomerLocationAsync(AppState state, CustomerLocationEditModel model)
        {
            try
            {
                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(CustomerLocationEditModel.CustomerLocationId)} is required.");
                }

                CustomerLocation entity;

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    entity = await _context.CustomerLocations
                        .SingleOrDefaultAsync(x => x.CustomerLocationId == model.CustomerLocationId
                            && x.CustomerId == state.CustomerId);
                }
                else
                {
                    entity = await _context.CustomerLocations
                        .SingleOrDefaultAsync(x => x.CustomerLocationId == model.CustomerLocationId);
                }

                if (entity is null)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

                if (!model.Name.HasValue())
                {
                    return Result.Fail($"{nameof(CustomerLocationEditModel.Name)} is required.");
                }
                else
                {
                    var exist = await _context.CustomerLocations
                        .AnyAsync(x => x.Name.Trim().ToLower() == model.Name.Trim().ToLower()
                            && x.Name.Trim().ToLower() != entity.Name.Trim().ToLower()
                            && x.CustomerId == entity.CustomerId);

                    if (exist)
                    {
                        return Result.Fail($"{nameof(CustomerEditModel.Name)} is already exist.");
                    }
                }

                //var stateResult = _stateService.GetState(model.State.ToInt());

                //if (stateResult.IsFailed)
                //{
                //    return Result.Fail(stateResult.Errors.ToErrorString());
                //}

                entity.Name = model.Name;
                //entity.StateId = model.State;

                await _context.SaveChangesAsync();

                return Result.Ok(new CustomerLocationGetModel
                {
                    CustomerLocationId = entity.CustomerLocationId,
                    CustomerId = entity.CustomerId.Value,
                    Name = entity.Name,
                    //State = entity.StateId
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        //public async Task<Result<CustomerLocationGetModel>> DeleteCustomerLocationAsync(AppState state, CustomerLocationDeleteModel model)
        //{
        //    try
        //    {
        //        if (state is null)
        //        {
        //            return Result.Fail($"{nameof(AppState)} is null.");
        //        }

        //        if (model.CustomerLocationId is null)
        //        {
        //            return Result.Fail($"{nameof(CustomerLocationDeleteModel.CustomerLocationId)} is required.");
        //        }

        //        using (var transaction = _context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                _context.Database.SetCommandTimeout(900);

        //                CustomerLocation entity;

        //                if (state.Role != RoleEnum.SuperAdmin)
        //                {
        //                    entity = await _context.CustomerLocations
        //                        // CUSTOMERFACILITIES
        //                        .Include(x => x.CustomerFacilities)
        //                            .ThenInclude(x => x.PurchaseOrders)
        //                                .ThenInclude(x => x.Receives)
        //                                    .ThenInclude(x => x.PutAways)
        //                                        .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.InventoryZones) // InventoryZones
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.InventoryBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.ActivityLogs)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.ReceiptBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.TransferCurrents)
        //                                        .ThenInclude(x => x.Transfers)
        //                                            .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.TransferNews)
        //                                        .ThenInclude(x => x.Transfers)
        //                                            .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.AdjustBinQuantities)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.ActivityLogs) // ActivityLogs
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.TransferCurrents) // TransferCurrents
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.TransferNews) // TransferNews
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.TransferZoneBins) // TransferZoneBins
        //                                                                      // CUSTOMERDEVICES
        //                        .Include(x => x.CustomerDevices)
        //                            .ThenInclude(x => x.CustomerDeviceTokens)
        //                        // USERS
        //                        .Include(x => x.Users)
        //                            .ThenInclude(x => x.ErrorLogs)
        //                        .Include(x => x.Users)
        //                            .ThenInclude(x => x.ActivityLogs)
        //                        // ZONES
        //                        .Include(x => x.Zones)
        //                            .ThenInclude(x => x.InventoryZones) // InventoryZones
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.InventoryBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.ActivityLogs)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.ReceiptBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.TransferCurrents)
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.TransferNews)
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.AdjustBinQuantities)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.ActivityLogs) // ActivityLogs
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.TransferCurrents) // TransferCurrents
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.TransferNews) // TransferNews
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.TransferZoneBins) // TransferZoneBins
        //                                                                  // INVENTORYZONES
        //                        .Include(x => x.InventoryZones)
        //                        // BINS
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.InventoryBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.ActivityLogs)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.ReceiptBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.TransferCurrents)
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.TransferNews)
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.AdjustBinQuantities)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.TransferZoneBins)
        //                        // INVENTORYBINS
        //                        .Include(x => x.InventoryBins)
        //                        // PURCHASEORDERS
        //                        .Include(x => x.PurchaseOrders)
        //                            .ThenInclude(x => x.Receives)
        //                                .ThenInclude(x => x.PutAways)
        //                                    .ThenInclude(x => x.PutAwayBins)
        //                        // RECEIVES
        //                        .Include(x => x.Receives)
        //                                .ThenInclude(x => x.PutAways)
        //                                    .ThenInclude(x => x.PutAwayBins)
        //                        // PUTAWAYS
        //                        .Include(x => x.PutAways)
        //                            .ThenInclude(x => x.PutAwayBins)
        //                        // PUTAWAYBINS
        //                        .Include(x => x.PutAwayBins)
        //                        // RECEIPTS
        //                        .Include(x => x.Receipts)
        //                            .ThenInclude(x => x.ReceiptBins)
        //                        // RECEIPTBINS
        //                        .Include(x => x.ReceiptBins)
        //                        // TRANSFERS
        //                        .Include(x => x.Transfers)
        //                            .ThenInclude(x => x.TransferZoneBins)
        //                        // TRANSFERCURRENTS
        //                        .Include(x => x.TransferCurrents)
        //                            .ThenInclude(x => x.Transfers)
        //                                .ThenInclude(x => x.TransferZoneBins)
        //                        // TRANSFERNEWS
        //                        .Include(x => x.TransferNews)
        //                            .ThenInclude(x => x.Transfers)
        //                                .ThenInclude(x => x.TransferZoneBins)
        //                        // ADJUSTBINQUANTITIES
        //                        .Include(x => x.AdjustBinQuantities)
        //                        // TRANSFERZONEBINS
        //                        .Include(x => x.TransferZoneBins)
        //                        .SingleOrDefaultAsync(x => x.CustomerLocationId == model.CustomerLocationId
        //                            && x.CustomerId == state.CustomerId);
        //                }
        //                else
        //                {
        //                    entity = await _context.CustomerLocations
        //                        // CUSTOMERFACILITIES
        //                        .Include(x => x.CustomerFacilities)
        //                            .ThenInclude(x => x.PurchaseOrders)
        //                                .ThenInclude(x => x.Receives)
        //                                    .ThenInclude(x => x.PutAways)
        //                                        .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.InventoryZones) // InventoryZones
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.InventoryBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.ActivityLogs)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.ReceiptBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.TransferCurrents)
        //                                        .ThenInclude(x => x.Transfers)
        //                                            .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.TransferNews)
        //                                        .ThenInclude(x => x.Transfers)
        //                                            .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.AdjustBinQuantities)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.Bins) // Bins
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.ActivityLogs) // ActivityLogs
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.TransferCurrents) // TransferCurrents
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.TransferNews) // TransferNews
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.CustomerFacilities) // Zones
        //                            .ThenInclude(x => x.Zones)
        //                                .ThenInclude(x => x.TransferZoneBins) // TransferZoneBins
        //                                                                      // CUSTOMERDEVICES
        //                        .Include(x => x.CustomerDevices)
        //                            .ThenInclude(x => x.CustomerDeviceTokens)
        //                        // USERS
        //                        .Include(x => x.Users)
        //                            .ThenInclude(x => x.ErrorLogs)
        //                        .Include(x => x.Users)
        //                            .ThenInclude(x => x.ActivityLogs)
        //                        // ZONES
        //                        .Include(x => x.Zones)
        //                            .ThenInclude(x => x.InventoryZones) // InventoryZones
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.InventoryBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.ActivityLogs)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.ReceiptBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.TransferCurrents)
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.TransferNews)
        //                                    .ThenInclude(x => x.Transfers)
        //                                        .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.AdjustBinQuantities)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.Bins) // Bins
        //                                .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.ActivityLogs) // ActivityLogs
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.TransferCurrents) // TransferCurrents
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.TransferNews) // TransferNews
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Zones) // Zones
        //                            .ThenInclude(x => x.TransferZoneBins) // TransferZoneBins
        //                                                                  // INVENTORYZONES
        //                        .Include(x => x.InventoryZones)
        //                        // BINS
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.InventoryBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.ActivityLogs)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.PutAwayBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.ReceiptBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.TransferCurrents)
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.TransferNews)
        //                                .ThenInclude(x => x.Transfers)
        //                                    .ThenInclude(x => x.TransferZoneBins)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.AdjustBinQuantities)
        //                        .Include(x => x.Bins)
        //                            .ThenInclude(x => x.TransferZoneBins)
        //                        // INVENTORYBINS
        //                        .Include(x => x.InventoryBins)
        //                        // PURCHASEORDERS
        //                        .Include(x => x.PurchaseOrders)
        //                            .ThenInclude(x => x.Receives)
        //                                .ThenInclude(x => x.PutAways)
        //                                    .ThenInclude(x => x.PutAwayBins)
        //                        // RECEIVES
        //                        .Include(x => x.Receives)
        //                                .ThenInclude(x => x.PutAways)
        //                                    .ThenInclude(x => x.PutAwayBins)
        //                        // PUTAWAYS
        //                        .Include(x => x.PutAways)
        //                            .ThenInclude(x => x.PutAwayBins)
        //                        // PUTAWAYBINS
        //                        .Include(x => x.PutAwayBins)
        //                        // RECEIPTS
        //                        .Include(x => x.Receipts)
        //                            .ThenInclude(x => x.ReceiptBins)
        //                        // RECEIPTBINS
        //                        .Include(x => x.ReceiptBins)
        //                        // TRANSFERS
        //                        .Include(x => x.Transfers)
        //                            .ThenInclude(x => x.TransferZoneBins)
        //                        // TRANSFERCURRENTS
        //                        .Include(x => x.TransferCurrents)
        //                            .ThenInclude(x => x.Transfers)
        //                                .ThenInclude(x => x.TransferZoneBins)
        //                        // TRANSFERNEWS
        //                        .Include(x => x.TransferNews)
        //                            .ThenInclude(x => x.Transfers)
        //                                .ThenInclude(x => x.TransferZoneBins)
        //                        // ADJUSTBINQUANTITIES
        //                        .Include(x => x.AdjustBinQuantities)
        //                        // TRANSFERZONEBINS
        //                        .Include(x => x.TransferZoneBins)
        //                        .SingleOrDefaultAsync(x => x.CustomerLocationId == model.CustomerLocationId);
        //                }

        //                if (entity is null)
        //                {
        //                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
        //                }

        //                entity.Deleted = true;

        //                foreach (var x in entity.CustomerFacilities)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var po in x.PurchaseOrders)
        //                    {
        //                        po.Deleted = true;

        //                        foreach (var r in po.Receives)
        //                        {
        //                            r.Deleted = true;

        //                            foreach (var pa in r.PutAways)
        //                            {
        //                                pa.Deleted = true;

        //                                foreach (var pab in pa.PutAwayBins)
        //                                {
        //                                    pab.Deleted = true;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    foreach (var z in x.Zones)
        //                    {
        //                        z.Deleted = true;

        //                        foreach (var iz in z.InventoryZones)
        //                        {
        //                            iz.Deleted = true;
        //                        }

        //                        foreach (var b in z.Bins)
        //                        {
        //                            b.Deleted = true;

        //                            foreach (var ib in b.InventoryBins)
        //                            {
        //                                ib.Deleted = true;
        //                            }

        //                            foreach (var al in b.ActivityLogs)
        //                            {
        //                                al.Deleted = true;
        //                            }

        //                            foreach (var pb in b.PutAwayBins)
        //                            {
        //                                pb.Deleted = true;
        //                            }

        //                            foreach (var rb in b.ReceiptBins)
        //                            {
        //                                rb.Deleted = true;
        //                            }

        //                            foreach (var tc in b.TransferCurrents)
        //                            {
        //                                tc.Deleted = true;

        //                                foreach (var t in tc.Transfers)
        //                                {
        //                                    t.Deleted = true;

        //                                    foreach (var tz in t.TransferZoneBins)
        //                                    {
        //                                        tz.Deleted = true;
        //                                    }
        //                                }
        //                            }

        //                            foreach (var tn in b.TransferNews)
        //                            {
        //                                tn.Deleted = true;

        //                                foreach (var t in tn.Transfers)
        //                                {
        //                                    t.Deleted = true;

        //                                    foreach (var tz in t.TransferZoneBins)
        //                                    {
        //                                        tz.Deleted = true;
        //                                    }
        //                                }
        //                            }

        //                            foreach (var ab in b.AdjustBinQuantities)
        //                            {
        //                                ab.Deleted = true;
        //                            }

        //                            foreach (var tz in b.TransferZoneBins)
        //                            {
        //                                tz.Deleted = true;
        //                            }
        //                        }

        //                        foreach (var al in z.ActivityLogs)
        //                        {
        //                            al.Deleted = true;
        //                        }

        //                        foreach (var tc in z.TransferCurrents)
        //                        {
        //                            tc.Deleted = true;

        //                            foreach (var t in tc.Transfers)
        //                            {
        //                                t.Deleted = true;

        //                                foreach (var tz in t.TransferZoneBins)
        //                                {
        //                                    tz.Deleted = true;
        //                                }
        //                            }
        //                        }

        //                        foreach (var tn in z.TransferNews)
        //                        {
        //                            tn.Deleted = true;

        //                            foreach (var t in tn.Transfers)
        //                            {
        //                                t.Deleted = true;

        //                                foreach (var tz in t.TransferZoneBins)
        //                                {
        //                                    tz.Deleted = true;
        //                                }
        //                            }
        //                        }

        //                        foreach (var tz in z.TransferZoneBins)
        //                        {
        //                            tz.Deleted = true;
        //                        }
        //                    }
        //                }

        //                foreach (var x in entity.CustomerDevices)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.CustomerDeviceTokens)
        //                    {
        //                        z.Deleted = true;
        //                    }
        //                }

        //                foreach (var x in entity.Users)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.ErrorLogs)
        //                    {
        //                        z.Deleted = true;
        //                    }

        //                    foreach (var z in x.ActivityLogs)
        //                    {
        //                        z.Deleted = true;
        //                    }
        //                }

        //                foreach (var z in entity.Zones)
        //                {
        //                    z.Deleted = true;

        //                    foreach (var iz in z.InventoryZones)
        //                    {
        //                        iz.Deleted = true;
        //                    }

        //                    foreach (var b in z.Bins)
        //                    {
        //                        b.Deleted = true;

        //                        foreach (var ib in b.InventoryBins)
        //                        {
        //                            ib.Deleted = true;
        //                        }

        //                        foreach (var al in b.ActivityLogs)
        //                        {
        //                            al.Deleted = true;
        //                        }

        //                        foreach (var pb in b.PutAwayBins)
        //                        {
        //                            pb.Deleted = true;
        //                        }

        //                        foreach (var rb in b.ReceiptBins)
        //                        {
        //                            rb.Deleted = true;
        //                        }

        //                        foreach (var tc in b.TransferCurrents)
        //                        {
        //                            tc.Deleted = true;

        //                            foreach (var t in tc.Transfers)
        //                            {
        //                                t.Deleted = true;

        //                                foreach (var tz in t.TransferZoneBins)
        //                                {
        //                                    tz.Deleted = true;
        //                                }
        //                            }
        //                        }

        //                        foreach (var tn in b.TransferNews)
        //                        {
        //                            tn.Deleted = true;

        //                            foreach (var t in tn.Transfers)
        //                            {
        //                                t.Deleted = true;

        //                                foreach (var tz in t.TransferZoneBins)
        //                                {
        //                                    tz.Deleted = true;
        //                                }
        //                            }
        //                        }

        //                        foreach (var ab in b.AdjustBinQuantities)
        //                        {
        //                            ab.Deleted = true;
        //                        }

        //                        foreach (var tz in b.TransferZoneBins)
        //                        {
        //                            tz.Deleted = true;
        //                        }
        //                    }

        //                    foreach (var al in z.ActivityLogs)
        //                    {
        //                        al.Deleted = true;
        //                    }

        //                    foreach (var tc in z.TransferCurrents)
        //                    {
        //                        tc.Deleted = true;

        //                        foreach (var t in tc.Transfers)
        //                        {
        //                            t.Deleted = true;

        //                            foreach (var tz in t.TransferZoneBins)
        //                            {
        //                                tz.Deleted = true;
        //                            }
        //                        }
        //                    }

        //                    foreach (var tn in z.TransferNews)
        //                    {
        //                        tn.Deleted = true;

        //                        foreach (var t in tn.Transfers)
        //                        {
        //                            t.Deleted = true;

        //                            foreach (var tz in t.TransferZoneBins)
        //                            {
        //                                tz.Deleted = true;
        //                            }
        //                        }
        //                    }

        //                    foreach (var tz in z.TransferZoneBins)
        //                    {
        //                        tz.Deleted = true;
        //                    }
        //                }

        //                foreach (var x in entity.InventoryZones)
        //                {
        //                    x.Deleted = true;
        //                }

        //                foreach (var b in entity.Bins)
        //                {
        //                    b.Deleted = true;

        //                    foreach (var ib in b.InventoryBins)
        //                    {
        //                        ib.Deleted = true;
        //                    }

        //                    foreach (var al in b.ActivityLogs)
        //                    {
        //                        al.Deleted = true;
        //                    }

        //                    foreach (var pb in b.PutAwayBins)
        //                    {
        //                        pb.Deleted = true;
        //                    }

        //                    foreach (var rb in b.ReceiptBins)
        //                    {
        //                        rb.Deleted = true;
        //                    }

        //                    foreach (var tc in b.TransferCurrents)
        //                    {
        //                        tc.Deleted = true;

        //                        foreach (var t in tc.Transfers)
        //                        {
        //                            t.Deleted = true;

        //                            foreach (var tz in t.TransferZoneBins)
        //                            {
        //                                tz.Deleted = true;
        //                            }
        //                        }
        //                    }

        //                    foreach (var tn in b.TransferNews)
        //                    {
        //                        tn.Deleted = true;

        //                        foreach (var t in tn.Transfers)
        //                        {
        //                            t.Deleted = true;

        //                            foreach (var tz in t.TransferZoneBins)
        //                            {
        //                                tz.Deleted = true;
        //                            }
        //                        }
        //                    }

        //                    foreach (var ab in b.AdjustBinQuantities)
        //                    {
        //                        ab.Deleted = true;
        //                    }

        //                    foreach (var tz in b.TransferZoneBins)
        //                    {
        //                        tz.Deleted = true;
        //                    }
        //                }

        //                foreach (var x in entity.InventoryBins)
        //                {
        //                    x.Deleted = true;
        //                }

        //                foreach (var x in entity.PurchaseOrders)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.Receives)
        //                    {
        //                        z.Deleted = true;

        //                        foreach (var xx in z.PutAways)
        //                        {
        //                            xx.Deleted = true;

        //                            foreach (var zz in xx.PutAwayBins)
        //                            {
        //                                zz.Deleted = true;
        //                            }
        //                        }
        //                    }
        //                }

        //                foreach (var z in entity.Receives)
        //                {
        //                    z.Deleted = true;

        //                    foreach (var xx in z.PutAways)
        //                    {
        //                        xx.Deleted = true;

        //                        foreach (var zz in xx.PutAwayBins)
        //                        {
        //                            zz.Deleted = true;
        //                        }
        //                    }
        //                }

        //                foreach (var x in entity.PutAways)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.PutAwayBins)
        //                    {
        //                        x.Deleted = true;
        //                    }
        //                }

        //                foreach (var x in entity.PutAwayBins)
        //                {
        //                    x.Deleted = true;
        //                }

        //                foreach (var x in entity.Receipts)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.ReceiptBins)
        //                    {
        //                        z.Deleted = true;
        //                    }
        //                }

        //                foreach (var x in entity.ReceiptBins)
        //                {
        //                    x.Deleted = true;
        //                }

        //                foreach (var x in entity.Transfers)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.TransferZoneBins)
        //                    {
        //                        z.Deleted = true;
        //                    }
        //                }

        //                foreach (var x in entity.TransferCurrents)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.Transfers)
        //                    {
        //                        z.Deleted = true;

        //                        foreach (var xx in z.TransferZoneBins)
        //                        {
        //                            xx.Deleted = true;
        //                        }
        //                    }
        //                }

        //                foreach (var x in entity.TransferNews)
        //                {
        //                    x.Deleted = true;

        //                    foreach (var z in x.Transfers)
        //                    {
        //                        z.Deleted = true;

        //                        foreach (var xx in z.TransferZoneBins)
        //                        {
        //                            xx.Deleted = true;
        //                        }
        //                    }
        //                }

        //                foreach (var x in entity.AdjustBinQuantities)
        //                {
        //                    x.Deleted = true;
        //                }

        //                foreach (var x in entity.TransferZoneBins)
        //                {
        //                    x.Deleted = true;
        //                }

        //                await _context.SaveChangesAsync();

        //                // sum all zones of selected inventory
        //                var inventories = await _context.Inventories
        //                    .Include(x => x.InventoryZones)
        //                    .Where(x => x.CustomerId == entity.CustomerId)
        //                    .ToListAsync();

        //                foreach (var x in inventories)
        //                {
        //                    x.QtyOnHand = x.InventoryZones.Where(x => !x.Deleted).Sum(z => z.Qty);
        //                }
        //                await _context.SaveChangesAsync();

        //                await transaction.CommitAsync();

        //                return Result.Ok(new CustomerLocationGetModel
        //                {
        //                    CustomerLocationId = entity.CustomerLocationId,
        //                    CustomerId = entity.CustomerId.Value,
        //                    Name = entity.Name,
        //                    //State = entity.StateId
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                await transaction.RollbackAsync();
        //                ex = await _exceptionService.HandleExceptionAsync(ex);
        //                return Result.Fail(ex.ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex = await _exceptionService.HandleExceptionAsync(ex);
        //        return Result.Fail(ex.ToString());
        //    }
        //}

        public async Task<Result<CustomerLocationGetModel>> DeleteCustomerLocationAsync(AppState state, CustomerLocationDeleteModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerLocationId is null)
                {
                    return Result.Fail($"{nameof(CustomerLocationDeleteModel.CustomerLocationId)} is required.");
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        CustomerLocation entity;

                        if (state.Role != RoleEnum.SuperAdmin)
                        {
                            entity = await _context.CustomerLocations
                                .Include(x => x.CustomerFacilities)
                                .Include(x => x.CustomerDevices)
                                    .ThenInclude(x => x.CustomerDeviceTokens)
                                .Include(x => x.Users)
                                    .ThenInclude(x => x.ErrorLogs)
                                .Include(x => x.Zones)
                                .Include(x => x.InventoryZones)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.InventoryBins)
                                .Include(x => x.PurchaseOrders)
                                .Include(x => x.Receives)
                                .Include(x => x.PutAways)
                                .Include(x => x.PutAwayBins)
                                .Include(x => x.Receipts)
                                .Include(x => x.Transfers)
                                .Include(x => x.TransferCurrents)
                                .Include(x => x.TransferNews)
                                .Include(x => x.AdjustBinQuantities)
                                .Include(x => x.TransferZoneBins)
                                .Include(x => x.SaleOrders)
                                .Include(x => x.OrderLines)
                                .Include(x => x.OrderLineBins)
                                .Include(x => x.Recalls)
                                .Include(x => x.RecallBins)
                                .AsSplitQuery()
                                .SingleOrDefaultAsync(x => x.CustomerLocationId == model.CustomerLocationId
                                    && x.CustomerId == state.CustomerId);
                        }
                        else
                        {
                            entity = await _context.CustomerLocations
                                .Include(x => x.CustomerFacilities)
                                .Include(x => x.CustomerDevices)
                                    .ThenInclude(x => x.CustomerDeviceTokens)
                                .Include(x => x.Users)
                                    .ThenInclude(x => x.ErrorLogs)
                                .Include(x => x.Zones)
                                .Include(x => x.InventoryZones)
                                .Include(x => x.Bins)
                                    .ThenInclude(x => x.ActivityLogs)
                                .Include(x => x.InventoryBins)
                                .Include(x => x.PurchaseOrders)
                                .Include(x => x.Receives)
                                .Include(x => x.PutAways)
                                .Include(x => x.PutAwayBins)
                                .Include(x => x.Receipts)
                                .Include(x => x.Transfers)
                                .Include(x => x.TransferCurrents)
                                .Include(x => x.TransferNews)
                                .Include(x => x.AdjustBinQuantities)
                                .Include(x => x.TransferZoneBins)
                                .Include(x => x.SaleOrders)
                                .Include(x => x.OrderLines)
                                .Include(x => x.OrderLineBins)
                                .Include(x => x.Recalls)
                                .Include(x => x.RecallBins)
                                .AsSplitQuery()
                                .SingleOrDefaultAsync(x => x.CustomerLocationId == model.CustomerLocationId);
                        }

                        if (entity is null)
                        {
                            return Result.Fail($"{nameof(CustomerLocation)} not found.");
                        }

                        entity.Deleted = true;

                        foreach (var x in entity.CustomerFacilities)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.CustomerDevices)
                        {
                            x.Deleted = true;

                            foreach (var z in x.CustomerDeviceTokens)
                            {
                                z.Deleted = true;
                            }
                        }

                        foreach (var x in entity.Users)
                        {
                            x.Deleted = true;

                            foreach (var z in x.ErrorLogs)
                            {
                                z.Deleted = true;
                            }
                        }

                        foreach (var x in entity.Zones)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.InventoryZones)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Bins)
                        {
                            x.Deleted = true;

                            foreach (var z in x.ActivityLogs)
                            {
                                z.Deleted = true;
                            }
                        }

                        foreach (var x in entity.InventoryBins)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.PurchaseOrders)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Receives)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.PutAways)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.PutAwayBins)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Receipts)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Transfers)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.TransferCurrents)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.TransferNews)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.AdjustBinQuantities)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.TransferZoneBins)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.SaleOrders)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.OrderLines)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.OrderLineBins)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.Recalls)
                        {
                            x.Deleted = true;
                        }

                        foreach (var x in entity.RecallBins)
                        {
                            x.Deleted = true;
                        }

                        await _context.SaveChangesAsync();

                        // sum all zones of selected inventory
                        var inventories = await _context.Inventories
                            .Include(x => x.InventoryZones)
                            .Where(x => x.CustomerId == entity.CustomerId)
                            .ToListAsync();

                        foreach (var x in inventories)
                        {
                            x.QtyOnHand = x.InventoryZones.Where(x => !x.Deleted).Sum(z => z.Qty);
                        }
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new CustomerLocationGetModel
                        {
                            CustomerLocationId = entity.CustomerLocationId,
                            CustomerId = entity.CustomerId.Value,
                            Name = entity.Name,
                            //State = entity.StateId
                        });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ex = await _exceptionService.HandleExceptionAsync(ex);
                        return Result.Fail(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<CustomerLocationGetModel>>> GetCustomerLocationsSuperAdminAsync()
        {
            try
            {
                IEnumerable<CustomerLocationGetModel> model = await _context.CustomerLocations
                    .AsNoTracking()
                    .Select(x => new CustomerLocationGetModel
                    {
                        CustomerLocationId = x.CustomerLocationId,
                        CustomerId = x.CustomerId.Value,
                        Name = x.Name,
                    })
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<CustomerLocationGetModel>>> GetCustomerLocationsByCustomerIdAsync(int customerId)
        {
            try
            {
                IEnumerable<CustomerLocationGetModel> model = await _context.CustomerLocations
                    .AsNoTracking()
                    .Select(x => new CustomerLocationGetModel
                    {
                        CustomerLocationId = x.CustomerLocationId,
                        CustomerId = x.CustomerId.Value,
                        Name = x.Name,
                        //State = x.StateId
                    })
                    .Where(x => x.CustomerId == customerId)
                    .ToListAsync();

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<CustomerLocationGetModel>> GetCustomerLocationAsync(AppState state, int customerLocationId)
        {
            try
            {
                var query = _context.CustomerLocations.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query.Where(x => x.CustomerLocationId == customerLocationId
                        && x.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query.Where(x => x.CustomerLocationId == customerLocationId);
                }

                var model = await query
                    .AsNoTracking()
                    .Select(x => new CustomerLocationGetModel
                    {
                        CustomerLocationId = x.CustomerLocationId,
                        CustomerId = x.CustomerId.Value,
                        Name = x.Name,
                        //State = x.StateId
                    })
                    .SingleOrDefaultAsync();

                if (model == null)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }

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
