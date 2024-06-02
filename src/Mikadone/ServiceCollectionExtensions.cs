using Microsoft.Extensions.DependencyInjection;
using Mikadone.GoalEdit;

namespace Mikadone;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddMikadoneServices(this IServiceCollection collection)
    => collection
    .AddSingleton<IGoalStorage, IsolatedGoalStorage>()
    .AddSingleton<IGoalIdProvider, GoalIdProvider>()
    .AddSingleton<IGoalEditing, GoalEditing>()
    .AddSingleton<IGoalFactory, GoalFactory>()
    .AddSingleton<IGoalSerialization, GoalSerialization>()
    .AddSingleton<IRootGoalProvider, RootGoalProvider>()
    .AddTransient<MainViewModel>()
    .AddTransient<GoalsViewModel>();
}