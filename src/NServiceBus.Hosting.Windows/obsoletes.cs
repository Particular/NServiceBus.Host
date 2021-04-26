#pragma warning disable 1591
#pragma warning disable PS0018 // A task-returning method should have a CancellationToken parameter unless it has a parameter implementing ICancellableContext
namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    // Internal only type referenced by the APIApprovals test
    class HostingInternalType
    {
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public interface IWantToRunWhenEndpointStartsAndStops
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        Task Start(IMessageSession session);

        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        Task Stop(IMessageSession session);
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public interface IConfigureThisEndpoint
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]

        void Customize(EndpointConfiguration configuration);
    }


    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public static class EndpointStartableAndStoppableExtensions
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        public static void RunWhenEndpointStartsAndStops(this EndpointConfiguration configuration, IWantToRunWhenEndpointStartsAndStops startableAndStoppable)
        {
            throw new NotImplementedException();
        }
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public static class EndpointConfigurationExtensions
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        public static void DefineEndpointName(this EndpointConfiguration configuration, string endpointName)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public sealed class EndpointNameAttribute : Attribute
    {
        public EndpointNameAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
#pragma warning disable IDE1006 // Naming Styles
    public interface Integration : IProfile
#pragma warning restore IDE1006 // Naming Styles
    {
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public interface IProfile
    {
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public interface IWantTheListOfActiveProfiles
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        IEnumerable<Type> ActiveProfiles { get; set; }
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
#pragma warning disable IDE1006 // Naming Styles
    public interface Lite : IProfile
#pragma warning restore IDE1006 // Naming Styles
    {
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
#pragma warning disable IDE1006 // Naming Styles
    public interface Production : IProfile
#pragma warning restore IDE1006 // Naming Styles
    {
    }
}

namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.Collections.Generic;

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public class Program
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        static void Main(string[] args)
        {
            throw new NotImplementedException();
        }
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public class InstallWindowsHost : MarshalByRefObject
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        public InstallWindowsHost(Type endpointType, string[] args, string endpointName, IEnumerable<string> scannableAssembliesFullName)
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        public void Install(string username)
        {
            throw new NotImplementedException();
        }
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public class WindowsHost : MarshalByRefObject
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        public WindowsHost(Type endpointType, string[] args, string endpointName, IEnumerable<string> scannableAssembliesFullName)
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        public void Start()
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}

namespace NServiceBus.Hosting.Profiles
{
    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public interface IHandleAnyProfile : IHandleProfile<IProfile>, IWantTheListOfActiveProfiles
    {
    }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public interface IHandleProfile<T> : IHandleProfile where T : IProfile { }

    [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
        TreatAsErrorFromVersion = "9.0",
        RemoveInVersion = "10.0")]
    public interface IHandleProfile
    {
        [ObsoleteEx(Message = "The windows host has been deprecated. It is recommended to switch to self-hosting or the generic host instead. See the upgrade guide and docs for alternatives.",
            TreatAsErrorFromVersion = "9.0",
            RemoveInVersion = "10.0")]
        void ProfileActivated(EndpointConfiguration config);
    }
}
#pragma warning restore 1591