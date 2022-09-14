using FluentResults;
using Microsoft.EntityFrameworkCore;
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
    public class LicensePlateService : ILicensePlateService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public LicensePlateService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<string>> GetGenerateLicensePlateNoAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Customers.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query
                        .Where(x => x.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query
                        .Where(x => x.CustomerId == customerId);
                }

                var exist = await query
                    .AnyAsync();

                if (!exist)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                var lpStr = await _context.LicensePlates
                    .Where(x => x.CustomerId == customerId)
                    .OrderByDescending(x => x.LicensePlateId)
                    .Take(1)
                    .AsNoTracking()
                    .Select(x => x.LicensePlateNo)
                    .SingleOrDefaultAsync();

                var lp = Convert.ToInt64(lpStr);
                lp = lp + 1;

                return Result.Ok($"{lp:D8}");
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LicensePlateGetModel>> CreateLicensePlateUnknownAsync(AppState state, LicensePlateUnknownCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlateUnknownCreateModel)} is required.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(LicensePlateUnknownCreateModel.CustomerId)} is required.");
                }
                else
                {
                    var query = _context.Customers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }

                if (!model.LicensePlateNo.HasValue())
                {
                    return Result.Fail($"{nameof(LicensePlateUnknownCreateModel.LicensePlateNo)} is required.");
                }
                else
                {
                    if (!model.LicensePlateNo.IsNumeric())
                    {
                        return Result.Fail($"{nameof(LicensePlateUnknownCreateModel.LicensePlateNo)} must be numeric.");
                    }
                    else
                    {
                        var exist = await _context.LicensePlates
                            .AnyAsync(x => x.LicensePlateNo.Trim().ToLower() == model.LicensePlateNo.Trim().ToLower()
                                && x.CustomerId == model.CustomerId);

                        if (exist)
                        {
                            return Result.Fail($"{nameof(LicensePlateUnknownCreateModel.LicensePlateNo)} is already exist.");
                        }
                    }
                }

                var entity = new LicensePlate
                {
                    CustomerId = model.CustomerId,
                    LicensePlateNo = model.LicensePlateNo,
                    LicensePlateType = LicensePlateTypeEnum.Unknown,
                    Printed = true,
                    UserId = state.UserId,
                    CreatedDateTime = DateTime.Now,
                    Palletized = false
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new LicensePlateGetModel
                {
                    LicensePlateId = entity.LicensePlateId,
                    CustomerId = entity.CustomerId.Value,
                    LicensePlateNo = entity.LicensePlateNo,
                    LicensePlateType = entity.LicensePlateType
                });
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LicensePlateGetModel>> CreateLicensePlateKnownAsync(AppState state, LicensePlateKnownCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlateKnownCreateModel)} is required.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(LicensePlateKnownCreateModel.CustomerId)} is required.");
                }
                else
                {
                    var query = _context.Customers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }

                if (!model.LicensePlateNo.HasValue())
                {
                    return Result.Fail($"{nameof(LicensePlateKnownCreateModel.LicensePlateNo)} is required.");
                }
                else
                {
                    if (!model.LicensePlateNo.IsNumeric())
                    {
                        return Result.Fail($"{nameof(LicensePlateKnownCreateModel.LicensePlateNo)} must be numeric.");
                    }
                    else
                    {
                        var exist = await _context.LicensePlates
                            .AnyAsync(x => x.LicensePlateNo.Trim().ToLower() == model.LicensePlateNo.Trim().ToLower()
                                && x.CustomerId == model.CustomerId);

                        if (exist)
                        {
                            return Result.Fail($"{nameof(LicensePlateKnownCreateModel.LicensePlateNo)} is already exist.");
                        }
                    }
                }

                if (model.Products is null || model.Products.Count() == 0)
                {
                    return Result.Fail($"{nameof(model.Products)} is required.");
                }
                else
                {
                    if (model.Products.Any(x => x.ItemId is null))
                    {
                        return Result.Fail($"Product SKU is required.");
                    }
                    else
                    {
                        foreach (var x in model.Products)
                        {
                            var exist = await _context.Items
                                .AnyAsync(z => z.ItemId == x.ItemId
                                    && z.CustomerId == model.CustomerId);

                            if (!exist)
                            {
                                return Result.Fail($"{nameof(Item)} not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.VendorId is not null))
                    {
                        foreach (var x in model.Products.Where(z => z.VendorId is not null))
                        {
                            var exist = await _context.Vendors
                                .AnyAsync(z => z.VendorId == x.VendorId
                                    && z.CustomerId == model.CustomerId);

                            if (!exist)
                            {
                                return Result.Fail($"{nameof(Vendor)} not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.LotId is not null))
                    {
                        foreach (var x in model.Products.Where(z => z.LotId is not null))
                        {
                            var exist = await _context.Lots
                                .AnyAsync(z => z.LotId == x.LotId
                                    && z.ItemId == x.ItemId);

                            if (!exist)
                            {
                                return Result.Fail("Lot not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.TotalQty is null))
                    {
                        return Result.Fail($"Product Total Qty is required.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = new LicensePlate
                        {
                            CustomerId = model.CustomerId,
                            LicensePlateNo = model.LicensePlateNo,
                            LicensePlateType = LicensePlateTypeEnum.Known,
                            ArrivalDateTime = model.ArrivalDateTime,
                            Printed = true,
                            UserId = state.UserId,
                            CreatedDateTime = DateTime.Now,
                            Palletized = false
                        };

                        _context.Add(entity);
                        await _context.SaveChangesAsync();

                        foreach (var x in model.Products)
                        {
                            var lpItem = new LicensePlateItem
                            {
                                CustomerId = model.CustomerId,
                                LicensePlateId = entity.LicensePlateId,
                                ItemId = x.ItemId,
                                VendorId = x.VendorId,
                                LotId = x.LotId,
                                ReferenceNo = x.ReferenceNo,
                                Cases = x.Cases,
                                EaCase = x.EaCase,
                                TotalQty = x.TotalQty.Value,
                                Qty = x.TotalQty.Value,
                                TotalWeight = x.TotalWeight
                            };
                            _context.Add(lpItem);
                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();

                        return Result.Ok(new LicensePlateGetModel
                        {
                            LicensePlateId = entity.LicensePlateId,
                            CustomerId = entity.CustomerId.Value,
                            LicensePlateNo = entity.LicensePlateNo,
                            LicensePlateType = entity.LicensePlateType
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

        public async Task<Result<IEnumerable<LicensePlateLookupLPNoGetModel>>> GetLicensePlateLookupByLicensePlateNoAsync(AppState state, int customerId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.LicensePlates
                    .AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    customerId = state.CustomerId.Value;
                }

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    query = query.Where(x => x.CustomerId == customerId
                        && x.Palletized == false
                        && x.LicensePlateNo.Trim().ToLower().Contains(searchText));
                }
                else
                {
                    query = query.Where(x => x.CustomerId == customerId && x.Palletized == false);
                }

                IEnumerable<LicensePlateLookupLPNoGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new LicensePlateLookupLPNoGetModel
                    {
                        LicensePlateId = x.LicensePlateId,
                        LicensePlateNo = x.LicensePlateNo
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlate)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LicensePlateKnownAssignmentGetModel>> GetLicensePlateKnownAssignmentAsync(AppState state, int customerId, int licensePlateId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var queryC = _context.Customers.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    queryC = queryC
                        .Where(x => x.CustomerId == state.CustomerId);
                }
                else
                {
                    queryC = queryC
                        .Where(x => x.CustomerId == customerId);
                }

                var existC = await queryC
                    .AnyAsync();

                if (!existC)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                var lp = await _context.LicensePlates
                .Include(x => x.LicensePlateItems)
                    .ThenInclude(x => x.Item)
                .Include(x => x.LicensePlateItems)
                    .ThenInclude(x => x.Vendor)
                .Include(x => x.LicensePlateItems)
                    .ThenInclude(x => x.Lot)
                .Where(x => x.CustomerId == customerId
                    && x.LicensePlateId == licensePlateId
                    && x.Palletized == false)
                .AsNoTracking()
                .SingleOrDefaultAsync();

                if (lp is null)
                {
                    return null;
                }

                var model = new LicensePlateKnownAssignmentGetModel();
                model.LicensePlateId = lp.LicensePlateId;
                model.ArrivalDateTime = lp.ArrivalDateTime;

                var products = new List<LicensePlateKnownAssignmentGetModel.Product>();
                foreach (var x in lp.LicensePlateItems)
                {
                    var p = new LicensePlateKnownAssignmentGetModel.Product();
                    p.LicensePlateItemId = x.LicensePlateItemId;
                    p.ItemId = x.Item.ItemId;
                    p.ItemSKU = x.Item.SKU;

                    if (x.Vendor is not null)
                    {
                        p.VendorId = x.Vendor.VendorId;
                        p.VendorName = x.Vendor.Name;
                    }

                    if (x.Lot is not null)
                    {
                        p.LotId = x.Lot.LotId;
                        p.LotNo = x.Lot.LotNo;
                    }

                    p.ReferenceNo = x.ReferenceNo;
                    p.Cases = x.Cases;
                    p.EaCase = x.EaCase;
                    p.TotalQty = x.TotalQty;
                    p.TotalWeight = x.TotalWeight;

                    products.Add(p);
                }
                model.Products = products;

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlate)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LicensePlateGetModel>> EditLicensePlateKnownAssignmentAsync(AppState state, LicensePlateKnownAssignmentEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlateKnownAssignmentEditModel)} is required.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(LicensePlateKnownAssignmentEditModel.CustomerId)} is required.");
                }
                else
                {
                    var query = _context.Customers.AsQueryable();

                    if (state.Role != RoleEnum.SuperAdmin)
                    {
                        query = query
                            .Where(x => x.CustomerId == state.CustomerId);
                    }
                    else
                    {
                        query = query
                            .Where(x => x.CustomerId == model.CustomerId);
                    }

                    var exist = await query
                        .AnyAsync();

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(Customer)} not found.");
                    }
                }

                if (model.LicensePlateId is null)
                {
                    return Result.Fail($"{nameof(LicensePlateKnownAssignmentEditModel.LicensePlateId)} is required.");
                }
                else
                {
                    var exist = await _context.LicensePlates
                        .AnyAsync(x => x.LicensePlateId == model.LicensePlateId
                            && x.Palletized == false);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(LicensePlate)} not found.");
                    }
                }

                if (model.Products is null || model.Products.Count() == 0)
                {
                    return Result.Fail($"{nameof(model.Products)} is required.");
                }
                else
                {
                    if (model.Products.Any(x => x.ItemId is null))
                    {
                        return Result.Fail($"Product SKU is required.");
                    }
                    else
                    {
                        foreach (var x in model.Products)
                        {
                            var exist = await _context.Items
                                .AnyAsync(z => z.ItemId == x.ItemId
                                    && z.CustomerId == model.CustomerId);

                            if (!exist)
                            {
                                return Result.Fail($"{nameof(Item)} not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.VendorId is not null))
                    {
                        foreach (var x in model.Products.Where(z => z.VendorId is not null))
                        {
                            var exist = await _context.Vendors
                                .AnyAsync(z => z.VendorId == x.VendorId
                                    && z.CustomerId == model.CustomerId);

                            if (!exist)
                            {
                                return Result.Fail($"{nameof(Vendor)} not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.LotId is not null))
                    {
                        foreach (var x in model.Products.Where(z => z.LotId is not null))
                        {
                            var exist = await _context.Lots
                                .AnyAsync(z => z.LotId == x.LotId
                                    && z.ItemId == x.ItemId);

                            if (!exist)
                            {
                                return Result.Fail("Lot not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.TotalQty is null))
                    {
                        return Result.Fail($"Product Total Qty is required.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.LicensePlates
                            .Include(x => x.LicensePlateItems)
                            .SingleOrDefaultAsync(x => x.LicensePlateId == model.LicensePlateId);

                        foreach (var x in entity.LicensePlateItems) // edit
                        {
                            if (model.Products.Any(z => z.LicensePlateItemId == x.LicensePlateItemId))
                            {
                                var p = model.Products.SingleOrDefault(z => z.LicensePlateItemId == x.LicensePlateItemId);
                                if (p is not null)
                                {
                                    x.ItemId = p.ItemId;
                                    x.VendorId = p.VendorId;
                                    x.LotId = p.LotId;
                                    x.ReferenceNo = p.ReferenceNo;
                                    x.Cases = p.Cases;
                                    x.EaCase = p.EaCase;
                                    x.TotalQty = p.TotalQty.Value;
                                    x.Qty = p.TotalQty.Value;
                                    x.TotalWeight = p.TotalWeight;

                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                        foreach (var x in model.Products) // add
                        {
                            if (x.LicensePlateItemId is null)
                            {
                                var lpItem = new LicensePlateItem
                                {
                                    CustomerId = model.CustomerId,
                                    LicensePlateId = entity.LicensePlateId,
                                    ItemId = x.ItemId,
                                    VendorId = x.VendorId,
                                    LotId = x.LotId,
                                    ReferenceNo = x.ReferenceNo,
                                    Cases = x.Cases,
                                    EaCase = x.EaCase,
                                    TotalQty = x.TotalQty.Value,
                                    Qty = x.TotalQty.Value,
                                    TotalWeight = x.TotalWeight
                                };
                                _context.Add(lpItem);
                                await _context.SaveChangesAsync();
                            }
                        }

                        // remove lp's item in db which is not exist in model.Products
                        var toRemove = entity.LicensePlateItems.Select(x => x.LicensePlateItemId)
                            .Except(model.Products.Where(z => z.LicensePlateItemId is not null).Select(z => z.LicensePlateItemId.Value)).ToList();
                        foreach (var x in toRemove)
                        {
                            var lpItem = await _context.LicensePlateItems
                                .SingleOrDefaultAsync(z => z.LicensePlateItemId == x);

                            lpItem.Deleted = true;
                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();

                        return Result.Ok(new LicensePlateGetModel
                        {
                            LicensePlateId = entity.LicensePlateId,
                            CustomerId = entity.CustomerId.Value,
                            LicensePlateNo = entity.LicensePlateNo,
                            LicensePlateType = entity.LicensePlateType
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

        public async Task<Result<IEnumerable<LicensePlateHistoryGetModel>>> GetLicensePlateHistoryAsync(AppState state, int customerId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var query = _context.Customers.AsQueryable();

                if (state.Role != RoleEnum.SuperAdmin)
                {
                    query = query
                        .Where(x => x.CustomerId == state.CustomerId);
                }
                else
                {
                    query = query
                        .Where(x => x.CustomerId == customerId);
                }

                var exist = await query
                    .AnyAsync();

                if (!exist)
                {
                    return Result.Fail($"{nameof(Customer)} not found.");
                }

                var lp = await _context.LicensePlates
                    .Include(x => x.Pallet)
                        .ThenInclude(x => x.Bin)
                    .Include(x => x.User)
                        .ThenInclude(x => x.Role)
                    .Include(x => x.LicensePlateItems)
                        .ThenInclude(x => x.Item)
                    .AsNoTracking()
                    .Where(x => x.CustomerId == customerId)
                    .ToListAsync();

                var model = new List<LicensePlateHistoryGetModel>();
                foreach (var x in lp)
                {
                    var l = new LicensePlateHistoryGetModel();
                    l.LicensePlateId = x.LicensePlateId;
                    l.LicensePlateNo = x.LicensePlateNo;
                    l.Type = x.LicensePlateType.ToLabel();
                    l.Generated = x.CreatedDateTime.ToString("MM/dd/yyyy, h:mm tt");
                    l.Owner = $"{x.User.Name} ({x.User.Role.Name})"; ;

                    if (x.ArrivalDateTime is not null)
                    {
                        l.Arrival = x.ArrivalDateTime.Value.ToString("MM/dd/yyyy, h:mm tt");
                    }

                    if (x.Pallet is not null)
                    {
                        l.Status = "Assigned";

                        if (x.Pallet.Bin is not null)
                        {
                            l.Location = x.Pallet.Bin.Name;
                        }
                    }
                    else
                    {
                        l.Status = "Unassigned";
                    }

                    if (x.LicensePlateItems.Any())
                    {
                        if (x.LicensePlateItems.Count == 1)
                        {
                            l.SKU = x.LicensePlateItems.FirstOrDefault().Item.SKU;
                        }
                        else
                        {
                            l.SKU = x.LicensePlateItems.Count.ToString();
                        }
                    }

                    model.Add(l);
                }

                return Result.Ok(model.AsEnumerable());
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<LicensePlateLookupDeviceGetModel>>> GetLicensePlateLookupDeviceAsync(CustomerDeviceTokenAuthModel state, string searchText, bool barcodeSearch = false)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (barcodeSearch && !searchText.HasValue())
                {
                    return Result.Fail("License plate no is required.");
                }

                var query = _context.LicensePlates
                    .AsQueryable();

                if (searchText.HasValue())
                {
                    searchText = searchText.Trim().ToLower();

                    if (barcodeSearch)
                    {
                        query = query.Where(x => x.CustomerId == state.CustomerId
                            && x.Palletized == false
                            && x.LicensePlateNo.Trim() == searchText.Trim());
                    }
                    else
                    {
                        query = query.Where(x => x.CustomerId == state.CustomerId
                            && x.Palletized == false
                            && x.LicensePlateNo.Trim().ToLower().Contains(searchText));
                    }
                }
                else
                {
                    query = query.Where(x => x.CustomerId == state.CustomerId
                        && x.Palletized == false);
                }

                IEnumerable<LicensePlateLookupDeviceGetModel> model = await query
                    .AsNoTracking()
                    .Select(x => new LicensePlateLookupDeviceGetModel
                    {
                        LicensePlateId = x.LicensePlateId,
                        LicensePlateNo = x.LicensePlateNo,
                        LicensePlateType = x.LicensePlateType
                    })
                    .ToListAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlate)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LicensePlateKnownAssignmentDeviceGetModel>> GetLicensePlateKnownAssignmentDeviceAsync(CustomerDeviceTokenAuthModel state, int licensePlateId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var lp = await _context.LicensePlates
                .Include(x => x.LicensePlateItems)
                    .ThenInclude(x => x.Item)
                .Include(x => x.LicensePlateItems)
                    .ThenInclude(x => x.Vendor)
                .Include(x => x.LicensePlateItems)
                    .ThenInclude(x => x.Lot)
                .Where(x => x.CustomerId == state.CustomerId
                    && x.LicensePlateId == licensePlateId
                    && x.Palletized == false)
                .AsNoTracking()
                .SingleOrDefaultAsync();

                if (lp is null)
                {
                    return null;
                }

                var model = new LicensePlateKnownAssignmentDeviceGetModel();
                model.LicensePlateId = lp.LicensePlateId;
                model.ArrivalDateTime = lp.ArrivalDateTime;

                var products = new List<LicensePlateKnownAssignmentDeviceGetModel.Product>();
                foreach (var x in lp.LicensePlateItems)
                {
                    var p = new LicensePlateKnownAssignmentDeviceGetModel.Product();
                    p.LicensePlateItemId = x.LicensePlateItemId;
                    p.ItemId = x.Item.ItemId;
                    p.ItemSKU = x.Item.SKU;
                    p.ItemDescription = x.Item.Description;

                    if (x.Vendor is not null)
                    {
                        p.VendorId = x.Vendor.VendorId;
                        p.VendorName = x.Vendor.Name;
                    }

                    if (x.Lot is not null)
                    {
                        p.LotId = x.Lot.LotId;
                        p.LotNo = x.Lot.LotNo;
                    }

                    p.ReferenceNo = x.ReferenceNo;
                    p.Cases = x.Cases;
                    p.EaCase = x.EaCase;
                    p.TotalQty = x.TotalQty;
                    p.TotalWeight = x.TotalWeight;

                    products.Add(p);
                }
                model.Products = products;

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlate)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LicensePlateGetModel>> EditLicensePlateUnknownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateUnknownToPalletizedEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlateUnknownToPalletizedEditModel)} is required.");
                }

                if (model.LicensePlateId is null)
                {
                    return Result.Fail($"{nameof(LicensePlateUnknownToPalletizedEditModel.LicensePlateId)} is required.");
                }
                else
                {
                    var exist = await _context.LicensePlates
                        .AnyAsync(x => x.LicensePlateId == model.LicensePlateId
                            && x.LicensePlateType == LicensePlateTypeEnum.Unknown
                            && x.Palletized == false
                            && x.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(LicensePlate)} not found.");
                    }
                }

                if (model.Products is null || model.Products.Count() == 0)
                {
                    return Result.Fail($"{nameof(model.Products)} is required.");
                }
                else
                {
                    if (model.Products.Any(x => x.ItemId is null))
                    {
                        return Result.Fail($"Product SKU is required.");
                    }
                    else
                    {
                        foreach (var x in model.Products)
                        {
                            var exist = await _context.Items
                                .AnyAsync(z => z.ItemId == x.ItemId
                                    && z.CustomerId == state.CustomerId);

                            if (!exist)
                            {
                                return Result.Fail($"{nameof(Item)} not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.VendorId is not null))
                    {
                        foreach (var x in model.Products.Where(z => z.VendorId is not null))
                        {
                            var exist = await _context.Vendors
                                .AnyAsync(z => z.VendorId == x.VendorId
                                    && z.CustomerId == state.CustomerId);

                            if (!exist)
                            {
                                return Result.Fail($"{nameof(Vendor)} not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.LotId is not null))
                    {
                        foreach (var x in model.Products.Where(z => z.LotId is not null))
                        {
                            var exist = await _context.Lots
                                .AnyAsync(z => z.LotId == x.LotId
                                    && z.ItemId == x.ItemId);

                            if (!exist)
                            {
                                return Result.Fail("Lot not found.");
                            }
                        }
                    }

                    if (model.Products.Any(x => x.TotalQty is null))
                    {
                        return Result.Fail($"Product Total Qty is required.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.LicensePlates
                            .SingleOrDefaultAsync(x => x.LicensePlateId == model.LicensePlateId);
                        entity.Palletized = true;
                        entity.ArrivalDateTime = DateTime.Now;
                        await _context.SaveChangesAsync();

                        var qty = 0;

                        foreach (var x in model.Products)
                        {
                            var lpItem = new LicensePlateItem
                            {
                                CustomerId = state.CustomerId,
                                LicensePlateId = model.LicensePlateId,
                                ItemId = x.ItemId,
                                VendorId = x.VendorId,
                                LotId = x.LotId,
                                ReferenceNo = x.ReferenceNo,
                                Cases = x.Cases,
                                EaCase = x.EaCase,
                                TotalQty = x.TotalQty.Value,
                                Qty = x.TotalQty.Value,
                                TotalWeight = x.TotalWeight
                            };
                            _context.Add(lpItem);
                            await _context.SaveChangesAsync();

                            qty += lpItem.TotalQty;
                        }

                        var putAway = new PutAway
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            LicensePlateId = entity.LicensePlateId,
                            Qty = qty,
                            Remaining = qty,
                            PutAwayType = PutAwayTypeEnum.LicensePlate,
                            PutAwayDate = DateTime.Now
                        };
                        _context.Add(putAway);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new LicensePlateGetModel
                        {
                            LicensePlateId = entity.LicensePlateId,
                            CustomerId = entity.CustomerId.Value,
                            LicensePlateNo = entity.LicensePlateNo,
                            LicensePlateType = entity.LicensePlateType
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

        public async Task<Result<LicensePlateGetModel>> EditLicensePlateKnownToPalletizedDeviceAsync(CustomerDeviceTokenAuthModel state, LicensePlateKnownToPalletizedEditModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model is null)
                {
                    return Result.Fail($"{nameof(LicensePlateKnownToPalletizedEditModel)} is required.");
                }

                if (model.LicensePlateId is null)
                {
                    return Result.Fail($"{nameof(LicensePlateKnownToPalletizedEditModel.LicensePlateId)} is required.");
                }
                else
                {
                    var exist = await _context.LicensePlates
                        .AnyAsync(x => x.LicensePlateId == model.LicensePlateId
                            && x.LicensePlateType == LicensePlateTypeEnum.Known
                            && x.Palletized == false
                            && x.CustomerId == state.CustomerId);

                    if (!exist)
                    {
                        return Result.Fail($"{nameof(LicensePlate)} not found.");
                    }
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = await _context.LicensePlates
                            .SingleOrDefaultAsync(x => x.LicensePlateId == model.LicensePlateId);
                        entity.Palletized = true;
                        await _context.SaveChangesAsync();

                        var qty = await _context.LicensePlateItems
                            .Where(x => x.LicensePlateId == model.LicensePlateId)
                            .SumAsync(x => x.Qty);

                        var putAway = new PutAway
                        {
                            CustomerLocationId = state.CustomerLocationId,
                            LicensePlateId = entity.LicensePlateId,
                            Qty = qty,
                            Remaining = qty,
                            PutAwayType = PutAwayTypeEnum.LicensePlate,
                            PutAwayDate = DateTime.Now
                        };
                        _context.Add(putAway);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Result.Ok(new LicensePlateGetModel
                        {
                            LicensePlateId = entity.LicensePlateId,
                            CustomerId = entity.CustomerId.Value,
                            LicensePlateNo = entity.LicensePlateNo,
                            LicensePlateType = entity.LicensePlateType
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
    }
}