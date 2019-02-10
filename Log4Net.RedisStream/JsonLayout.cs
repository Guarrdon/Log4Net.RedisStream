using System.IO;
using log4net.Core;
using log4net.Layout;
using Newtonsoft.Json;

namespace Log4Net.RedisStream
{
    public class JsonLayout : LayoutSkeleton
    {

        public JsonLayout():base()
        {
          base.IgnoresException = true;   
        }
        public override void ActivateOptions()
        {
            
        }

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            var logEventJson = JsonConvert.SerializeObject(loggingEvent);
            writer.Write(logEventJson);
        }
    }
}