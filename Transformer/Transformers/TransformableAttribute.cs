using System;
using Transformer.Messages;

namespace Transformer.Transformers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TransformableAttribute : Attribute
    {
        public TransformableAttribute(DeviceTypeEnum deviceType)
        {
            this.DeviceType = deviceType;
        }

        public DeviceTypeEnum DeviceType { get; private set; }
    }
}
