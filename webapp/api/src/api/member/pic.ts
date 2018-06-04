import { createBlobService } from "azure-storage";
import { Request, Response } from "express";
import { MongoClient } from "mongodb";
import nanoid from "nanoid";
import sharp from "sharp";
import {
  blobImagePrefix,
  blobUrl,
  dbCollection,
  dbName,
  dbUrl,
  dbUser,
  dbPass
} from "../../utils/config";
import logger from "../../utils/logger";

const putBlobIntoStorage = (
  file
): Promise<{ photoUrl?: string; error?: string }> => {
  return new Promise((resolve, reject) => {
    let ext;
    if (file.mimetype === "image/jpeg" || file.mimetype === "image/jpg") {
      ext = ".jpg";
    } else if (file.mimetype === "image/png") {
      ext = ".png";
    } else if (file.mimetype === "image/gif") {
      ext = ".gif";
    } else {
      reject({
        error: "improper mime type; only allows jpg, jpeg, png, or gif"
      });
      return;
    }

    const blobService = createBlobService(blobUrl);
    const fileName = `${nanoid()}${ext}`;
    blobService.createBlockBlobFromText(
      "pics",
      fileName,
      file.data,
      {
        contentSettings: {
          contentType: file.mimetype,
          contentEncoding: file.encoding
        }
      },
      function blobUploadCB(error, result, response) {
        if (error) {
          logger.error(`failed image upload ${file.name}`);
          reject(error);
        }
        const photoUrl = `${blobImagePrefix}${fileName}`;
        logger.debug(
          `successfully uploaded to blob storage, from: ${
            file.name
          }, to: ${photoUrl}`
        );
        logger.silly("blob response", result, response);
        resolve({ photoUrl });
      }
    );
  });
};

export default function postPicApi(req: Request, res: Response): void {
  if (!req.user || !req.user.username) {
    res
      .status(401)
      .json({ status: "error", message: "you must authenticate first" });
    return;
  }
  const displayName = req.user.username;
  let resUrl;

  if (!req.files || !req.files.pic) {
    res.status(401).json({
      status: "error",
      message: "you need to upload an image with the `pic` key"
    });
    return;
  }
  const pic: any = req.files.pic;
  const caption = req.body.caption || "";

  if (pic.truncated) {
    res.status(413).json({
      status: "error",
      message: "size of image is too large"
    });
    return;
  }
  console.log("user", pic);

  let client: MongoClient;
  sharp(pic.data)
    .resize(1024, 1024)
    .max()
    .withoutEnlargement()
    .jpeg()
    .toBuffer({ resolveWithObject: true })
    .then(img => {
      img.mimetype = "image/jpeg";
      console.log("img", img);
      return Promise.all([
        MongoClient.connect(dbUrl, {
          auth: { user: dbUser, password: dbPass }
        }),
        putBlobIntoStorage(img)
      ]);
    })
    .then(([resClient, { photoUrl }]) => {
      resUrl = photoUrl;
      client = resClient;

      const db = client.db(dbName);

      logger.info(`image upload from ${displayName}`);

      logger.silly(JSON.stringify(req.files, null, 2));

      const comments = caption
        ? [{ date: new Date(), text: caption, displayName, userId: "" }]
        : [];

      return db.collection(dbCollection).insertOne({
        photoUrl,
        thumbnailUrl: "",
        comments,
        upVotes: 0,
        downVotes: 0,
        uploadDate: new Date(),
        tags: [],
        displayName,
        userId: ""
      });
    })
    .then(() => {
      client.close();
      res.json({ status: "ok", url: resUrl });
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
