# FlatNuGet

## Goal

The goal of this tool is to provide an easy way to manage libraries from NuGet when a project struct does not support NuGet such as a standard  Unity3D project.

## Usage

```
FlatNuGet <index-file> [options]
```

### Options
```
-o -op -operation	Do an operation
-v -ver -version	Show version information.
```
### Operations

#### init

This operation will init an index file.

##### Example
```
FlatNuGet ./foo.index -o init
```

#### update

This operation will try download and extract nuget packages and use cache if present.

##### Example
```
FlatNuGet ./foo.index -o update
```

#### forceupdate

This operation will try download and extract nuget packages and will not use cache even if present.

##### Example
```
FlatNuGet ./foo.index -o forceupdate
```
#### add [packageid/packageversion]

This operation will add a package to reference.

##### Example
```
FlatNuGet ./foo.index -o add LibCLCC.NET/1.0.0
```

#### remove [packageid]

This operation will remove a package from reference.

##### Example
```
FlatNuGet ./foo.index -o remove LibCLCC.NET
```

#### track [extension]

This operation will track a file extension to extract.

##### Example
```
FlatNuGet ./foo.index -o track .dll
```

#### filter [filter]

This operation will add a filter that filters files in the nuget package.

##### Example
```
FlatNuGet ./foo.index -o filter lib/nets*
```

## Licensing

The NuGet SDK is licensed under the Apache License 2.0.

This tool is licensed under the MIT License.