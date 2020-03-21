using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mediumvalue_api.Interface;
using mediumvalue_api.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SentencesFuzzyComparison;
using mediumvalue_api.Helper;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

namespace mediumvalue_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHttpClient<IRequestMethodYoula, MediumValueYoulaRequest>();
            services.AddHttpClient<IDataControlYandex, MediumValueYandexRequest>();
            services.AddHttpClient<ISearchYandexLayer, SearchYandexRequest>();
            services.AddHttpClient<RequestDatabase>();

            services.AddSingleton<BasicImplementation>();
            services.AddSingleton<FuzzyComparer>(instance => new FuzzyComparer(0.45, 0.95, 1, 1));
            services.AddSingleton<HandlerRequest>();
            services.AddSingleton<HtmlAgilityPack.HtmlWeb>(instance =>
            {
                HtmlAgilityPack.HtmlWeb htmlWeb = new HtmlAgilityPack.HtmlWeb();
                
                htmlWeb.UseCookies = true;
                htmlWeb.AutoDetectEncoding = true;
                htmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36";
                htmlWeb.PreRequest += (options) =>
                {
                    options.Proxy = new WebProxy("45.139.168.174:8000");
                    options.Proxy.Credentials = new NetworkCredential("Jh6PXe", "aGggJg");
                    options.AllowAutoRedirect = true;
                    options.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";

                    return true;
                };

                return htmlWeb;
            });
            services.AddSingleton<IParserMethodAvito, MediumValueAvitoParser>();
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

            app.UseEndpoints(options => options.MapControllers());
        }
    }
}
