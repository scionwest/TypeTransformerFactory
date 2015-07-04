using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transformer.Messages;

namespace Transformer.Transformers
{
    [Transformable(DeviceTypeEnum.Bar)]
    public class BMessageTransformer : IMessageTransformer<BMessage>
    {
        public BMessage Transform(IIncomingMessage message)
        {
            if (!(message is IncomingBarMessage))
            {
                throw new InvalidCastException("Message was not an IncomingBarMessage");
            }

            return new BMessage();
        }
    }
}
