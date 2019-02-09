using System;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;
using StackExchange.Redis;


namespace Log4Net.RedisStream
{
    public class RedisStreamAppender : AppenderSkeleton
    {

        //todo: enforce secure connection?
        public string RedisConnectionString { get; set; }
        public string RedisStreamName { get; set; }
        public string RedisStreamMessageField { get; set; } = "message";

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                //connect to Redis
                using (var connection = ConnectionMultiplexer.Connect(this.RedisConnectionString))
                {
                    //get Redis database
                    var db = connection.GetDatabase();
                    
                    //convert raw loggingEvent to json
                    //JsonLayout allowed
                    var logEventJson = this.RenderLoggingEvent(loggingEvent);

                    //add log message to stream
                    var messageId = db.StreamAdd(this.RedisStreamName, this.RedisStreamMessageField, logEventJson);

                    //check for message failure
                    if (messageId == RedisValue.Null || ((string)messageId).Length == 0)
                        throw new RedisException("The message failed to log to a Redis Stream.  Return message was either null or empty.");
                }
            }
            catch (RedisException ex)
            {
                this.ErrorHandler.Error($"Custom {this.GetType().Name} failed to interact with Redis properly.", ex);
            }
            catch (Exception ex)
            {
                this.ErrorHandler.Error($"Custom {this.GetType().Name} has failed.", ex);
            }


        }
    }
}
