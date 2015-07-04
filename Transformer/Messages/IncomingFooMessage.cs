using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transformer.Messages
{
    public class IncomingFooMessage : IIncomingMessage
    {
        public DeviceTypeEnum DeviceType
        {
            get
            {
                return DeviceTypeEnum.Foo;
            }
        }
    }
}
