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

                if (!File.Exists("log4net.config"))
                    throw new FileLoadException("Failed to load log4net.config.  It is not is app path.");

                XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
                Console.WriteLine($"Log4Net configuration loaded.");

                var logger = logRepository.GetLogger("DefaultLogger");
                Console.WriteLine($"Default logger created.");

                logger.Log(typeof(Program), Level.Info, "Example of a Redis Stream logging entry", null);
                Console.WriteLine($"Log attempted.");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Failed to execute Redis Stream logging.");
                Console.WriteLine(ex);
            }

            Console.WriteLine($"Redis Stream log example finished.");

        }
    }
}
