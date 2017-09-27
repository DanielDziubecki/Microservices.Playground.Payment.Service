using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Payment.Model.PayU;
using Payment.Service.Providers.PayU;

namespace Payment.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddOptions();

            services.Configure<PayUSettings>(Configuration.GetSection("PayUSettings"));

            var builder = new ContainerBuilder();
            builder.RegisterType<PayUClient>().As<IPayUClient>();

            builder.Register(context =>
                {
                    var busControl = Bus.Factory.CreateUsingRabbitMq(rabbitMqConfig =>
                    {
                        var host = rabbitMqConfig.Host(new Uri("rabbitmq://localhost/"), h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        rabbitMqConfig.ConfigureJsonSerializer(settings =>
                        {
                            settings.NullValueHandling = NullValueHandling.Ignore;
                            return settings;
                        });
                    });
                    return busControl;
                })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();

            builder.Populate(services);

            var applicationContainer = builder.Build();

            var bus = applicationContainer.Resolve<IBusControl>();
            bus.Start();
            return new AutofacServiceProvider(applicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}