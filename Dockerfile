FROM mcr.microsoft.com/dotnet/core/sdk:3.1  as build-env
WORKDIR /app

#install debugger for NET Core
COPY *.csproj ./
RUN dotnet restore

#Copy the project files and build our release
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY  --from=build-env /app/out .
ENTRYPOINT [ "dotnet","App.dll" ]

#command to build 
# docker build -t aspnetapp:3.1 .
# run command
# docker run -d -p 8080:80 --name myapp aspnetapp
# remove build images 
# docker image prune
# save Images
# docker save aspnetapp:3.1 > aspnetapp-3.1.tar