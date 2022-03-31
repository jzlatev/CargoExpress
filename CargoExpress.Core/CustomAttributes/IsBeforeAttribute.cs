using System.ComponentModel.DataAnnotations;

namespace CargoExpress.Core.CustomAttributes
{
    public class IsBeforeAttribute : ValidationAttribute
    {
        private readonly string date;

        public IsBeforeAttribute(string _date, string errorMessage = "")
        {
            this.date = _date;
            this.ErrorMessage = errorMessage;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Invalid date!");
            }

            try
            {
                DateTime dateToCompare = (DateTime)validationContext
                    .ObjectType
                    .GetProperty(date)
                    .GetValue(validationContext.ObjectInstance);
            
                if ((DateTime)value > dateToCompare)
                {
                    return ValidationResult.Success;
                }
            }
            catch (Exception)
            {}
            
            return new ValidationResult(ErrorMessage);
        }
    }
}
