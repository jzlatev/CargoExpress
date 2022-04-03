namespace CargoExpress.Core.Exceptions
{
    using System;

    public class FormException : Exception
    {
        public FormException(string inputName, string errorMessage)
        {
            InputName = inputName;
            ErrorMessage = errorMessage;
        }

        public string InputName { get; }

        public string ErrorMessage { get; }
    }
}
