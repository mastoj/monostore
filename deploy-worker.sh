#!/bin/bash

echo "Deploying $APP"
LABEL=$(git rev-parse --short HEAD)
echo "Deploying image $DOCKER_SERVER/$APP:$LABEL to resource group $AZ_RESOURCE_GROUP and environment $AZ_CONTAINER_APP_ENV"

# az containerapp create \
#   --name $APP \
#   --resource-group $AZ_RESOURCE_GROUP \
#   --environment $AZ_CONTAINER_APP_ENV \
#   --image $DOCKER_SERVER/$APP:$LABEL \
#   --registry-server $DOCKER_SERVER \
#   --env-vars AZ_RESOURCE_GROUP=${AZ_RESOURCE_GROUP} \
#       AZ_CONTAINER_APP_ENV=${AZ_CONTAINER_APP_ENV} \
#       ConnectionStrings__clustering=${ConnectionStrings__clustering} \
#       ConnectionStrings__grainstate=${ConnectionStrings__grainstate} \
#       Orleans__ClusterId=${Orleans__ClusterId} \
#       Orleans__Clustering__ProviderType=${Orleans__Clustering__ProviderType} \
#       Orleans__Clustering__ServiceKey=${Orleans__Clustering__ServiceKey} \
#       Orleans__EnableDistributedTracing=${Orleans__EnableDistributedTracing} \
#       Orleans__Endpoints__GatewayPort=${Orleans__Endpoints__GatewayPort} \
#       Orleans__Endpoints__SiloPort=${Orleans__Endpoints__SiloPort} \
#       Orleans__GrainStorage__default__ProviderType=${Orleans__GrainStorage__default__ProviderType} \
#       Orleans__GrainStorage__default__ServiceKey=${Orleans__GrainStorage__default__ServiceKey} \
#       Orleans__ServiceId=${Orleans__ServiceId} \
#       COSMOS_CONNECTION_STRING=${COSMOS_CONNECTION_STRING} \
#   --query properties.configuration.ingress.fqdn

#  --ingress external \



#   # Update the container ap with a new image
az containerapp update \
  --name $APP \
  --resource-group $AZ_RESOURCE_GROUP \
  --image $DOCKER_SERVER/$APP:$LABEL 