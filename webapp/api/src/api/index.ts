import { Router } from "express";
import feed from "./feed";
import admin from "./admin";
import member from "./member";
import request from "request";
import { predictionApiUrl } from "../utils/config";

const r: Router = Router();

r.get("/feed", feed);
r.get("/feed/:username", feed);
r.use("/admin", admin);
r.use("/member", member);

if (predictionApiUrl) {
  r.post("/predict", (req, res) => {
    req.pipe(request.post(predictionApiUrl)).pipe(res);
  });
}

export default r;
