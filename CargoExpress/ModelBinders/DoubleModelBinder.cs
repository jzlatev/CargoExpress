using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace CargoExpress.ModelBinders
{
    public class DoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult != ValueProviderResult.None && !String.IsNullOrEmpty(valueResult.FirstValue))
            {
                double value = 0;
                bool isDouble = false;

                try
                {
                    string currentValue = valueResult.FirstValue;
                    currentValue = currentValue.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    currentValue = currentValue.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                    value = Convert.ToDouble(currentValue, CultureInfo.CurrentCulture);
                    isDouble = true;
                }
                catch (FormatException fe)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, fe, bindingContext.ModelMetadata);
                }

                if (isDouble)
                {
                    bindingContext.Result = ModelBindingResult.Success(value);
                }
            }

            return Task.CompletedTask;
        }
    }
}
