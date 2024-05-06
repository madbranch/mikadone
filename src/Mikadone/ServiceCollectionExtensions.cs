using Microsoft.Extensions.DependencyInjection;

namespace Mikadone;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddMikadoneServices(this IServiceCollection collection)
    => collection
    .AddSingleton<IGoalIdProvider, GoalIdProvider>()
    .AddSingleton<IGoalFactory, GoalFactory>()
    .AddSingleton<IGoalSerialization, GoalSerialization>()
    .AddSingleton<IRootGoalProvider, RootGoalProvider>()
    .AddTransient<MainViewModel>()
    .AddTransient<GoalsViewModel>();
}