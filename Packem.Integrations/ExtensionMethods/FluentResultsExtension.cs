using FluentResults;
using System.Collections.Generic;

namespace Packem.Integrations.ExtensionMethods
{
    public static class FluentResultsExtension
    {
        public static string ToErrorString(this List<IError> errors)
        {
            string error = null;
            IEnumerable<IError> errors2 = errors;

            foreach (var e in errors2)
                error = e.Message;

            return error;
        }
    }
}
