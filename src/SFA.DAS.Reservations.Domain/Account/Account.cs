namespace SFA.DAS.Reservations.Domain.Account
{
    public class Account
    {
        public long Id { get ;  }
        public bool IsLevy { get ;  }
        public string Name { get ;  }
        public int ReservationLimit { get;  }

        public Account(Domain.Entities.Account account, int globalReservationLimit)
        {
            Id = account.Id;
            IsLevy = account.IsLevy;
            Name = account.Name;
            ReservationLimit = account.ReservationLimit ?? globalReservationLimit;
        }
        
        public Account(long id, bool isLevy, string name, int? reservationLimit)
        {
            Id = id;
            IsLevy = isLevy;
            Name = name;
            ReservationLimit = reservationLimit ?? 0;
        }

    }
}