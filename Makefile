# Generate default function that prints all the availble targets and also the help message
.DEFAULT_GOAL := help
.PHONY: help


help: ## Print all the available targets
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

build: ## Build the docker image
	dotnet build

dockerize-api: ## Build the docker image for the api, use with LABEL=<tag> to specify the tag
	DOCKER_DEFAULT_PLATFORM=linux/amd64 docker build . -t tomascontainers.azurecr.io/monostore-api:${LABEL} -f ./src/MonoStore.Api/Dockerfile

dockerize-cart: ## Build the docker image for cart, use with LABEL=<tag> to specify the tag
	DOCKER_DEFAULT_PLATFORM=linux/amd64 docker build . -t tomascontainers.azurecr.io/monostore-cart-host:${LABEL} -f ./src/cart/MonoStore.Cart.Host/Dockerfile

dockerize-product: ## Build the docker image for product, use with LABEL=<tag> to specify the tag
	DOCKER_DEFAULT_PLATFORM=linux/amd64 docker build . -t tomascontainers.azurecr.io/monostore-product-host:${LABEL} -f ./src/product/MonoStore.Product.Host/Dockerfile

dockerize-all: dockerize-api dockerize-cart dockerize-product ## Build all the docker images use with LABEL=<tag> to specify the tag

publish-api: ## Publish the api image to the registry, use with LABEL=<tag> to specify the tag
	docker push tomascontainers.azurecr.io/monostore-api:${LABEL}

publish-cart: ## Publish the cart image to the registry, use with LABEL=<tag> to specify the tag
	docker push tomascontainers.azurecr.io/monostore-cart-host:${LABEL}

publish-product: ## Publish the product image to the registry, use with LABEL=<tag> to specify the tag
	docker push tomascontainers.azurecr.io/monostore-product-host:${LABEL}

publish-all: publish-api publish-cart publish-product ## Publish all the images to the registry, use with LABEL=<tag> to specify the tag

deploy-api: ## Deploy the api to the azure container instance, use with LABEL=<tag> to specify the tag
	./deploy-api.sh

deploy-cart: ## Deploy the cart to the azure container instance, use with LABEL=<tag> to specify the tag
	WORKER_NAME=monostore-cart-host ./deploy-worker.sh

deploy-product: ## Deploy the product to the azure container instance, use with LABEL=<tag> to specify the tag
	WORKER_NAME=monostore-product-host ./deploy-worker.sh

deploy-all: deploy-api deploy-cart deploy-product ## Deploy all the images to the azure container instance, use with LABEL=<tag> to specify the tag

# az containerapp create \
#   --name $API_NAME \
#   --resource-group $RESOURCE_GROUP \
#   --environment $ENVIRONMENT \
#   --image $ACR_NAME.azurecr.io/$API_NAME \
#   --target-port 8080 \
#   --ingress external \
#   --registry-server $ACR_NAME.azurecr.io \
#   --query properties.configuration.ingress.fqdn