import { Request, Response } from "express";
import { Db, MongoClient } from "mongodb";
import {
  dbCollection,
  dbName,
  dbPass,
  dbUrl,
  dbUser
} from "../../utils/config";
import logger from "../../utils/logger";

export default function castVoteApi(req: Request, res: Response): void {
  if (!req.user || !req.user.username) {
    res
      .status(401)
      .json({ status: "error", message: "you must authenticate first" });
    return;
  }
  console.log(req.body);
  const validationResult = validateParameters(req.body);
  if (!validationResult.valid) {
    res.status(400).json({
      status: "error",
      message: validationResult.message
    });
    return;
  }

  const displayName = req.user.username;
  const { upVote, downVote, photoUrl } = req.body;
  let client: MongoClient;
  let db: Db;
  let newPost;

  MongoClient.connect(dbUrl, { auth: { user: dbUser, password: dbPass } })
    .then(resClient => {
      client = resClient;

      db = client.db(dbName);
      const voteType = upVote ? "up vote" : "down vote";
      logger.info(`${displayName} casted one ${voteType} on ${photoUrl}"`);

      return db.collection(dbCollection).findOne({ photoUrl });
    })
    .then(pic => {
      newPost = {
        ...pic,
        upVotes: upVote ? pic.upVotes + 1 : pic.upVotes,
        downVotes: downVote ? pic.downVotes + 1 : pic.downVotes,
        votes: upVote || downVote ? pic.votes + 1 : pic.votes
      };
      return db.collection(dbCollection).replaceOne({ photoUrl }, newPost);
    })
    .then(() => {
      client.close();
      res.json(newPost).end();
    })
    .catch(err => {
      res
        .status(500)
        .json({ status: "error", message: "uh, something went wrong" })
        .end();
      logger.error(err);
      client.close();
    });
}

function validateParameters(body) {
  let validationResult = {
    valid: true,
    message: "Values received are valid"
  };

  if ((!body.upVote && !body.downVote) || !body.photoUrl) {
    validationResult = {
      valid: false,
      message:
        "You are missing values. Please make sure to provide either an upVote or downVote and a photoUrl"
    };
  } else if (body.upVote && body.downVote) {
    validationResult = {
      valid: false,
      message:
        "You attempted to send both an upVote and downVote. Please choose only one action."
    };
  }

  return validationResult;
}
