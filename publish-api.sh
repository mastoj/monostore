#!/bin/sh

LABEL=$(git rev-parse --short HEAD)

echo "Publishing image with label: $LABEL"

LABEL=$LABEL make publish-api