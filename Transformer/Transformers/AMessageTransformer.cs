using System;
using Transformer.Messages;

namespace Transformer.Transformers
{
    [Transformable(DeviceTypeEnum.Foo)]
    public class AMessageTransformer : IMessageTransformer<AMessage>
    {
        public AMessage Transform(IIncomingMessage message)
        {
            if (!(message is IncomingFooMessage))
            {
                throw new InvalidCastException("Message was not an IncomingFooMessage");
            }

            return new AMessage();
        }
    }
}
