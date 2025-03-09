FROM node:18 AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .

ARG API_BASE_URL
# ENV API_BASE_URL=${API_BASE_URL}
RUN echo "API_BASE_URL is: $API_BASE_URL"
# RUN sed -i "s|\"apiBaseUrl\": *\"[^\"]*\"|\"apiBaseUrl\": \"${API_BASE_URL}\"|g" src/assets/config.json
RUN sed -i "s|apiBaseUrl: 'API_BASE_URL_PLACEHOLDER'|apiBaseUrl: '$API_BASE_URL'|g" src/environments/environment.prod.ts

RUN echo "API_BASE_URL is: $API_BASE_URL"


RUN npm run build --configuration=production

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/dist/appointment-scheduler-web/browser ./
COPY nginx.conf /etc/nginx/nginx.conf


EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
