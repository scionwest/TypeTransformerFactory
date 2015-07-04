using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Transformer.Messages;

namespace Transformer.Transformers
{
    public class MessageTransformFactory
    {
        /// <summary>
        /// The assemblies to cache. Defaults to including the assembly this factory exists in.
        /// </summary>
        private static List<Assembly> assembliesToCache
            = new List<Assembly> { typeof(MessageTransformFactory).GetTypeInfo().Assembly };

        /// <summary>
        /// The factory method optionally used to instance the transformer
        /// </summary>
        private static Func<Type, IMessageTransformer> factoryMethod;

        /// <summary>
        /// The device type cache
        /// </summary>
        private static Dictionary<DeviceTypeEnum, Type> deviceCache
            = new Dictionary<DeviceTypeEnum, Type>();

        /// <summary>
        /// Initializes the <see cref="CommandFormatterFactory"/> class.
        /// </summary>
        static MessageTransformFactory()
        {
            BuildCache();
        }

        /// <summary>
        /// Sets the formatter factory used to instance formatters.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public static void SetFormatterFactory(Func<Type, IMessageTransformer> factory)
        {
            //if (availableMessages.Any(type => type != typeof(IMessageTransformer<>)))
            //{
            //    throw new InvalidOperationException("The Factory was provided with a collection of Types containing non-IMessageTransformer<T> implementations");
            //}
            //else if (availableMessages.Any(type => type.GetTypeInfo().GetCustomAttribute<TransformableAttribute>() == null))
            //{
            //    throw new InvalidOperationException("At least one of the Types provided to the Factory for transforming did not include a TransformableAttribute.");
            //}

            //// Create a dictionary of cached transformers for each of the available device types.
            //MessageTransformFactory.deviceCache = availableMessages.ToDictionary(type =>
            //{
            //    var attributeOnType = type.GetTypeInfo().GetCustomAttribute<TransformableAttribute>();
            //    return attributeOnType.DeviceType;
            //}
            //, type => type);

            MessageTransformFactory.factoryMethod = factory;
        }

        /// <summary>
        /// Scans a given assembly for formatters.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        public static void ScanAssembly(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName", "A valid assembly name must be provided.");
            }

            Assembly assembly = Assembly.Load(assemblyName);

            if (assembliesToCache.Any(a => a.FullName == assemblyName.FullName))
            {
                return;
            }

            assembliesToCache.Add(assembly);
            CacheAssemblyFormatters(assembly);
        }

        /// <summary>
        /// Gets the available transformer types that have been registered to this factory.
        /// </summary>
        /// <returns></returns>
        public static Type[] GetAvailableTransformerTypes()
        {
            return deviceCache.Values.ToArray();
        }

        /// <summary>
        /// Gets an ICommandFormatter implementation for the command code given.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Returns an ICommandFormatter concrete type</returns>
        public IMessageTransformer<T> CreateTransformer<T>(DeviceTypeEnum deviceType) where T : class
        {
            // If we have a factory method, then we use it.
            if (factoryMethod == null)
            {
                throw new NullReferenceException("The MessageTransformerFactory did not have its factory method set.");
            }

            // Cast the non-generic return value to the generic version for the caller.
            Type transformerType = MessageTransformFactory.deviceCache[deviceType];
            return factoryMethod(transformerType) as IMessageTransformer<T>;
        }

        /// <summary>
        /// Builds the cache of ICommandResponse instances.
        /// </summary>
        private static void BuildCache()
        {
            foreach (Assembly assembly in assembliesToCache)
            {
                CacheAssemblyFormatters(assembly);
            }
        }

        private static void CacheAssemblyFormatters(Assembly assembly)
        {
            var transformableTypes = assembly.DefinedTypes
                .Where(type => type
                .ImplementedInterfaces
                .Any(inter => inter == typeof(TransformableAttribute)) && !type.IsAbstract);

            foreach (TypeInfo transformer in transformableTypes)
            {
                var commandCode = transformer.GetCustomAttribute<TransformableAttribute>();

                // Create a new instance of the response
                deviceCache.Add(commandCode.DeviceType, transformer.AsType());
            }
        }
    }
}
