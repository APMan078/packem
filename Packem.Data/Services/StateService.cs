using FluentResults;
using Packem.Data.Interfaces;
using Packem.Domain.Common.Enums;
using Packem.Domain.Common.ExtensionMethods;
using Packem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Packem.Data.Services
{
    public class StateService : IStateService
    {
        public Result<IEnumerable<StateGetModel>> GetStates()
        {
            var model = new List<StateGetModel>();

            foreach (StateEnum x in Enum.GetValues(typeof(StateEnum)))
            {
                model.Add(new StateGetModel
                {
                    StateId = x.ToInt(),
                    Name = x.GetEnumDescription()
                });
            }

            return Result.Ok((IEnumerable<StateGetModel>)model);
        }

        public Result<StateGetModel> GetState(int stateId)
        {
            var states = new List<StateGetModel>();

            foreach (StateEnum x in Enum.GetValues(typeof(StateEnum)))
            {
                states.Add(new StateGetModel
                {
                    StateId = x.ToInt(),
                    Name = x.GetEnumDescription()
                });
            }

            var model = states.SingleOrDefault(x => x.StateId == stateId);

            if (model is null)
            {
                return Result.Fail("State not found.");
            }

            return Result.Ok(model);
        }

        public Result<IEnumerable<StatePostalGetModel>> GetStatePostals()
        {
            var model = new List<StatePostalGetModel>();

            foreach (StateEnum x in Enum.GetValues(typeof(StateEnum)))
            {
                model.Add(new StatePostalGetModel
                {
                    StateId = x.ToInt(),
                    Name = x.ToString()
                });
            }

            return Result.Ok((IEnumerable<StatePostalGetModel>)model);
        }

        public Result<StatePostalGetModel> GetStatePostal(int stateId)
        {
            var states = new List<StatePostalGetModel>();

            foreach (StateEnum x in Enum.GetValues(typeof(StateEnum)))
            {
                states.Add(new StatePostalGetModel
                {
                    StateId = x.ToInt(),
                    Name = x.ToString()
                });
            }

            var model = states.SingleOrDefault(x => x.StateId == stateId);

            if (model is null)
            {
                return Result.Fail("State not found.");
            }

            return Result.Ok(model);
        }
    }
}
