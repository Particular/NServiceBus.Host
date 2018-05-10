namespace NServiceBus
{
    using System;


    [ObsoleteEx(
        TreatAsErrorFromVersion = "8.0",
        Message = "Performance counters are now provided via a separate package.",
        RemoveInVersion = "9.0")]
    [AttributeUsage(AttributeTargets.Class)]
#pragma warning disable 1591
    public sealed class EndpointSLAAttribute : Attribute

    {
        public EndpointSLAAttribute(string sla)
        {
            throw new NotImplementedException();
        }
        public TimeSpan SLA
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
#pragma warning restore 1591
}