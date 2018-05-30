// tslint:disable-next-line:no-var-requires
const appInsights = require("applicationinsights");
import bodyParser from "body-parser";
import mongoStoreFn from "connect-mongo";
import cookieParser from "cookie-parser";
import cors from "cors";
import express from "express";
import fileUpload from "express-fileupload";
import session from "express-session";
import morgan from "morgan";
import passport from "passport";
import api from "./api";
import auth from "./auth";
import {
  appInsightsIntsrumentationKey,
  dbPass,
  dbUrl,
  dbUser,
  port,
  sessionSecret
} from "./utils/config";
import logger from "./utils/logger";

appInsights.setup(appInsightsIntsrumentationKey);
appInsights.start();
const app = express();

const MongoStore = mongoStoreFn(session);

app.use(cors());

const root = `${process.cwd()}`;

app.use("/", express.static(`${root}/dist`));

app.use(
  session({
    secret: sessionSecret,
    resave: true,
    saveUninitialized: true,
    store: new MongoStore({
      url: dbUrl,
      mongoOptions: {
        auth: { user: dbUser, password: dbPass }
      }
    })
  })
);

app.use(
  fileUpload({ abortOnLimit: true, limits: { fileSize: 2 * 1024 * 1024 } })
);
app.use(cookieParser());
app.use(bodyParser.urlencoded({ extended: true }));

app.use(passport.initialize());
app.use(passport.session());

app.use(
  morgan("dev", {
    skip(req, res) {
      return res.statusCode < 400;
    },
    stream: process.stderr
  })
);

app.use(
  morgan("dev", {
    skip(req, res) {
      return res.statusCode >= 400;
    },
    stream: process.stdout
  })
);

app.use("/api", api);
app.use("/auth", auth);

if (process.env.NODE_ENV === "development") {
  import("parcel-bundler").then(pkg => {
    const Bundler = pkg.default;
    const bundler = new Bundler("../frontend/src/index.html");
    app.use(bundler.middleware());
  });
} else {
  app.use("/", express.static("./static"));
}

app.listen(port, () => logger.info(`listening on port ${port}`));
