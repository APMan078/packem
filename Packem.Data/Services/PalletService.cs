using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Entities;
using Packem.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Packem.Data.Services
{
    public class PalletService : IPalletService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExceptionService _exceptionService;

        public PalletService(ApplicationDbContext context,
            IExceptionService exceptionService)
        {
            _context = context;
            _exceptionService = exceptionService;
        }
    }
}