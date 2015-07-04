using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transformer.Messages
{
    public class IncomingBarMessage : IIncomingMessage
    {
        public DeviceTypeEnum DeviceType
        {
            get
            {
                return DeviceTypeEnum.Bar;
            }
        }
    }
}
