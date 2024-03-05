## clone

clone with submodules

```shell
git clone --recursive https://github.com/jobf/fna-first
```

## prepare

make sure you have dotnet 7 sdk installed

```shell
dotnet --list-sdks
```

### IMPORTANT STEP !

make sure you have the prebuilt FNF binaries, see `deps/fna-binaries/readme.md` for instructions of where to get them and where to put them

## build

from the repo root

```shell
dotnet build
```

## run

```shell
./FnaFirst.Console/bin/Debug/net7.0/FnaFirst.Console 
```