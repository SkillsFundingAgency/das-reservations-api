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
            Console.WriteLine("ReservationCreatedEvent event received:");
            Console.WriteLine($"id:{message.Id}");
            Console.WriteLine($"Start Date:{message.StartDate}");
            Console.WriteLine($"Legal Entity Name:{message.AccountLegalEntityName}");
            Console.WriteLine();

            return Task.CompletedTask;
        }
    }
}
