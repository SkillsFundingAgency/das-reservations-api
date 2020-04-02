namespace SFA.DAS.Reservations.Domain.Account
{
    public class Account
    {
        public long Id { get ; set ; }
        public bool IsLevy { get ; set ; }
        public string Name { get ; set ; }
        public int? ReservationLimit { get; set; }

        public Account(long id, bool isLevy, string name, int? reservationLimit)
        {
            Id = id;
            IsLevy = isLevy;
            Name = name;
            ReservationLimit = reservationLimit;
        }

        public Account()
        {
                
        }
    }
}