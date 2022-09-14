using Packem.Data.Interfaces;
using Packem.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class ExceptionService : IExceptionService
    {
        private readonly ApplicationDbContext _context;

        public ExceptionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Exception> HandleExceptionAsync(Exception ex)
        {
            var _ex = GetException(ex);
            var entity = new ErrorLog()
            {
                Message = _ex.Message,
                StackTrace = ex.StackTrace,
                Date = DateTime.Now
            };

            _context.Add(entity);
            await _context.SaveChangesAsync();

            return new Exception("An error has occurred. Please try again or contact support.");
        }

        public Exception GetException(Exception ex)
        {
            Exception _ex = ex;

            if (ex.InnerException != null)
            {
                _ex = GetException(ex.InnerException);
            }

            return _ex;
        }

        public async Task LogMessageAsync(string message, string stack)
        {
            var entity = new ErrorLog()
            {
                Message = message,
                StackTrace = stack,
                Date = DateTime.Now
            };

            _context.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
