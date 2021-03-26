using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AppSettingsToAzureWebAppsConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("please drop appsettings.json file into the executable file, Exit.");
                Console.ReadKey();
            }

            var filePath = args[0];

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(filePath);

            var config = builder.Build();
            var items = config.AsEnumerable().ToList();

            var appConfig = new List<ApplicationConfigModel>();
            var connConfig = new List<ConnectionStringConfigModel>();

            foreach (var (key, value) in items)
            {
                if (value == null)
                {
                    continue;
                }

                if (key.StartsWith("ConnectionStrings:"))
                {
                    connConfig.Add(new ConnectionStringConfigModel()
                    {
                        name = key.TrimStart("ConnectionStrings:"),
                        value = value
                    });
                }
                else
                {
                    appConfig.Add(new ApplicationConfigModel
                    {
                        name = key,
                        value = value,
                        slotSetting = false
                    });
                }
            }

            var nowUnix = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var jsonSetting = new JsonSerializerSettings() {Formatting = Formatting.Indented};

            var appConfigFile = $"app_config_{nowUnix}.json";
            var connConfigFile = $"conn_config_{nowUnix}.json";
            
            Console.WriteLine($"save {appConfigFile}");
            Console.WriteLine($"save {connConfigFile}");
            
            File.WriteAllText(appConfigFile, JsonConvert.SerializeObject(appConfig, jsonSetting));
            File.WriteAllText(connConfigFile, JsonConvert.SerializeObject(connConfig, jsonSetting));
            
            Console.WriteLine("done");
        }
    }
}