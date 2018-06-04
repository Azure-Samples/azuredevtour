import winston from "winston";

const level = process.env.LOG_LEVEL || "debug";

winston.configure({
  level,
  transports: [
    new winston.transports.Console({
      colorize: true,
      timestamp() {
        return new Date().toISOString();
      }
    })
  ]
});

export default winston;
