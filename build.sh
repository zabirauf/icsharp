#!/usr/bin/env bash
set -e
set -o pipefail
set -x

python ./kernel-spec/kernelWriter.py

# Install dependencies
mozroots --import --sync --quiet
mono ./.nuget/NuGet.exe restore ./iCSharp.sln

# Build iCSharp
mkdir -p build/bin/Release
xbuild ./iCSharp.sln /property:Configuration=Release /nologo /verbosity:normal

# Copy files safely
for line in $(find ./*/bin/Release/*); do 
	 cp $line ./build/bin/Release
done

jupyter kernelspec install kernel-spec --name=csharp --user