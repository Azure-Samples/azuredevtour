// tslint:disable-next-line:no-var-requires
const KeyVault = require("azure-keyvault");
// tslint:disable-next-line:no-var-requires
const AuthenticationContext = require("adal-node").AuthenticationContext;
// tslint:disable-next-line:no-var-requires
const logger = require("../utils/logger").default;

const clientId = process.env.KEYVAULT_CLIENT_ID || "";
const clientSecret = process.env.KEYVAULT_CLIENT_SECRET || "";
const keyVaultUri = process.env.KEYVAULT_URI || "";

async function getKeyVaultSecrets() {
  if (!(clientId && clientSecret && keyVaultUri)) {
    return {};
  }

  // Authenticator - retrieves the access token
  function authenticator(challenge, callback) {
    // Create a new authentication context.
    const context = new AuthenticationContext(challenge.authorization);

    // Use the context to acquire an authentication token.
    return context.acquireTokenWithClientCredentials(
      challenge.resource,
      clientId,
      clientSecret,
      (err, tokenResponse) => {
        if (err) {
          throw err;
        }
        // Calculate the value to be set in the request's Authorization header and resume the call.
        const authorizationValue =
          tokenResponse.tokenType + " " + tokenResponse.accessToken;

        return callback(null, authorizationValue);
      }
    );
  }

  const credentials = new KeyVault.KeyVaultCredentials(authenticator);
  const client = new KeyVault.KeyVaultClient(credentials);

  const secrets = await client.getSecrets(keyVaultUri, { maxResults: 100 });
  const secretsPromises = secrets.map(async s => {
    const { name } = KeyVault.parseSecretIdentifier(s.id);
    const { value } = await client.getSecret(keyVaultUri, name, "");
    return { name, value };
  });
  const secretsData: any[] = await Promise.all(secretsPromises);
  const results = {};
  for (const s of secretsData) {
    const key = s.name.replace("-", "_");
    results[key] = s.value;
    logger.debug(`Read KeyVault secret: ${key}`);
  }
  return results;
}

export default getKeyVaultSecrets;
