namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.IO;
    using System.Linq;
    using Arguments;

    /// <summary>
    ///     Representation of an Endpoint Type with additional descriptive properties.
    /// </summary>
    class EndpointType
    {
        internal EndpointType(HostArguments arguments, Type type) : this(type)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }
            this.arguments = arguments;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EndpointType" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public EndpointType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Type = type;
            AssertIsValid();
        }

        internal Type Type { get; }

        public string EndpointConfigurationFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Type.Assembly.ManifestModule.Name + ".config");

        public string EndpointVersion => FileVersionRetriever.GetFileVersion(Type);

        public string AssemblyQualifiedName => Type.AssemblyQualifiedName;

        public string EndpointName
        {
            get
            {
                var hostEndpointAttribute = (EndpointNameAttribute)Type.GetCustomAttributes(typeof(EndpointNameAttribute), false)
                    .FirstOrDefault();
                return hostEndpointAttribute != null ? hostEndpointAttribute.Name : arguments.EndpointName;
            }
        }

        public string ServiceName
        {
            get
            {
                var serviceName = Type.Namespace ?? Type.Assembly.GetName().Name;

                if (arguments.ServiceName != null)
                {
                    serviceName = arguments.ServiceName;
                }

                return serviceName;
            }
        }

        void AssertIsValid()
        {
            var constructor = Type.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new InvalidOperationException(
                    "Endpoint configuration type needs to have a default constructor: " + Type.FullName);
            }
        }

        readonly HostArguments arguments = new HostArguments(new string[0]);
    }
}