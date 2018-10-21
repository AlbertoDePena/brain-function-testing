namespace BFT.AzureFuncApp
{
    using System;

    public static class ExceptionExtensions
    {
        public static string ToDetails(this Exception e)
        {
            var message = e.Message;

            if (e.InnerException == null)
            {
                return message;
            }

            if (e.InnerException.Message == message)
            {
                return message;
            }

            var innerMessage = e.InnerException.ToDetails();

            return $"{message}{Environment.NewLine}{innerMessage}";
        }
    }
}