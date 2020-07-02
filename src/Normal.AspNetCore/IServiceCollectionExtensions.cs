using System;
using Normal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddNormal(this IServiceCollection services, Action<IServiceProvider, IDatabaseBuilder> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            services.AddScoped<IDatabase>((serviceProvider) =>
            {
                return new Database((configure) => callback(serviceProvider, configure));
            });
            return services;
        }
    }
}