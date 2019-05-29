using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class UserRuleNotificationRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public UserRuleNotificationRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task<UserRuleNotification> Add(UserRuleNotification userRuleNotification)
        {
            var result = await _reservationsDataContext.UserRuleNotifications.AddAsync(userRuleNotification);

            _reservationsDataContext.SaveChanges();

            return result.Entity;
        }
    }
}
