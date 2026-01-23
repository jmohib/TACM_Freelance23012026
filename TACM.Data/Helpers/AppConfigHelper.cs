using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACM.Core;

namespace TACM.Data.Helpers
{
    public static class AppConfigHelper
    {
        public static string GetConnectionString()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("sharedsettings.json").Result;
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var config = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                .Build();

            return config.GetConnectionString("DefaultConnection");
        }
    }
}
