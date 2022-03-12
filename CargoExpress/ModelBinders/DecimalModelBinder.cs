using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace CargoExpress.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult != ValueProviderResult.None && !String.IsNullOrEmpty(valueResult.FirstValue))
            {
                decimal value = 0;
                bool isDecimal = false;

                try
                {
                    string currentValue = valueResult.FirstValue;
                    currentValue = currentValue.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    currentValue = currentValue.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                    value = Convert.ToDecimal(currentValue, CultureInfo.CurrentCulture);
                    isDecimal = true;
                }
                catch (FormatException fe)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, fe, bindingContext.ModelMetadata);
                }

                if (isDecimal)
                {
                    bindingContext.Result = ModelBindingResult.Success(value);
                }
            }

            return Task.CompletedTask;
        }
    }
}
