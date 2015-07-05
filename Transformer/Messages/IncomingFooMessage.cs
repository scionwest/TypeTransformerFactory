namespace Transformer.Messages
{
    public class IncomingFooMessage : IIncomingMessage
    {
        public DeviceTypeEnum DeviceType  { get { return DeviceTypeEnum.Foo; } }
    }
}
