using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Log4Net.RedisStream.Example
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

                var logger = logRepository.GetLogger("DefaultLogger");

                logger.Log(typeof(Program), Level.Info, "Example of a Redis Stream logging entry", null);
            }
            catch (System.Exception ex)
            {
                var t = ex;
                //todo: clean up
            }

        }
    }
}
