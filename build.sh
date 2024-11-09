#!/bin/sh

LABEL=$(git rev-parse --short HEAD)

echo "Building image with label: $LABEL"

LABEL=$LABEL make dockerize-all