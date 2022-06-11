using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Licensing;

namespace MetaMetricsViewer.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTIzODE0QDMxMzkyZTMzMmUzMGpHZERQdWlERE5IUlpHMk5sdUFlNnVWVjRVaVZma2svOWE3aExiQjJuVVE9;NTIzODE1QDMxMzkyZTMzMmUzMGhwOFVXT3RSVTU1RW84Y0twOUlqM0VybnNWNFRKZHNYSEZrbGlUUUV5SDA9;NTIzODE2QDMxMzkyZTMzMmUzMFJNQktweVhDVTIxekVZdGlQRnVLVHAzVjRNcFA2SXBiR0UvVk5IU0djYnM9;NTIzODE3QDMxMzkyZTMzMmUzMFNDRFFuUWhkbWxadWNTSzVtY0hMUmxGOUxDdkFPbG1NTDBrU0V4SUNSWnc9;NTIzODE4QDMxMzkyZTMzMmUzMGZQaFYxYk9vMTVFSHk0eHdPTmFNekU1alE1ZUFaanNFM1k3M3RUb2ozSTg9;NTIzODE5QDMxMzkyZTMzMmUzMFZ5S0dVTHRnUkh2VENqcVpISXBkOTY4Zk92bkVtelpiWHdiQlI5bXFEcDA9;NTIzODIwQDMxMzkyZTMzMmUzMENLR0IxbEpHdEI3SVFhVzFFUU90N1MvajdlbmZlQ2tKNTY2YzJnY01BRlk9;NTIzODIxQDMxMzkyZTMzMmUzMGwzdUxJWUlFMElRMkhDSEozVGRTMGpJNnJyT211MitNdlg5YVl1RU9udGc9;NTIzODIyQDMxMzkyZTMzMmUzMFZPbGJQcExXMHFEMitLUHZwbmV0anExZldNN2FHMUdIVTVYOTFkZVVsRlU9;NTIzODIzQDMxMzkyZTMzMmUzMFQ3Y0NENFBhc1B1bDRYSjVyemlLcVM5VHFBOGVLZW81YlRLT1o4dnRSRDA9;NTIzODI0QDMxMzkyZTMzMmUzMGl6NnF6dTYydkNtZUFvSlVpdzJreFRYUncwU1BvSDFsMC91TXdGWUdQSDA9");
            var valid = Syncfusion.Licensing.SyncfusionLicenseProvider.ValidateLicense(Platform.Blazor);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
