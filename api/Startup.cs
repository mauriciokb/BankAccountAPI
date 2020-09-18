using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;

namespace BankAccountWebAPI
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
            Context context = new Context();

            services.AddSingleton<IDataReader>(new DatabaseReader(context));
            services.AddSingleton<IPersistor<Account>>(new DatabasePersistence<Account>(context));
            services.AddSingleton<Withdraw>(new Withdraw(new DatabaseReader(context), new DatabasePersistence<SingleAccountOperation>(context)));
            services.AddSingleton<Deposit>(new Deposit(new DatabaseReader(context), new DatabasePersistence<SingleAccountOperation>(context)));
            services.AddSingleton<Transference>(new Transference(new DatabaseReader(context), new DatabasePersistence<DoubleAccountOperation>(context)));      

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                int errorCode;

                if (exception is ArgumentNullException || exception is ArgumentOutOfRangeException || exception is InvalidOperationException)
                    errorCode = (int)System.Net.HttpStatusCode.UnprocessableEntity;
                else if (exception is KeyNotFoundException)
                    errorCode = (int)System.Net.HttpStatusCode.NotFound;
                else
                    errorCode = (int)System.Net.HttpStatusCode.InternalServerError;

                context.Response.StatusCode = errorCode;
                await context.Response.WriteAsync(exception.Message);
            }));         

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });        

          
        }
    }
}
