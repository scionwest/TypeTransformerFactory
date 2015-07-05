using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Transformer.Messages;

namespace Transformer.Transformers
{
    public class MessageTransformFactory
    {
        /// <summary>
        /// The assemblies to cache. Defaults to including the assembly this factory exists in.
        /// if there are additional assemblies that hold transformers, they can be added via the 
        /// MessageTransformFactory.ScanAssembly(Assembly) method.
        /// </summary>
        private static List<Assembly> assembliesToCache
            = new List<Assembly> { typeof(MessageTransformFactory).GetTypeInfo().Assembly };

        /// <summary>
        /// The factory method used to instance a transformer
        /// </summary>
        private static Func<Type, IMessageTransformer> factoryMethod;

        /// <summary>
        /// The DeviceType to Transformer mapping cache
        /// </summary>
        private static Dictionary<DeviceTypeEnum, Type> deviceTransformerMapCache
            = new Dictionary<DeviceTypeEnum, Type>();

        /// <summary>
        /// Initializes the <see cref="CommandFormatterFactory"/> class.
        /// This will build the initial device to transformer mapping when the
        /// Factory is first used.
        /// </summary>
        static MessageTransformFactory()
        {
            BuildCache();
        }

        /// <summary>
        /// Sets the transformer factory used to instance transformers.
        /// </summary>
        /// <param name="factory">The factory delegate used to instance new IMessageTransformer objects.</param>
        public static void SetTransformerFactory(Func<Type, IMessageTransformer> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory", "Factory delegate can not be null.");
            }

            MessageTransformFactory.factoryMethod = factory;
        }

        /// <summary>
        /// Scans a given assembly for IMessageTransformer implementations.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to scan.</param>
        public static void ScanAssembly(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName", "A valid assembly name must be provided.");
            }

            Assembly assembly = Assembly.Load(assemblyName);

            // If we had previously scanned the assembly, we return. No need to scan twice.
            if (assembliesToCache.Any(a => a.FullName == assemblyName.FullName))
            {
                return;
            }

            assembliesToCache.Add(assembly);

            // Create a mapping from DeviceType to Transformers for this assembly.
            MapDeviceTypesFromAssembly(assembly);
        }

        /// <summary>
        /// Gets the available transformer types that have been registered to this factory.
        /// </summary>
        public static Type[] GetAvailableTransformerTypes()
        {
            return deviceTransformerMapCache.Values.ToArray();
        }

        /// <summary>
        /// Gets an IMessageTransformer implementation for the Device Type given.
        /// </summary>
        /// <param name="deviceType">The DeviceType that the factory must create an IMessageTransformer for.</param>
        public IMessageTransformer<T> CreateTransformer<T>(DeviceTypeEnum deviceType) where T : class
        {
            // If we have a factory method, then we use it.
            if (factoryMethod == null)
            {
                throw new NullReferenceException("The MessageTransformerFactory did not have its factory method set.");
            }

            // Cast the non-generic return value to the generic version for the caller.
            Type transformerType = MessageTransformFactory.deviceTransformerMapCache[deviceType];

            // Since our factory delegate method returns the non-generic implementation, we must
            // case it to the required generic version of it when we return the result.
            return factoryMethod(transformerType) as IMessageTransformer<T>;
        }

        /// <summary>
        /// Builds the cache of IMessageTransformer Types that can be used by this factory.
        /// </summary>
        private static void BuildCache()
        {
            foreach (var assembly in assembliesToCache)
            {
                MapDeviceTypesFromAssembly(assembly);
            }
        }

        /// <summary>
        /// Creates a DeviceType to IMessageTransformer Type mapping.
        /// </summary>
        /// <param name="assembly"></param>
        private static void MapDeviceTypesFromAssembly(Assembly assembly)
        {
            // Find all of the Types that implement the IMessageTransformer interface
            var transformableTypes = assembly.DefinedTypes
                .Where(type => type
                .ImplementedInterfaces
                .Any(inter => inter == typeof(IMessageTransformer)) && !type.IsAbstract);

            foreach (TypeInfo transformer in transformableTypes)
            {
                // Fetch the Transformable attribute from the type and create a mapping
                // storing the device type that this IMessageTransformer targets.
                var commandCode = transformer.GetCustomAttribute<TransformableAttribute>();
                deviceTransformerMapCache.Add(commandCode.DeviceType, transformer.AsType());
            }
        }
    }
}
