import { Request, Response } from "express";
import { Db, MongoClient } from "mongodb";
import { dbCollection, dbName, dbUrl } from "../../utils/config";
import logger from "../../utils/logger";

export default function banAdminApi(req: Request, res: Response) {
  if (!req.body.username) {
    res
      .status(400)
      .json({
        status: "error",
        message: "you need a username to ban in the body"
      })
      .end();
  }

  const displayName = req.body.username.toLowerCase();
  const admin = req.user ? req.user.username : "unnamed admin";
  const reason = req.body.reason ? req.body.reason : "";
  logger.info(`${admin} has iniated a ban on ${displayName}`);
  let client: MongoClient;
  let db: Db;
  MongoClient.connect(dbUrl)
    .then(resClient => {
      client = resClient;
      db = client.db(dbName);

      return db
        .collection(dbCollection)
        .find({
          displayName: { $ne: displayName }
        })
        .toArray();
    })
    .then(pics => {
      const promises = pics
        .map(pic => ({
          ...pic,
          comments: pic.comments.filter(
            comment => comment.displayName !== displayName
          )
        }))
        .map(pic =>
          db
            .collection(dbCollection)
            .updateOne({ _id: pic._id }, { $set: { comments: pic.comments } })
        );
      const postsPromise = db
        .collection(dbCollection)
        .deleteMany({ displayName });
      db
        .collection("banned")
        .insertOne({ displayName, bannedBy: admin, reason }); // TODO TypeScript is having a hard time thinking this is a promise, right now this is fire and forget
      return Promise.all([postsPromise, ...promises]);
    })
    .then(([deleteOp, writeToBan, ...updateOps]) => {
      logger.info(`${displayName} successfully banned`);
      res
        .json({
          displayName,
          banned: true
        })
        .end();
      client.close();
    })
    .catch(error => {
      logger.error(error);
      res
        .status(500)
        .json({ message: "there was an error", status: 500 })
        .end();
      client.close();
    });
}
