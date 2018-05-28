FROM node:8

COPY webapp /app

ENV NODE_ENV PRODUCTION
ENV API_URL //123REPLACE-ME456

WORKDIR /app/packages/frontend
RUN npm install && \
  npm run build


FROM nginx:alpine
COPY --from=0 /app/packages/frontend/dist /usr/share/nginx/html
COPY startup.sh /

CMD ["/bin/sh", "startup.sh"]
