namespace Packem.Mobile.Common.Validations.Rules
{
    public class NonNegativeRule : IValidationRule<decimal>
    {
        public string ValidationMessage { get; set; }

        public bool Check(decimal value)
        {
            return value > 0;
        }
    }
}
