import { Router } from "express";
// import checkMember from "./checkMember";
import comment from "./comment";
import pic from "./pic";
import vote from "./vote";

const r: Router = Router();

// r.use(checkMember);
r.post("/pic", pic);
r.post("/comment", comment);
r.post("/vote", vote);

export default r;
