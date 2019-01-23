using FundsHistoricalQuotes.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FundsHistoricalQuotes.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services
                .AddMvc()
                .AddJsonOptions(opt => { opt.SerializerSettings.DateFormatString = "yyyy-MM-dd"; });

            services.AddHttpClient<MorningstarHttpClient>();

            services.AddTransient<IQuotesCalculationService, QuotesCalculationService>();

            services.AddTransient<IEtfHistoricalQuotesProvider, MorningstarEtfHistoricalQuotesProvider>();
            services.Decorate<IEtfHistoricalQuotesProvider, EtfHistoricalQuotesInMemoryCacheDecorator>();
            services.Decorate<IEtfHistoricalQuotesProvider, EtfHistoricalQuotesConcurrencyControlDecorator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();
        }
    }
}