using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public class QueueMonitor
    {
        public string QueueName { get; }

        public bool? IsHealthy { get; set; }
        public string QueueErrorMessage { get; }
        public string Environment { get; }

        public QueueMonitor(string queueName, bool? isHealthy, string environment)
        {
            QueueName = queueName;
            IsHealthy = isHealthy;
            Environment = environment;
            QueueErrorMessage = $"`{QueueName}` in *{environment}* has entered an error state";
        }

    }
}
