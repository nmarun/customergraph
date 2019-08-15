using CustomerGraph.Models.Schema;
using CustomerGraph.Models.Services;
using GraphiQl;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerGraph
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<CustomerType>();
            services.AddSingleton<CustomersQuery>();
            services.AddSingleton<CustomerSchema>();
            services.AddSingleton<IDependencyResolver>(
                c => new FuncDependencyResolver(type => c.GetRequiredService(type)));

            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
            });

            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(new CustomerSchema(new FuncDependencyResolver(type => sp.GetService(type))));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseGraphQL<ISchema>("/graphql");
            app.UseGraphQLWebSockets<CustomerSchema>("/graphql");
            app.UseWebSockets();
            app.UseGraphiQl();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
