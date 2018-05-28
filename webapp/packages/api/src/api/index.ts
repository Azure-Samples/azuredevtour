import { Router } from "express";
import feed from "./feed";
import admin from "./admin";
import member from "./member";

const r: Router = Router();

r.get("/feed", feed);
r.get("/feed/:username", feed);
r.use("/admin", admin);
r.use("/member", member);

export default r;
