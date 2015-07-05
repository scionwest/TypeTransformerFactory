using Transformer.Messages;

namespace Transformer.Transformers
{
    public interface IMessageTransformer
    {
    }

    public interface IMessageTransformer<TTargetType> : IMessageTransformer where TTargetType : class
    {
        TTargetType Transform(IIncomingMessage message);
    }
}
