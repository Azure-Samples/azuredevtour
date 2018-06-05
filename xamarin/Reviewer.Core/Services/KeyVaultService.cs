using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using PhotoTour.Services;

namespace PhotoTour.Core
{
    public class KeyVaultService : IKeyVaultService
    {
        KeyVaultClient keyClient;

        public async Task<string> GetValueForKey(string keyName)
        {
            try
            {
                if (keyClient == null)
                {
                    keyClient = new KeyVaultClient(async (authority, resource, scope) =>
                    {
                        var adCredential = new ClientCredential(APIKeys.KeyVaultClientId, APIKeys.KeyVaultClientSecret);
                        var authContext = new AuthenticationContext(authority, null);

                        var authResult = await authContext.AcquireTokenAsync(resource, adCredential);
                        return authResult.AccessToken;
                    });
                }

                var secret = await keyClient.GetSecretAsync(APIKeys.KeyVaultUrl, keyName);

                return secret.Value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"***** {ex.Message}");

                return "";
            }
        }
    }
}
