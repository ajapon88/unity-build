#!/bin/bash -l
# upload alf https://license.unity3d.com/manual 

UNITY_VERSION=2018.4.13f1
IMAGE=gableroux/unity3d:${UNITY_VERSION}

docker run -v "$(pwd)/output:/output" -it "${IMAGE}" bash -c "cd /output; /opt/Unity/Editor/Unity -quit -batchmode -nographics -logFile -createManualActivationFile"
