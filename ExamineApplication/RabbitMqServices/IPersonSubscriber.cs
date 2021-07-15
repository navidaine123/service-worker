using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamineApplication.RabbitMqServices
{
    public interface IPersonSubscriber
    {
        Task Consume();
        void Disconnect();
    }
}
