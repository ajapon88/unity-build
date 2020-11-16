#!/bin/bash -l

# create Unity_v***.alf 
IMAGE=gableroux/unity3d:2019.4.14f1
docker run -v "$(pwd)/output:/output" -it "${IMAGE}" bash -c "cd /output; /opt/Unity/Editor/Unity -quit -batchmode -nographics -logFile -createManualActivationFile"

echo ""
echo "upload alf https://license.unity3d.com/manual to get lisence file"
