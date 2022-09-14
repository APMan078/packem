using FluentResults;
using Packem.Domain.Models;
using System.Collections.Generic;

namespace Packem.Data.Interfaces
{
    public interface IStateService
    {
        Result<IEnumerable<StateGetModel>> GetStates();
        Result<StateGetModel> GetState(int stateId);
        Result<IEnumerable<StatePostalGetModel>> GetStatePostals();
        Result<StatePostalGetModel> GetStatePostal(int stateId);
    }
}
