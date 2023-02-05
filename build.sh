#!/bin/sh
cd FlatNuGet
dotnet build -c Release -v Quiet --nologo
cd ..

