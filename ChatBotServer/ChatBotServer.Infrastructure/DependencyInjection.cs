using ChatBotServer.Domain.Configuration;
using ChatBotServer.Domain.Interfaces;
using ChatBotServer.Infrastructure.Data;
using ChatBotServer.Infrastructure.Repositories;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using ChatBotServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBotServer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register ChatBot services
            services.Configure<ChatBotServiceOptions>(options => configuration.GetSection("ChatBotService").Bind(options));
            
            var chatBotOptions = configuration.GetSection("ChatBotService").Get<ChatBotServiceOptions>();
            if (chatBotOptions?.UseAzureAI == true)
            {
                services.AddScoped<IChatBotService, AzureAIChatBotService>();
            }
            else
            {
                services.AddScoped<IChatBotService, FakeChatBotService>();
            }

            return services;
        }
    }
}
