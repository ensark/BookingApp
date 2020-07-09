using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.SignalR;
using Hangfire;
using Hangfire.SqlServer;
using FluentValidation.AspNetCore;
using FluentValidation;
using Stripe;
using Booking.API.Extensions;
using Booking.Infrastructure.Database;
using Booking.Infrastructure.Payments.Stripe;
using Booking.Infrastructure.SignalR.Chat.Hubs;
using Booking.Core.Domain.DTOs;
using Booking.Common.Validations;

namespace Booking.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureStripe();

            services.AddHangfire(_configuration);
            services.AddHangfireServer();

            services.AddDbContext<BookingDBContext>(opt =>
            {  
                opt.UseLazyLoadingProxies().UseSqlServer(_configuration.GetConnectionString(Constants.Config.CONNECTION_STRING), x => x.UseNetTopologySuite());
                opt.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
            });

            services.AddJwtAuthentication(_configuration);
            services.AddIocMapping();

            services.AddSwaggerDocument();

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithOrigins("http://localhost:4200")
                  .AllowCredentials();
            }));

            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUserDtoValidator>());                                
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHangfireDashboard();
            app.StartHangfire(_configuration);

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");               
            });

            app.UseMvc();
        }

        public void ConfigureStripe()
        {
            var stripeSecretKey = _configuration[StripeConfig.STRIPE_SECRET_KEY];
            StripeConfiguration.ApiKey = stripeSecretKey;
        }
    }
}
