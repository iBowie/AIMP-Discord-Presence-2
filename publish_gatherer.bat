rmdir publish\AlbumCoverGatherer /q /s
del publish\AlbumCoverGatherer.exe

dotnet publish AlbumCoverGatherer\AlbumCoverGatherer.csproj -r win-x86 -p:PublishSingleFile=true -o publish\AlbumCoverGatherer

move publish\AlbumCoverGatherer\AlbumCoverGatherer.exe publish\AlbumCoverGatherer.exe

rmdir publish\AlbumCoverGatherer /q /s
