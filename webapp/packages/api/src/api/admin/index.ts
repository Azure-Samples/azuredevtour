import { Router } from "express";
import ban from "./ban";
import checkAdmin from "./checkAdmin";

const r: Router = Router();

r.use(checkAdmin);
r.post("/ban", ban);

export default r;
