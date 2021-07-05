namespace HeadlessArchitect.DeliveryApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using Deliverystack.DeliveryApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //TODO: ModularBlocks and case insensittive serialization defaults
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ContentstackDeliveryClientOptions>(
                new ContentstackDeliveryClientOptions(Configuration.GetSection("ContentstackOptions")));
            services.AddHttpClient<IDeliveryClient, ContentstackDeliveryClient>();

            //TODO: initialize and register object rather than lazy-loading constructor logic
            services.AddSingleton<PathApiCache>(sp => { return new PathApiCache(sp.GetRequiredService<IDeliveryClient>()); });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HeadlessArchitect.DeliveryApi", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HeadlessArchitect.DeliveryApi v1"));
            }

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
