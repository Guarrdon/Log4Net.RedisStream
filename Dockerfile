FROM microsoft/dotnet:2.2-sdk
WORKDIR /code

# copy csproj and restore as distinct layers
COPY Log4Net.RedisStream/*.cs* ./Log4Net.RedisStream/
COPY Log4Net.RedisStream.Example/*.cs* ./Log4Net.RedisStream.Example/
RUN dotnet restore ./Log4Net.RedisStream.Example

ADD ./Log4Net.RedisStream.Example/*.config ./Log4Net.RedisStream.Example/

WORKDIR /code/Log4Net.RedisStream.Example/

RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "run"]