using System;
using System.IO;
using log4net.Core;
using log4net.Layout;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;


namespace Log4Net.RedisStream.Test
{
    public class LayoutTests
    {
        [Fact]
        public void JsonLayoutIsLayoutSkeleton()
        {
            var layout = new JsonLayout();
            Assert.IsAssignableFrom<LayoutSkeleton>(layout);
        }

        [Fact]
        public void JsonLayoutRecommendedIgnoresExceptions()
        {
            var layout = new JsonLayout();
            Assert.True(((LayoutSkeleton)layout).IgnoresException);
        }

        [Fact]
        public void JsonLayoutOutputIsJson()
        {
            try
            {
                var layout = new JsonLayout();
                var str = new StringWriter();
                layout.Format(str, new LoggingEvent(typeof(LayoutTests), null, "LoggerName", Level.Info, "Example of a Redis Stream logging entry", null));
                var tmpObj = JObject.Parse(str.ToString());

                Assert.True(true);
            }
            catch (JsonReaderException)
            {
                Assert.True(false);
            }
            catch (Exception) //some other exception
            {
               Assert.True(false);
            }
        }
    }
}
