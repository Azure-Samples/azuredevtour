import dotenv from "dotenv";
import logger from "./utils/logger";
// dotenv needs to read from before keyvault attempted since keyvault
// keys could be in there
dotenv.config();

import getKeyVaultSecrets from "./utils/keyvault";

// read secrets from keyvault before starting main app
(async () => {
  logger.info(`NODE_ENV: ${process.env.NODE_ENV}`);
  const keyVaultSecrets = await getKeyVaultSecrets();
  process.env = Object.assign(process.env, keyVaultSecrets);
  require("./index");
})();
