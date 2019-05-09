using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.NServiceBusListener
{
    public class CreatedEventHandler : IHandleMessages<ReservationCreatedEvent>
    {
        public Task Handle(ReservationCreatedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Event recieved: {message.Id}{message.StartDate}");

            return Task.CompletedTask;
        }
    }
}
