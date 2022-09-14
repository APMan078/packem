using System;

namespace Packem.Mobile.Common.Validations.Rules
{
    public class DateTimeRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            return DateTime.TryParse(value as string, out DateTime dt);
        }
    }
}