#!/usr/bin/env bash
set -e
set -o pipefail
set -x

# Install dependencies
mozroots --import --sync --quiet
mono ./.nuget/NuGet.exe restore ./iCSharp.sln

# Build scriptcs
cd ./Engine

if [ "$1" == "brew" ]
then
	./build_brew.sh
else
	./build.sh
fi

cd ../

# Build iCSharp
mkdir -p build/Release/bin
xbuild ./iCSharp.sln /property:Configuration=Release /nologo /verbosity:normal
#yes | cp -f $(find ./*/bin/Release/*) ./build/Release/bin
for line in $(find ./*/bin/Release/*); do 
     echo "$line"
	 cp $line ./build/Release/bin
 #    ls -l "$line"
done
