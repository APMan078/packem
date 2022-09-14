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
    public class LotService : ILotService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public LotService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        private async Task<Result<LotGetModel>> CreateLot(LotCreateModel model)
        {
            if (model is null)
            {
                return Result.Fail($"{nameof(LotCreateModel)} is required.");
            }

            if (model.CustomerLocationId is null)
            {
                return Result.Fail($"{nameof(LotCreateModel.CustomerLocationId)} is required.");
            }
            else
            {
                var exist = await _context.CustomerLocations
                    .AnyAsync(x => x.CustomerLocationId == model.CustomerLocationId
                        && x.CustomerId == model.CustomerId);

                if (!exist)
                {
                    return Result.Fail($"{nameof(CustomerLocation)} not found.");
                }
            }

            if (model.ItemId is null)
            {
                return Result.Fail($"{nameof(LotCreateModel.ItemId)} is required.");
            }
            else
            {
                var exist = await _context.Items
                    .AnyAsync(x => x.ItemId == model.ItemId
                        && x.CustomerId == model.CustomerId);

                if (!exist)
                {
                    return Result.Fail($"{nameof(Item)} not found.");
                }
            }

            if (!model.LotNo.HasValue())
            {
                return Result.Fail($"{nameof(LotCreateModel.LotNo)} is required.");
            }
            else
            {
                var exist = await _context.Lots
                    .AnyAsync(x => x.LotNo.Trim().ToLower() == model.LotNo.Trim().ToLower()
                        && x.ItemId == model.ItemId);

                if (exist)
                {
                    return Result.Fail($"{nameof(LotCreateModel.LotNo)} is already exist.");
                }
            }

            if (model.ExpirationDate is null)
            {
                return Result.Fail($"{nameof(LotCreateModel.ExpirationDate)} is required.");
            }

            var entity = new Lot
            {
                CustomerLocationId = model.CustomerLocationId,
                ItemId = model.ItemId,
                LotNo = model.LotNo,
                ExpirationDate = model.ExpirationDate.Value
            };

            _context.Add(entity);
            await _context.SaveChangesAsync();

            return Result.Ok(new LotGetModel
            {
                LotId = entity.LotId,
                CustomerLocationId = entity.CustomerLocationId.Value,
                ItemId = entity.ItemId.Value,
                LotNo = entity.LotNo,
                ExpirationDate = entity.ExpirationDate
            });
        }

        public async Task<Result<LotGetModel>> CreateLotAsync(AppState state, LotCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                if (model.CustomerId is null)
                {
                    return Result.Fail($"{nameof(LotCreateModel.CustomerId)} is required.");
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

                return await CreateLot(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        private async Task<IEnumerable<LotLookupGetModel>> GetLotLookupByItemId(int itemId, string searchText)
        {
            var query = _context.Lots
                .AsQueryable();

            if (searchText.HasValue())
            {
                searchText = searchText.Trim().ToLower();

                query = query.Where(x => x.ItemId == itemId
                    && x.LotNo.Trim().ToLower().Contains(searchText));
            }
            else
            {
                query = query.Where(x => x.ItemId == itemId);
            }

            return await query
                .AsNoTracking()
                .Select(x => new LotLookupGetModel
                {
                    LotId = x.LotId,
                    LotNo = x.LotNo,
                    ExpirationDate = x.ExpirationDate
                })
                .ToListAsync();
        }

        public async Task<Result<IEnumerable<LotLookupGetModel>>> GetLotLookupByItemIdAsync(AppState state, int itemId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                return Result.Ok(await GetLotLookupByItemId(itemId, searchText));
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<IEnumerable<LotLookupGetModel>>> GetLotLookupByItemIdDeviceAsync(CustomerDeviceTokenAuthModel state, int itemId, string searchText)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                return Result.Ok(await GetLotLookupByItemId(itemId, searchText));
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LotLookupGetModel>> GetLotByItemIdAndBinIdAsync(AppState state, int itemId, int binId)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                var model = await _context.InventoryBins
                    .Include(x => x.Inventory)
                    .Include(x => x.Lot)
                    .AsNoTracking()
                    .Where(x => x.Inventory.ItemId == itemId
                        && x.BinId == binId)
                    .Select(x => new LotLookupGetModel
                    {
                        LotId = x.Lot.LotId,
                        LotNo = x.Lot.LotNo,
                        ExpirationDate = x.Lot.ExpirationDate
                    })
                    .SingleOrDefaultAsync();

                if (model is null)
                {
                    return Result.Fail($"Lot not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

        public async Task<Result<LotGetModel>> CreateLotDeviceAsync(CustomerDeviceTokenAuthModel state, LotCreateModel model)
        {
            try
            {
                if (state is null)
                {
                    return Result.Fail($"{nameof(AppState)} is null.");
                }

                model.CustomerId = state.CustomerId;
                model.CustomerLocationId = state.CustomerLocationId;

                return await CreateLot(model);
            }
            catch (Exception ex)
            {
                ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }
    }
}