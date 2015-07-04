using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transformer.Messages;

namespace Transformer.Transformers
{
    public interface IMessageTransformer
    {
    }

    public interface IMessageTransformer<T> : IMessageTransformer where T : class
    {
        T Transform(IIncomingMessage message);
    }
}
