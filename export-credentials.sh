#!/bin/bash

export GOOGLE_APPLICATION_CREDENTIALS=$(cat credentials.json | base64)
echo $GOOGLE_APPLICATION_CREDENTIALS