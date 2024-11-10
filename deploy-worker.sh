#!/bin/bash

echo "Deploying API"
LABEL=$(git rev-parse --short HEAD)
echo "Deploying image $APP:$LABEL to resource group $AZ_RESOURCE_GROUP and environment $AZ_CONTAINER_APP_ENV"

az containerapp create \
  --name $APP \
  --resource-group $AZ_RESOURCE_GROUP \
  --environment $AZ_CONTAINER_APP_ENV \
  --image $DOCKER_SERVER/$APP:$LABEL \
  --ingress external \
  --registry-server $DOCKER_SERVER \
  # --query properties.configuration.ingress.fqdn

  # Update the container ap with a new image
az containerapp update \
  --name $APP \
  --resource-group $AZ_RESOURCE_GROUP \
  --image $DOCKER_SERVER/$APP:$LABEL