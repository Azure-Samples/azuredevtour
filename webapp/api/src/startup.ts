import dotenv from "dotenv";
// dotenv needs to read from before keyvault attempted since keyvault
// keys could be in there
dotenv.config();

import getKeyVaultSecrets from "./utils/keyvault";

// read secrets from keyvault before starting main app
(async () => {
  const keyVaultSecrets = await getKeyVaultSecrets();
  process.env = Object.assign(process.env, keyVaultSecrets);
  require("./index");
})();
