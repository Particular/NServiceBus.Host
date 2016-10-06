namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Features;

    class StartableAndStoppableFeature : Feature
    {
        public StartableAndStoppableFeature()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var startableStoppableInstances =
                context.Settings.GetOrDefault<List<IWantToRunWhenEndpointStartsAndStops>>()
                ?? new List<IWantToRunWhenEndpointStartsAndStops>();

            var typesToExclude = new HashSet<Type>(startableStoppableInstances.Select(i => i.GetType()));

            context.Settings.GetAvailableTypes()
                .Where(IsConcrete)
                .Where(IsIWantToRunWhenEndpointStartsAndStops)
                .Where(t => !typesToExclude.Contains(t))
                .ToList()
                .ForEach(t => context.Container.ConfigureComponent(t, DependencyLifecycle.InstancePerCall));

            context.RegisterStartupTask(b =>
            {
                var instances = b.BuildAll<IWantToRunWhenEndpointStartsAndStops>().Concat(startableStoppableInstances);
                return new StartableAndStoppableTask(instances);
            });
        }

        static bool IsIWantToRunWhenEndpointStartsAndStops(Type type)
        {
            return typeof(IWantToRunWhenEndpointStartsAndStops).IsAssignableFrom(type);
        }

        static bool IsConcrete(Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        class StartableAndStoppableTask : FeatureStartupTask
        {
            public StartableAndStoppableTask(IEnumerable<IWantToRunWhenEndpointStartsAndStops> startablesAndStoppables)
            {
                startAndStoppablesRunner = new StartableAndStoppableRunner(startablesAndStoppables);
            }

            protected override Task OnStart(IMessageSession session)
            {
                return startAndStoppablesRunner.Start(session);
            }

            protected override Task OnStop(IMessageSession session)
            {
                return startAndStoppablesRunner.Stop(session);
            }

            StartableAndStoppableRunner startAndStoppablesRunner;
        }
    }
}