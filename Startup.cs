using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Catalog.Repositoies;
using Catalog.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Catalog
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
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            var mongoDBSettings = Configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();

            services.AddSingleton<IMongoClient>(servicesProvider => new MongoClient(mongoDBSettings.ConnectionString));

            services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();

            // services.AddSingleton<IItemsRepository, InMemItemsRepository>();

            services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });

            services.AddHealthChecks().AddMongoDb(
                mongoDBSettings.ConnectionString,
                name: "mongo",
                timeout: TimeSpan.FromSeconds(3),
                tags: new[] { "ready" }
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }
            // Be ready to docker image with if expression.
            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                status = report.Status.ToString(),
                                checks = report.Entries.Select(
                                    entity => new
                                    {
                                        name = entity.Key,
                                        status = entity.Value.Status.ToString(),
                                        exception = entity.Value.Exception != null ? entity.Value.Exception.Message : "none",
                                        duration = entity.Value.Duration.ToString()
                                    }
                                )
                            }
                        );
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                        // more database health check review https://github.com/xabaril/AspNetCore.Diagnostics.HealthChecks
                    }
                });
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = (_) => false });
            });
        }
    }
}
