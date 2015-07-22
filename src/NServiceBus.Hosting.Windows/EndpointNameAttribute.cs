namespace NServiceBus
{
    using System;

    /// <summary>
    /// Used to specify the name of the current endpoint.
    /// Will be used as the name of the input queue as well.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EndpointNameAttribute : Attribute
    {
        /// <summary>
        /// Used to specify the name of the current endpoint.
        /// Will be used as the name of the input queue as well.
        /// </summary>
        public EndpointNameAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            Name = name;
        }

        /// <summary>
        /// The name of the endpoint.
        /// </summary>
        public string Name { get; private set; }
    }
}
