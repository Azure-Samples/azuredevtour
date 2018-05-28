const {
  TWITTER_SECRET,
  TWITTER_KEY,
  LOG_LEVEL,
  DB_COLLECTION,
  DB_NAME,
  DB_URL,
  DB_USER,
  DB_PASS,
  PORT,
  SESSION_SECRET,
  BLOB_URL,
  SESSION_URL,
  BLOB_IMAGE_PREFIX,
  ADMINS,
  APPINSIGHTS_INSTRUMENTATIONKEY,
  IMAGE_ANALYSIS_BASE_URL
} = process.env;

export const twitterSecret: string = TWITTER_SECRET || "";
export const twitterKey: string = TWITTER_KEY || "";
export const logLevel: string = LOG_LEVEL || "debug";
export const dbCollection: string = DB_COLLECTION || "Photos";
export const dbName: string = DB_NAME || "PhotoTour";
export const dbUrl: string = DB_URL || "";
export const dbUser: string = DB_USER || "photos-tour";
export const dbPass: string = DB_PASS || "";
export const port: number = PORT ? +PORT : 3000;
export const sessionSecret: string =
  SESSION_SECRET || "developers developers developers";
export const blobUrl: string =
  BLOB_URL || "https://photostour.blob.core.windows.net/";
export const sessionUrl: string = SESSION_URL || "";
export const appInsightsIntrumentationKey: string =
  APPINSIGHTS_INSTRUMENTATIONKEY || "";
export const blobImagePrefix: string = BLOB_IMAGE_PREFIX || "photostour/";
export const admins: string[] = ADMINS ? ADMINS.split(",") : [];
export const imageAnalysisBaseUrl: string = IMAGE_ANALYSIS_BASE_URL || "";
