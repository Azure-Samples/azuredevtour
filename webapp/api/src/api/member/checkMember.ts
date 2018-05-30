import { NextFunction, Request, Response } from "express";
import { MongoClient } from "mongodb";
import { dbName, dbUrl } from "../../utils/config";
import logger from "../../utils/logger";

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
        message: "you must be logged in to use member APIs"
      })
      .end();
    return;
  }
  const displayName = req.user.username;
  let client: MongoClient;
  logger.silly("start checkMember");
  MongoClient.connect(dbUrl)
    .then(resClient => {
      client = resClient;
      return client
        .db(dbName)
        .collection("banned")
        .count({ displayName });
    })
    .then(count => {
      logger.silly(`${displayName} has ${count} in the banned list`);
      if (count > 0) {
        res
          .status(403)
          .json({
            status: "error",
            message: "lol no"
          })
          .end();
        return;
      }

      if (req.user) {
        res.cookie("username", req.user.username, {
          maxAge: 900000
        });
      }
      next();
    });
}
