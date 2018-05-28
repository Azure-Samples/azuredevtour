cd /usr/share/nginx/html
sed -i "s/123REPLACE-ME456/$API_URL/g" *
sed -i "s/process.env.APPINSIGHTS_INSTRUMENTATIONKEY/$APPINSIGHTS_INSTRUMENTATIONKEY/g" *
cd /
nginx -g 'daemon off;'
