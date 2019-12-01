using System;
using Normal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddNormal(this IServiceCollection services, Action<IServiceProvider, IDbContextBuilder> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            services.AddScoped<IDbContext>((sp) =>
            {
                var dbContextCallback = Bind(callback, sp);
                return new DbContext(dbContextCallback);
            });
            return services;
        }

        private static Action<T2> Bind<T1, T2>(Action<T1, T2> action, T1 arg1)
        {
            return (arg2) => action(arg1, arg2);
        }
    }
}