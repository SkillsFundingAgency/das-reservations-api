using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class IndexRegistryEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
