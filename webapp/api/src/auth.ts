import { Router } from "express";
import passport from "passport";
import { Strategy } from "passport-twitter";
import { twitterKey, twitterSecret } from "./utils/config";
import { Request, Response } from "express";

passport.use(
  new Strategy(
    {
      consumerKey: twitterKey,
      consumerSecret: twitterSecret,
      callbackURL: `${process.env.API_URL}/auth/callback`
    },
    function passportAuthCb(token, tokenSecret, profile, done) {
      return done(null, { username: profile.username.toLowerCase() });
    }
  )
);

passport.serializeUser(function passportSerialize(user, cb) {
  cb(null, user);
});

passport.deserializeUser(function passportDeserialize(obj, cb) {
  cb(null, obj);
});

const r: Router = Router();

r.get("/twitter", passport.authenticate("twitter"));

r.get(
  "/callback",
  passport.authenticate("twitter", { failureRedirect: "/login" }),
  (req: Request, res: Response) => {
    if (req.user) {
      res.cookie("username", req.user.username, {
        maxAge: 900000
      });
    }
    res.redirect("/");
  }
);

export default r;
