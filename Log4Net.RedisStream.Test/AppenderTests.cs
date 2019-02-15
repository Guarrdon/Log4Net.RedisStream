using System;
using log4net.Core;
using log4net.Layout;
using Moq;
using StackExchange.Redis;
using Xunit;


namespace Log4Net.RedisStream.Test
{
    public class AppenderTests
    {

        [Fact]
        public void Appender_MessageFieldDefaultedIfNotSet()
        {
            var appender = new RedisStreamAppender();
            Assert.Equal("message", appender.RedisStreamMessageField);
        }
        [Fact]
        public void Appender_AutoPropertiesSetCorrectly()
        {
            var appender = new RedisStreamAppender { RedisConnectionString = "a", RedisStreamName = "b", RedisStreamMessageField = "c" };
            Assert.Equal("a", appender.RedisConnectionString);
            Assert.Equal("b", appender.RedisStreamName);
            Assert.Equal("c", appender.RedisStreamMessageField);
        }
        [Fact]
        public void RedisConnection_ConnectionMissing()
        {
            var appender = new RedisStreamAppender();
            Assert.Throws<InvalidOperationException>(() => appender.ConnectToRedis(""));

            appender.RedisConnectionString = "";
            Assert.Throws<InvalidOperationException>(() => appender.ConnectToRedis(""));
        }
        [Fact]
        public void Logging_ConfigurationElementsMissing()
        {
            var errorHandler = new Log4NetErrorHandler();
            var mockDatabase = BuildDatabase("SUCCESS_ID");
            var mockMultiplexer = BuildSuccessConnectionMultiplexer(mockDatabase);
            var mockAppender = BuildSuccessAppender(mockMultiplexer, errorHandler);
            mockAppender.SetupProperty(_ => _.RedisConnectionString, null);
            mockAppender.SetupProperty(_ => _.RedisStreamName, null);

            var loggingEvent = new LoggingEvent(typeof(AppenderTests), null, "LoggerName", Level.Info, "Example of a Redis Stream logging entry", null);

            mockAppender.Object.DoAppend(loggingEvent);

            Assert.NotNull(errorHandler.LogException);
            Assert.IsAssignableFrom<InvalidOperationException>(errorHandler.LogException);
            Assert.Equal("Logging configuration elements are not correctly set.", errorHandler.Message);
        }

        [Fact]
        public void SuccessfulLogger()
        {
            var errorHandler = new Log4NetErrorHandler();
            var mockDatabase = BuildDatabase("SUCCESS_ID");
            var mockMultiplexer = BuildSuccessConnectionMultiplexer(mockDatabase);
            var mockAppender = BuildSuccessAppender(mockMultiplexer, errorHandler);

            var loggingEvent = new LoggingEvent(typeof(AppenderTests), null, "LoggerName", Level.Info, "Example of a Redis Stream logging entry", null);

            mockAppender.Object.DoAppend(loggingEvent);

            Assert.Null(errorHandler.LogException);
        }


        [Fact]
        public void FailedToLogToRedis_IncorrectReturnMessageId()
        {
            var errorHandler = new Log4NetErrorHandler();
            var mockDatabase = BuildDatabase(RedisValue.Null);
            var mockMultiplexer = BuildSuccessConnectionMultiplexer(mockDatabase);
            var mockAppender = BuildSuccessAppender(mockMultiplexer, errorHandler);

            var loggingEvent = new LoggingEvent(typeof(AppenderTests), null, "LoggerName", Level.Info, "Example of a Redis Stream logging entry", null);

            mockAppender.Object.DoAppend(loggingEvent);

            Assert.NotNull(errorHandler.LogException);
            Assert.IsAssignableFrom<RedisException>(errorHandler.LogException);
        }

        [Fact]
        public void FailedToLogToRedis_FailedConnectionToRedis()
        {
            var errorHandler = new Log4NetErrorHandler();
            var mockMultiplexer = BuildFailingConnectionMultiplexer();
            var mockAppender = BuildSuccessAppender(mockMultiplexer, errorHandler);

            var loggingEvent = new LoggingEvent(typeof(AppenderTests), null, "LoggerName", Level.Info, "Example of a Redis Stream logging entry", null);

            mockAppender.Object.DoAppend(loggingEvent);

            Assert.NotNull(errorHandler.LogException);
            Assert.IsAssignableFrom<RedisConnectionException>(errorHandler.LogException);
        }

        private Mock<RedisStreamAppender> BuildSuccessAppender(Mock<IConnectionMultiplexer> mockMultiplexer, Log4NetErrorHandler errorHandler)
        {
            var mockAppender = new Mock<RedisStreamAppender>() { CallBase = true };
            //mockAppender.Setup(_ => _.RedisConnectionString).Returns("a");
            //mockAppender.Setup(_ => _.RedisStreamName).Returns("b");
            mockAppender.SetupProperty(_ => _.RedisConnectionString, "a");
            mockAppender.SetupProperty(_ => _.RedisStreamName, "b");
            mockAppender.Setup(_ => _.ConnectToRedis(It.IsAny<string>()))
                .Returns(mockMultiplexer.Object);

            mockAppender.Object.Threshold = Level.Info;
            mockAppender.Object.ErrorHandler = errorHandler;
            mockAppender.Object.Layout = new PatternLayout("%date [%thread] %-5level %logger [%property{NDC}] - %message%newline");

            return mockAppender;
        }

        private Mock<IDatabase> BuildDatabase(RedisValue returnValue)
        {
            var mockDatabase = new Mock<IDatabase>();
            mockDatabase.Setup(_ => _.StreamAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue?>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<CommandFlags>()))
                        .Returns(System.Threading.Tasks.Task.FromResult<RedisValue>(returnValue));

            return mockDatabase;
        }
        private Mock<IConnectionMultiplexer> BuildSuccessConnectionMultiplexer(Mock<IDatabase> mockDatabase)
        {
            var mockMultiplexer = new Mock<IConnectionMultiplexer>();
            mockMultiplexer.Setup(_ => _.IsConnected).Returns(false);
            mockMultiplexer.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                            .Returns(mockDatabase.Object);

            return mockMultiplexer;
        }
        private Mock<IConnectionMultiplexer> BuildFailingConnectionMultiplexer()
        {
            var mockMultiplexer = new Mock<IConnectionMultiplexer>();
            mockMultiplexer.Setup(_ => _.IsConnected).Returns(false);
            mockMultiplexer.Setup(_ => _.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                            .Throws(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Failed"));


            return mockMultiplexer;
        }
    }

    public class Log4NetErrorHandler : IErrorHandler
    {
        public string Message { get; set; }
        public Exception LogException { get; set; }
        public ErrorCode LogErrorCode { get; set; }

        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            Message = message;
            LogException = e;
            LogErrorCode = errorCode;
        }

        public void Error(string message, Exception e)
        {
            Message = message;
            LogException = e;
            LogErrorCode = default(ErrorCode);
        }

        public void Error(string message)
        {
            Message = message;
            LogException = null;
            LogErrorCode = default(ErrorCode);
        }
    }
}
