# Log4Net.RedisStream

[![Build Status](https://dev.azure.com/mattlyons/mattlyons/_apis/build/status/Guarrdon.Log4Net.RedisStream?branchName=master)](https://dev.azure.com/mattlyons/mattlyons/_build/latest?definitionId=1&branchName=master)

Custom Log4Net appender to push log messages to a Redis Stream.
Written for .Net Core applications 

```
<TargetFramework>netstandard2.0</TargetFramework>
```

## Getting Started

Redis 5.0 introduced [Redis Streams](https://redis.io/topics/streams-intro), an append only log like data structure.  This paradigm is a natural fit for application logging, so a common logging framework, [Log4Net](https://logging.apache.org/log4net/), was extended to through a custom appender to provide the functionality.

This readme will provide detail on the how the appender should be leveraged to communicate with Redis Streams.

Available on [Nuget](https://www.nuget.org/packages/Log4Net.RedisStream/).

### Prerequisites

For Microsoft .Net Core applications
An accessible Redis instance >= v5.0.

Docker is not required to run the example application, however a Dockerfile and docker-compose.yml file are included to maintain a consistent environment.

To run a remote instance of Redis for tests, review the [Redis Docker container](https://hub.docker.com/_/redis).    


## Configuration

The following Log4Net details must be incorporated to provide the necessary configuration.

```xml
<log4net>
    <logger name="DefaultLogger">
        <level value="ALL" />    
        <appender-ref ref="RedisStreamAppender" />
    </logger>
    
    <appender
            name="RedisStreamAppender"
            type="Log4Net.RedisStream.RedisStreamAppender, Log4Net.RedisStream">
            <!-- required parameters -->
            <RedisConnectionString value="redis:6379" />
            <RedisStreamName value="ExampleAppStream" />
            <!-- optional parameters -->
            <RedisStreamMessageField value="message" />
            <threshold value="INFO" />
            <!-- layout -->
            <layout type="log4net.Layout.PatternLayout">
                <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
            </layout>
    </appender>
</log4net>
```

The appender configuration leverages three primary settings.
1. RedisConnectionString - **Required** - Connection string to the Redis instance
1. RedisStreamName - **Required** - The Redis Stream where log entries for this appender will be written
1. RedisStreamMessageField - **Optional** - The name of the field that hosts the log message data within the Redis Stream.  By default, it is *message*.

*It is important to note that in this configuration, all log messages up to the **INFO** subtype are processed.  **DEBUG** logs are not processed.* 


## Execution

Docker configuration files are located at the root of the solution.

The Dockerfile creates a Docker log4net-redisstream-example image locally.  Use the following command to build the image.

```
docker build -t log4net-redisstream-example .
```

Utilize the Docker Compose command to initialize the environment with a Redis instance along side the .NetCore example application.  Configurations for Docker Compose are found in the docker-compose.yml.  The configuration requires the previously built log4net-redisstream-example image.

```
docker-compose up
```

After some initial Redis startup logging, the following should be seen in the Docker console.

*console_1  | Log4Net configuration loaded.*
*console_1  | Default logger created.*
*console_1  | Log attempted.*
*console_1  | Redis Stream log example finished.*
*log4netredisstream_console_1 exited with code 0*

If Log4Net is correctly configured, the standard logger should automatically forward to the Redis Stream instance.
You can test that the number of entries has incremented by using the following command from the Redis cli command:

```
xlen ExampleAppStream
```
*Note the stream name comes from the configuration*

Or review the values with the following command (note this brings back **ALL** of stream entries):

```
xrange ExampleAppStream - +
```

See the [Redis Streams documentation](https://redis.io/topics/streams-intro) for more information.

To ensure that your Redis instance is available, you can use the cli command to PING the instance.  If successful, a PONG response is expected.


### Clean up

To clean up the resultant Docker images, containers, and network, use the following command: 

```
docker-compose down
```


## Running the tests

The tests are [XUnit](https://xunit.github.io/) unit tests.
You can execute the tests, with code coverage analysis, by running:

```
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info /p:Exclude=[xunit.*]*
```


## Built With

* [VSCode](https://code.visualstudio.com/) - Integrated Development Environment
* [Redis](https://redis.io/) - In-memory and persistent data strustures
* [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) - Redis Client
* [Log4Net](https://logging.apache.org/log4net/) - Base logging framework
* [XUnit](https://xunit.github.io/) - Unit test framework
* [Docker Compose](https://docs.docker.com/compose/) - Containerization and Orchestation

## Tags

[Tags on this repository](https://github.com/guarrdon/log4netredisstream/tags). 

## Authors

* **@Guarrdon** - *Problem-solving, technology leader*

See also the list of [contributors](https://github.com/guarrdon/log4netredisstream/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.