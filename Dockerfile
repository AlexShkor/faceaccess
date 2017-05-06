FROM microsoft/aspnetcore:latest
WORKDIR /app
COPY bin/Release/netcoreapp1.1/publish .
ENTRYPOINT ["dotnet", "faceaccess.dll"]