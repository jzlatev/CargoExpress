using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace CargoExpress.ModelBinders
{
    public class DateTimeModelBinder : IModelBinder
    {
        private readonly string customDateFormat;

        public DateTimeModelBinder(string _customDateFormat)
        {
            customDateFormat = _customDateFormat;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult != ValueProviderResult.None && !String.IsNullOrEmpty(valueResult.FirstValue))
            {
                DateTime date = DateTime.MinValue;
                bool isValidDate = false;
                string currentDateValue = valueResult.FirstValue;

                try
                {
                    date = DateTime.ParseExact(currentDateValue, customDateFormat, CultureInfo.InvariantCulture);
                    isValidDate = true;
                }
                catch (FormatException)
                {
                    try
                    {
                        date = DateTime.Parse(currentDateValue, new CultureInfo("bg-bg"));
                    }
                    catch (Exception e)
                    {
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, e, bindingContext.ModelMetadata);
                    }
                }
                catch (Exception e)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, e, bindingContext.ModelMetadata);
                }

                if (isValidDate)
                {
                    bindingContext.Result = ModelBindingResult.Success(date);
                }
            }

            return Task.CompletedTask;
        }
    }
}
