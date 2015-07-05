namespace Transformer.Messages
{
    public class IncomingBarMessage : IIncomingMessage
    {
        public DeviceTypeEnum DeviceType { get { return DeviceTypeEnum.Bar; } }
    }
}
