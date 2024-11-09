#!/bin/bash

echo "Deploying Jaeger"
IMAGE_NAME="jaegertracing/all-in-one:1.6.0"
echo "Deploying image $IMAGE_NAME to resource group $AZ_RESOURCE_GROUP and environment $AZ_CONTAINER_APP_ENV"

APP_NAME="monostore-jaeger"

az containerapp create \
  --name $APP_NAME \
  --resource-group $AZ_RESOURCE_GROUP \
  --environment $AZ_CONTAINER_APP_ENV \
  --image $IMAGE_NAME \
  --target-port 16686 \
  --ingress external \
  --query properties.configuration.ingress.fqdn

az containerapp update \
  --name $APP_NAME \
  --resource-group $AZ_RESOURCE_GROUP \
  --set-env-vars "DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true"


# docker run -d --name jaeger \
#   -e COLLECTOR_ZIPKIN_HTTP_PORT=9411 \
#   -p 5775:5775/udp \
#   -p 6831:6831/udp \
#   -p 6832:6832/udp \
#   -p 5778:5778 \
#   -p 16686:16686 \
#   -p 14268:14268 \
#   -p 9411:9411 \
#   jaegertracing/all-in-one:1.6.0