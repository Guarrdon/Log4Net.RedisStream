# Log4Net.RedisStream


```xml
    <appender
        name="RedisStreamAppender"
        type="Log4Net.RedisStream.RedisStreamAppender, Log4Net.RedisStream">
        <!-- required parameters -->
        <RedisConnectionString value="localhost:6379" />
        <RedisStreamName value="App1LoggingStream" />
        <!-- optional parameters -->
        <RedisStreamMessageField value="message" />
        <threshold value="ERROR" />
        <!-- layout -->
        <!-- optionally use Log4Net.RedisStream.JsonLayout for json serialized output -->
        <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%5level - %message%newline" />
        </layout>
  </appender>
  ```