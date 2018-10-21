namespace BFT.AzureFuncApp.Core
{
    using System;

    public class SettingsProvider
    {
        public static string GetDatabaseEndpointUrl()
        {
            return GetSettings("DatabaseEndpointUrl");
        }

        public static string GetDatabaseAccountKey()
        {
            return GetSettings("DatabaseAccountKey");
        }

        public static string GetDatabaseId()
        {
            return GetSettings("DatabaseId");
        }

        private static string GetSettings(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"'{name}' app setting is required");
            }

            return value;
        }
    }
}