import { Request, Response, NextFunction } from "express";
import { admins } from "../../utils/config";

export default function checkAdmin(
  req: Request,
  res: Response,
  next: NextFunction
): void {
  if (!req.user || !req.user.username) {
    res
      .status(401)
      .json({
        status: "error",
        message: "you must be logged in to use admin APIs"
      })
      .end();
    return;
  }

  if (admins.indexOf(req.user.username) < 0) {
    res
      .status(403)
      .json({
        status: "error",
        message: "you're not an admin lol"
      })
      .end();
    return;
  }
  next();
}
