import { MongoClient } from "mongodb";
import nanoid from "nanoid";
import { dbUrl } from "../src/utils/config";

const numberToInsert = 100;

const USERS = [
  "holtbt",
  "sarah_edo",
  "burkeholland",
  "clarkio",
  "simonacot",
  "jawache",
  "johnpapa",
  "sethjuarez"
];

const COMMENTS = [
  "This is amazing!",
  "🔥🔥🔥🔥🔥🔥",
  "👩‍🔬",
  "I love this",
  "Cute!",
  "Sweet picture",
  "Your mother was a 🐹 and your father smelt of elderberries",
  "lol"
];

const TAGS = [
  ["dog", "cool"],
  [],
  ["cat"],
  ["apple", "cool", "happy"],
  ["lol", "uncool"]
];

const getRandomNumber = n => Math.floor(Math.random() * n);
const getUsername = () => USERS[getRandomNumber(USERS.length)];
const getComment = () => COMMENTS[getRandomNumber(COMMENTS.length)];
const genTags = () => TAGS[getRandomNumber(TAGS.length)];
const getUrl = size =>
  `http://placecorgi.com/${size - getRandomNumber(10)}/${size -
    getRandomNumber(10)}?_id=${nanoid()}`; // nanoid is just so each url is unique

const genComments = () =>
  Array.from({ length: getRandomNumber(9) }, () => ({
    username: getUsername(),
    comment: getComment(),
    time: new Date().toISOString()
  }));

let client: MongoClient;
MongoClient.connect(dbUrl)
  .then(resClient => {
    client = resClient;
    const db = client.db("bitPic");

    return db.collection("pics").insertMany(
      Array.from({ length: numberToInsert }, () => ({
        photoUrl: getUrl(85),
        thumbnailUrl: getUrl(405),
        displayName: getUsername(),
        comments: genComments(),
        uploadDate: new Date(Date.now() - getRandomNumber(1000)),
        tags: genTags(),
        downVotes: getRandomNumber(10),
        upVotes: getRandomNumber(10)
      }))
    );
  })
  .then(() => {
    console.log("done");
    client.close();
  })
  .catch(err => console.error);
