using Microsoft.Extensions.DependencyInjection;
using Mikadone.GoalEdit;

namespace Mikadone;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddMikadoneServices(this IServiceCollection collection)
    => collection
    .AddSingleton<IGoalFileNameProvider, GoalFileNameProvider>()
    .AddSingleton<IGoalStorage, IsolatedGoalStorage>()
    .AddSingleton<IGoalIdProvider, GoalIdProvider>()
    .AddSingleton<IGoalEditing, GoalEditing>()
    .AddSingleton<IGoalFactory, GoalFactory>()
    .AddSingleton<IGoalSerialization, GoalSerialization>()
    .AddSingleton<IGoalDeserialization, GoalDeserialization>()
    .AddSingleton<IRootGoalProvider, RootGoalProvider>()
    .AddSingleton<IGoalAutoSave, GoalAutoSave>()
    .AddTransient<MainViewModel>()
    .AddTransient<GoalsViewModel>();
}