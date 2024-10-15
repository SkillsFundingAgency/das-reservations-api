using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class IndexRegistryEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
