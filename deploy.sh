#!/bin/bash

VERSION=$(cat ./VERSION)

if [ -z $TRAVIS_TAG ]
then
	printf -v build %05d $TRAVIS_BUILD_NUMBER
	VERSION=$VERSION-$build
fi

mkdir "./artifacts"

dotnet pack "./Discord.Addons.Interactive/Discord.Addons.Interactive.csproj" -c "Release" -o "$(pwd)/artifacts/" /p:Version=$VERSION

dotnet nuget push "./artifacts/*" -s $MYGET_SOURCE -k $MYGET_KEY