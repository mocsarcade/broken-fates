#!/usr/bin/env bash

set -e

docker run \
  -e UNITY_LICENSE_CONTENT \
  -e TEST_PLATFORM \
  -e UNITY_USERNAME \
  -e UNITY_PASSWORD \
  -w /project/ \
  -v $(pwd):/project/ \
  $IMAGE_NAME \
  /bin/bash -c "chmod +x /project/ci/before_script.sh && /project/ci/before_script.sh && chmod +x /project/ci/test.sh && /project/ci/test.sh"