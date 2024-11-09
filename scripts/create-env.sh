#!/bin/bash

echo "Create log analytics workspace"
az monitor log-analytics workspace create \
  --resource-group $AZ_RESOURCE_GROUP \
  --workspace-name monostore-log-analytics

echo "Getting workspace ID and key"
WORKSPACE_ID=$(az monitor log-analytics workspace show \
  --resource-group $AZ_RESOURCE_GROUP \
  --workspace-name monostore-log-analytics \
  --query customerId -o tsv)

WORKSPACE_KEY=$(az monitor log-analytics workspace get-shared-keys \
  --resource-group $AZ_RESOURCE_GROUP \
  --workspace-name monostore-log-analytics \
  --query primarySharedKey -o tsv)

echo "Creating environment"
az containerapp env create \
  --name $AZ_CONTAINER_APP_ENV \
  --resource-group $AZ_RESOURCE_GROUP \
  --location "North Europe" \
  --logs-workspace-id $WORKSPACE_ID \
  --logs-workspace-key $WORKSPACE_KEY


