using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.Interfaces;
using Packem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public RoleService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }

        public async Task<Result<IEnumerable<RoleGetModel>>> GetRolesAsync()
        {
            try
            {
                IEnumerable<RoleGetModel> model =await _context.Roles
                    .AsNoTracking()
                    .Select(x => new RoleGetModel
                    {
                        RoleId = x.RoleId,
                        Name = x.Name
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
    }
}
