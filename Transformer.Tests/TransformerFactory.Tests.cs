using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformer.Messages;
using Transformer.Transformers;

namespace Transformer.Tests
{
    [TestClass]
    public class TransformerFactoryTests
    {
        private IContainer container;

        [TestInitialize]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            // Register all of the available transformers.
            builder
                .RegisterTypes(MessageTransformFactory.GetAvailableTransformerTypes())
                .AsImplementedInterfaces()
                .AsSelf();

            // Build the IoC container
            this.container = builder.Build();

            // Define our factory method for resolving the transformer based on device type.
            MessageTransformFactory.SetTransformerFactory((type) =>
            {
                if (!type.IsAssignableTo<IMessageTransformer>())
                {
                    throw new InvalidOperationException("The type provided to the message transform factory resolver can not be cast to IMessageTransformer");
                }

                return container.Resolve(type) as IMessageTransformer;
            });
        }

        [TestMethod]
        public void Factory_resolves_AMessageTransformer_for_Foo_device_type()
        {
            // Arrange
            var factory = new MessageTransformFactory();
            IMessageTransformer<AMessage> transformer =
                factory.CreateTransformer<AMessage>(DeviceTypeEnum.Foo);

            // Act
            AMessage messageResult = transformer.Transform(new IncomingFooMessage());

            // Assert
            Assert.IsNotNull(messageResult, "Transformer failed to convert the IncomingMessage");
        }

        [TestMethod]
        public void Factory_resolves_BMessageTransformer_for_Bar_device_type()
        {
            // Arrange
            var factory = new MessageTransformFactory();
            IMessageTransformer<BMessage> transformer =
                factory.CreateTransformer<BMessage>(DeviceTypeEnum.Bar);

            // Act
            BMessage messageResult = transformer.Transform(new IncomingBarMessage());

            // Assert
            Assert.IsNotNull(messageResult, "Transformer failed to convert the IncomingMessage");
        }

        [TestMethod]
        public void Shared_factory_instance_resolves_multiple_transformers()
        {
            // Arrange
            var factory = new MessageTransformFactory();
            IMessageTransformer<AMessage> aTransformer =
                factory.CreateTransformer<AMessage>(DeviceTypeEnum.Foo);
            IMessageTransformer<BMessage> bTransformer =
                factory.CreateTransformer<BMessage>(DeviceTypeEnum.Bar);

            // Act
            AMessage aMessage = aTransformer.Transform(new IncomingFooMessage());
            BMessage bMessage = bTransformer.Transform(new IncomingBarMessage());

            // Assert
            Assert.IsNotNull(aMessage, "Transformer failed to convert the IncomingMessage");
            Assert.IsNotNull(bMessage, "Transformer failed to convert the IncomingMessage");
        }
    }
}
