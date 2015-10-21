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
                throw new ArgumentNullException("arguments");
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
                throw new ArgumentNullException("type");
            }
            this.type = type;
            AssertIsValid();
        }

        internal Type Type => type;

        public string EndpointConfigurationFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, type.Assembly.ManifestModule.Name + ".config");

        public string EndpointVersion => FileVersionRetriever.GetFileVersion(type);

        public string AssemblyQualifiedName => type.AssemblyQualifiedName;

        public string EndpointName
        {
            get
            {
                var hostEndpointAttribute = (EndpointNameAttribute)type.GetCustomAttributes(typeof(EndpointNameAttribute), false)
                    .FirstOrDefault();
                return hostEndpointAttribute != null ? hostEndpointAttribute.Name : arguments.EndpointName;
            }
        }

        public string ServiceName
        {
            get
            {
                var serviceName = type.Namespace ?? type.Assembly.GetName().Name;

                if (arguments.ServiceName != null)
                {
                    serviceName = arguments.ServiceName;
                }

                return serviceName;
            }
        }

        void AssertIsValid()
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new InvalidOperationException(
                    "Endpoint configuration type needs to have a default constructor: " + type.FullName);
            }
        }

        readonly HostArguments arguments = new HostArguments(new string[0]);
        readonly Type type;
    }
}