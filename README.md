# Log4Net.RedisStream

Custom Log4Net appender to push log messages to a Redis Stream.
Written for .Net Core applications 

```
<TargetFramework>netstandard2.0</TargetFramework>
```

## Getting Started

Redis 5.0 introduced [Redis Streams](https://redis.io/topics/streams-intro), an append only log like data structure.  This paradigm is a natural fit for application logging, so a common logging framework, [Log4Net](https://logging.apache.org/log4net/), was extended to through a custom appender to provide the functionality.

This readme will provide detail on the how the appender should be leveraged to communicate with Redis Streams.

### Prerequisites

For Microsoft .Net Core applications
An accessible Redis instance >= v5.0
To get started locally, a [Redis Docker container](https://hub.docker.com/_/redis) can be useful.    

### Installing

*Will be pushed to Nuget as an accessible public package*

Ensure that your Redis instance is available.  You can use the cli command to PING the Redis instance.  If successful, a PONG response is expected.

```
redis-cli -h localhost -p 6390 ping
```

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
            <RedisConnectionString value="localhost:6739" />
            <RedisStreamName value="App1LoggingStream" />
            <!-- optional parameters -->
            <RedisStreamMessageField value="message" />
            <threshold value="INFO" />
            <!-- layout -->
            <!-- optionally use Log4Net.RedisStream.JsonLayout for json serialized output -->
            <layout type="log4net.Layout.PatternLayout">
                <conversionPattern value="%5level - %message%newline" />
            </layout>
            <!--<layout type="Log4Net.RedisStream.JsonLayout, Log4Net.RedisStream" />-->
    </appender>
</log4net>
```

The appender configuration leverages three primary settings.
1. RedisConnectionString - **Required** - Connection string to the Redis instance
1. RedisStreamName - **Required** - The Redis Stream where log entries for this appender will be written
1. RedisStreamMessageField - **Optional** - The name of the field that hosts the log message data within the Redis Stream.  By default, it is *message*.

*It is important to note that in this configuration, all log messages up to the **INFO** subtype are processed.  **DEBUG** logs are not processed.* 

If Log4Net is correctly configured, the standard logger should automatically forward to the Redis Stream instance.
You can test that the number of entries has incremented by using the following command from the Redis cli command:

```
xlen App1LoggingStream
```

Or review the values with the following command (note this brings back **ALL** of stream entries):

```
xrange App1LoggingStream - +
```

See the [Redis Streams documentation](https://redis.io/topics/streams-intro) for more information.


### Layout format

There is also a JsonLayout included in the Log4Net.RedisStream project. This can  

## Running the tests

The tests are [XUnit](https://xunit.github.io/) unit tests.
You can execute the tests, with code coverage analysis, by running:

```
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```


## Built With

* [VSCode](https://code.visualstudio.com/) - Integrated Development Environment
* [Redis](https://redis.io/) - In-memory and persistent data strustures
* [Log4Net](https://logging.apache.org/log4net/) - Base logging framework
* [XUnit](https://xunit.github.io/) - Unit test framework


## Tags

[Tags on this repository](https://github.com/guarrdon/log4netredisstream/tags). 

## Authors

* **@Guarrdon** - *Problem-solving, technology leader*

See also the list of [contributors](https://github.com/guarrdon/log4netredisstream/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details