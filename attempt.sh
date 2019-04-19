#!/usr/bin/env sh

set -x



export UNITY_EXECUTABLE=${UNITY_EXECUTABLE:-"\C:\Program Files\Unity\Editor"}

export BUILD_NAME=${BUILD_NAME:-"ExampleProjectName"}



BUILD_TARGET=StandaloneLinux64 ./ci/build.sh

BUILD_TARGET=StandaloneOSX ./ci/build.sh

BUILD_TARGET=StandaloneWindows64 ./ci/build.sh

BUILD_TARGET=WebGL ./ci/build.sh
