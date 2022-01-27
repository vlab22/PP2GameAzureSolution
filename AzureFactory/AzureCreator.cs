using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace AzureFactory
{
    public class AzureCreator
    {
        static readonly string ClientId = "ba08aea9-6554-409b-8179-0a59f6826b83";
        static readonly string ClientSecret = "RFF7Q~1o_uFL8KreRbe.Y5FyiPiFvTygSe5lQ";
        static readonly string TenantId = "4c7186cb-b33d-457c-8af8-2666547ae506";

        /// <summary>
        /// Returns an authenticated Azure context using the credentials in the
        /// specified auth file.
        /// </summary>
        /// <returns>Authenticated IAzure context.</returns>
        public static IAzure GetAzureContext()
        {
            IAzure azure;
            ISubscription sub;

            try
            {
                AzureCredentials Credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(ClientId, ClientSecret, TenantId, AzureEnvironment.AzureGlobalCloud);

                azure = Azure.Configure().WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic).Authenticate(Credentials).WithDefaultSubscription();

                sub = azure.GetCurrentSubscription();

                Console.WriteLine($"Authenticated with subscription '{sub.DisplayName}' (ID: {sub.SubscriptionId})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to authenticate:\n{ex.Message}");

                throw;
            }

            return azure;
        }
    }
}
