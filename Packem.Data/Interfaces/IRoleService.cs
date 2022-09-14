using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IRoleService
    {
        Task<Result<IEnumerable<RoleGetModel>>> GetRolesAsync();
    }
}
