using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Api.Models
{
    public class Reservation
    {
        public long AccountId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
