namespace SFA.DAS.Reservations.Domain.Account
{
    public class Account(long id, bool isLevy, string name, int? reservationLimit)
    {
        public long Id { get ;  } = id;
        public bool IsLevy { get ;  } = isLevy;
        public string Name { get ;  } = name;
        public int? ReservationLimit { get;  } = reservationLimit;

        public Account(Domain.Entities.Account account, int? globalReservationLimit) : this(account.Id, account.IsLevy, account.Name, account.ReservationLimit ?? globalReservationLimit)
        {
        }
    }
}