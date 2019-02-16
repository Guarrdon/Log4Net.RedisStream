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
        public virtual string RedisConnectionString { get; set; }
        public virtual string RedisStreamName { get; set; }
        public virtual string RedisStreamMessageField { get; set; } = "message";


        protected virtual IConnectionMultiplexer RedisConnection { get; set; }

        public virtual IConnectionMultiplexer ConnectToRedis()
        {
            if (string.IsNullOrEmpty(RedisConnectionString))
                throw new InvalidOperationException("Connection string required to connect to Redis");

             if (RedisConnection == null)
                RedisConnection = ConnectionMultiplexer.Connect(RedisConnectionString);

            return RedisConnection;
        }

        protected async override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                //check configurations
                if (string.IsNullOrEmpty(this.RedisConnectionString) || string.IsNullOrEmpty(this.RedisStreamName))
                    throw new InvalidOperationException("RedisConnectionString and RedisStreamName configuration elements are required.");

                //connect to Redis
                using (var connection = ConnectToRedis())
                {
                    //get Redis database
                    var db = connection.GetDatabase();

                    //convert raw loggingEvent to json
                    var logEventJson = this.RenderLoggingEvent(loggingEvent);

                    //add log message to stream
                    var messageId = await db.StreamAddAsync(this.RedisStreamName, this.RedisStreamMessageField, logEventJson, null, null, false, CommandFlags.None);
                    
                    //check for message failure
                    if (messageId == RedisValue.Null || ((string)messageId).Length == 0)
                        throw new RedisException("The message failed to log to a Redis Stream.  Return message was either null or empty.");
                }
            }
            catch (InvalidOperationException ex)
            {
                this.ErrorHandler.Error($"Logging configuration elements are not correctly set.", ex);
            }
            catch (RedisConnectionException ex)
            {
                this.ErrorHandler.Error($"Custom {this.GetType().Name} failed to connect to Redis.", ex);
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
