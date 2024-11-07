# Generate default function that prints all the availble targets and also the help message
.DEFAULT_GOAL := help
.PHONY: help


help: ## Print all the available targets
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

build: ## Build the docker image
	dotnet build

dockerize-api: ## Build the docker image for the api, use with LABEL=<tag> to specify the tag
	docker build . -t monostore-api:${LABEL} -f ./src/product/MonoStore.Product.Host/Dockerfile

dockerize-cart: ## Build the docker image for cart, use with LABEL=<tag> to specify the tag
	docker build . -t monostore-cart-host:${LABEL} -f ./src/cart/MonoStore.Cart.Host/Dockerfile

dockerize-product: ## Build the docker image for product, use with LABEL=<tag> to specify the tag
	docker build . -t monostore-product-host:${LABEL} -f ./src/product/MonoStore.Product.Host/Dockerfile

dockerize-all: dockerize-api dockerize-cart dockerize-product ## Build all the docker images use with LABEL=<tag> to specify the tag