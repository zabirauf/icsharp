#!/usr/bin/env bash
set -e
set -o pipefail
set -x

# Install dependencies
mozroots --import --sync --quiet
mono ./.nuget/NuGet.exe restore ./iCSharp.sln

# Build scriptcs
cd ./Engine
./build.sh
cd ../

# Build iCSharp
mkdir -p build/Release/bin
xbuild ./iCSharp.sln /property:Configuration=Release /nologo /verbosity:normal
cp $(find ./*/bin/Release/*) ./build/Release/bin
