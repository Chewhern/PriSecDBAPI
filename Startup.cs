using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;

namespace PriSecDBAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            String CertPath = "";
            String Password = "";
            services.AddDistributedMemoryCache();

            SetPassword(ref Password);
            SetPath(ref CertPath);
            services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("{Path to Data Protection Folder as required by ASP.Net Core}")).ProtectKeysWithCertificate(
            new X509Certificate2(CertPath, Password));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void SetPassword(ref String Password)
        {
            StreamReader MyStreamReader = new StreamReader("{Path to X509 certificate password}");
            Password = MyStreamReader.ReadLine();
            MyStreamReader.Close();
        }

        public void SetPath(ref String CertPath)
        {
            StreamReader MyStreamReader = new StreamReader("{Path to X509 certificate}");
            CertPath = MyStreamReader.ReadLine();
            MyStreamReader.Close();
        }
    }
}
