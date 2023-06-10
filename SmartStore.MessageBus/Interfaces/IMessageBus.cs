using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartStore.MessageBus.Interfaces
{
    public interface IMessageBus
    {
        Task PublishMessage(IEnumerable<BaseMessage> messages);
    }
}
