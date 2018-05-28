FROM node:8

COPY webapp /app

ENV NODE_ENV PRODUCTION

WORKDIR /app/packages/api
RUN npm install && \
  npm run build:server


FROM node:8-alpine

COPY webapp /app
WORKDIR /app/packages/api

RUN npm install --production && \
  apk update && \
  apk add ca-certificates && \
  rm -rf /var/cache/apk/*
COPY --from=0 /app/packages/api/dist /app/packages/api/dist

ENV PORT 80
EXPOSE 80

CMD ["npm", "start"]
