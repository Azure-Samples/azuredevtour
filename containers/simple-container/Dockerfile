FROM node:8

RUN mkdir /app
COPY webapp /app

WORKDIR /app/packages/frontend
RUN npm install

WORKDIR /app/packages/api
RUN npm install

ENV PORT 80
EXPOSE 80

CMD ["npm", "run", "dev"]
