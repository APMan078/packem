using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packem.Data.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserGetModel>> CreateUserAsync(AppState state, UserCreateModel model);
        Task<Result<UserGetModel>> EditUserAsync(AppState state, UserEditModel model);
        Task<Result<UserGetModel>> EditUserIsActiveAsync(AppState state, UserIsActiveEditModel model);
        Task<Result<UserGetModel>> EditUserPasswordAsync(AppState state, UserPasswordEditModel model);
        Task<Result<IEnumerable<UserGetModel>>> GetUsersAsync(AppState state);
        Task<Result<List<UserGetModel>>> GetUsersByCustomerIdAsync(AppState state, int customerId);
        Task<Result<UserGetModel>> GetUserAsync(AppState state, int userId);
        Task<Result<IEnumerable<UserVendorGetModel>>> GetUserVendorAsync(AppState state, int userId);
    }
}