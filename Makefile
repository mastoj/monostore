# Generate default function that prints all the availble targets and also the help message
.DEFAULT_GOAL := help
.PHONY: help

LABEL := $(shell git rev-parse --short HEAD)

help: ## Print all the available targets
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

build: ## Build the docker image
	dotnet build

dockerize-app: ## Build the docker image for the app, use with APP=<app> and LABEL=<tag> to specify the app and tag, and DOCKER_FILE=<file> to specify the docker file
	DOCKER_DEFAULT_PLATFORM=linux/amd64 docker build . -t tomascontainers.azurecr.io/${APP}:${LABEL} -f ${DOCKER_FILE}

dockerize-api: ## Build the docker image for the api, use with LABEL=<tag> to specify the tag
	APP=monostore-api DOCKER_FILE=./src/MonoStore.Api/Dockerfile make dockerize-app
#	docker build . -t tomascontainers.azurecr.io/monostore-api:${LABEL} -f ./src/MonoStore.Api/Dockerfile

dockerize-cart: ## Build the docker image for cart, use with LABEL=<tag> to specify the tag
	APP=monostore-cart-host DOCKER_FILE=./src/cart/MonoStore.Cart.Host/Dockerfile make dockerize-app
#	docker build . -t tomascontainers.azurecr.io/monostore-cart-host:${LABEL} -f ./src/cart/MonoStore.Cart.Host/Dockerfile

dockerize-product: ## Build the docker image for product, use with LABEL=<tag> to specify the tag
	APP=monostore-product-host DOCKER_FILE=./src/product/MonoStore.Product.Host/Dockerfile make dockerize-app
# docker build . -t tomascontainers.azurecr.io/monostore-product-host:${LABEL} -f ./src/product/MonoStore.Product.Host/Dockerfile

dockerize-all: dockerize-api dockerize-cart dockerize-product ## Build all the docker images use with LABEL=<tag> to specify the tag

publish-app: ## Publish an app to the registry, use with APP=<app> and LABEL=<tag> to specify the app and tag
	docker push tomascontainers.azurecr.io/${APP}:${LABEL}

publish-api: ## Publish the api image to the registry, use with LABEL=<tag> to specify the tag
	APP=monostore-api make publish-app

publish-cart: ## Publish the cart image to the registry, use with LABEL=<tag> to specify the tag
	APP=monostore-cart-host make publish-app

publish-product: ## Publish the product image to the registry, use with LABEL=<tag> to specify the tag
	APP=monostore-product-host make publish-app

publish-all: publish-api publish-cart publish-product ## Publish all the images to the registry, use with LABEL=<tag> to specify the tag

deploy-api: ## Deploy the api to the azure container instance
	./deploy-api.sh

deploy-worker: ## Deploys a worker to container apps
	./deploy-worker.sh

deploy-cart: ## Deploy the cart to the azure container instance
	APP=monostore-cart-host make deploy-worker

deploy-product: ## Deploy the product to the azure container instance
	APP=monostore-product-host make deploy-worker

deploy-all: deploy-api deploy-cart deploy-product ## Deploy all the images to the azure container instance

build-publish-deploy-all: ## Deploys and builds everything
	./build.sh && ./publish.sh && make deploy-all

build-publish-deploy-api: ## Deploys and builds the api
	./build-api.sh && ./publish-api.sh && make deploy-api

build-publish-deploy-app: ## Deploys a worker, use with APP=<name> to specify the worker name
	make dockerize-app && make publish-app && make deploy-worker

build-publish-deploy-cart: ## Deploys and builds the cart
	APP=monostore-cart-host make build-publish-deploy-app

build-publish-deploy-product: ## Deploys and builds the product
	APP=monostore-product-host DOCKER_FILE=./src/product/MonoStore.Product.Host/Dockerfile make build-publish-deploy-app

# az containerapp create \
#   --name $API_NAME \
#   --resource-group $RESOURCE_GROUP \
#   --environment $ENVIRONMENT \
#   --image $ACR_NAME.azurecr.io/$API_NAME \
#   --target-port 8080 \
#   --ingress external \
#   --registry-server $ACR_NAME.azurecr.io \
#   --query properties.configuration.ingress.fqdn