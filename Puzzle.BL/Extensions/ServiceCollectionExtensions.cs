﻿using Microsoft.Extensions.DependencyInjection;
using Puzzle.BL.Factories;
using Puzzle.BL.Interfaces;
using Puzzle.BL.Models;

namespace Puzzle.BL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBLServices(this IServiceCollection services)
        {
            services.AddTransient<ISolver, Board3x3Solver>();
            services.AddTransient<IBoardBuilder, Board3x3Builder>();
            services.AddTransient<ICardMover, CardMover>();
            services.AddFactory<IPermutationGenerator, PermutationGenerator>();
            services.AddFactory<ICardMove, CardMove>();
            services.AddFactory<IEmoticonPart, EmoticonPart>();
            services.AddFactory<ICard, Card>();
            services.AddFactory<Board3x3, Board3x3>();
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
