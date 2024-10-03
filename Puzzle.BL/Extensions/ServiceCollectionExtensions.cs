using Microsoft.Extensions.DependencyInjection;
using Puzzle.BL.Factories;
using Puzzle.BL.Interfaces;
using Puzzle.BL.Models;

namespace Puzzle.BL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBLServices(this IServiceCollection services)
        {
            services.AddTransient<ISolver, Solver>();
            services.AddTransient<IBoard, Board>();
            services.AddFactory<IPermutationGenerator, PermutationGenerator>();
            services.AddFactory<ICardMove, CardMove>();
            services.AddFactory<IEmoticonPart, EmoticonPart>();
            services.AddFactory<ICard, Card>();
            return services;
        }

        public static void AddFactory<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>();

            services.AddSingleton<Func<TService>>(x => x.GetRequiredService<TService>);

            services.AddSingleton<IFactory<TService>, Factory<TService>>();
        }
    }
}
