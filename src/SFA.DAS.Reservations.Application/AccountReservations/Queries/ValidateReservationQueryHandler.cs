using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class ValidateReservationQueryHandler : IRequestHandler<ValidateReservationQuery, ValidateReservationResponse>
    {
        private readonly IAccountReservationService _reservationService;
        private readonly ICourseService _courseService;
        private readonly IValidator<ValidateReservationQuery> _validator;

        public ValidateReservationQueryHandler(IAccountReservationService reservationService,
            ICourseService courseService,
            IValidator<ValidateReservationQuery> validator)
        {
            _reservationService = reservationService;
            _courseService = courseService;
            _validator = validator;
        }

        public async Task<ValidateReservationResponse> Handle(ValidateReservationQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c=>c.Key).Aggregate((item1, item2)=> item1 + ", " + item2));
            }
            
            var reservation = await _reservationService.GetReservation(request.ReservationId);

            if (reservation == null)
            {
                throw new EntityNotFoundException<Reservation>();
            }

            if (reservation.IsLevyAccount)
            {
                return new ValidateReservationResponse
                {
                    Errors = new List<ReservationValidationError>()
                };
            }

            var reservationErrors = ValidateReservation(request, reservation);
            var courseErrors = await ValidateCourse(request, reservation);

            return new ValidateReservationResponse
            {
                Errors =  reservationErrors.Concat(courseErrors).ToList()
            };
        }
       
        private IEnumerable<ReservationValidationError> ValidateReservation(
            ValidateReservationQuery request, 
            Reservation reservation)
        {
            var errors = new List<ReservationValidationError>();

            if(reservation.Status != ReservationStatus.Change && reservation.StartDate.HasValue && reservation.ExpiryDate.HasValue && 
                (reservation.StartDate > request.StartDate || reservation.ExpiryDate < request.StartDate))
            {
                var errorMessage = "Training start date must be between the funding reservation dates " +
                                   $"{reservation.StartDate.Value:MM yyyy} to " +
                                   $"{reservation.ExpiryDate.Value:MM yyyy}";

                errors.Add(new ReservationValidationError(nameof(request.StartDate), errorMessage));
            }

            return errors;
        }

        private async Task<ICollection<ReservationValidationError>> ValidateCourse(
            ValidateReservationQuery request,
            Reservation reservation)
        {
            var errors = new List<ReservationValidationError>();

            var course = await _courseService.GetCourseById(request.CourseCode);

            if (course == null)
            {
                errors.Add(new ReservationValidationError(nameof(request.CourseCode),
                    "Selected course cannot be found"));

                return errors;
            }

            if (course.Type == ApprenticeshipType.Framework)
            {
                errors.Add(new ReservationValidationError(nameof(request.CourseCode),
                "Select an apprenticeship training course standard"));

                return errors;
            }

            var reservationDates = new ReservationDates
            {
                TrainingStartDate = request.StartDate,
                ReservationStartDate = reservation.StartDate.Value,
                ReservationExpiryDate = reservation.ExpiryDate.Value,
                ReservationCreatedDate = reservation.CreatedDate
            };

            if (course.GetActiveRules(reservationDates).Any())
            {
                errors.Add(new ReservationValidationError(nameof(request.CourseCode),
                    "Selected course has restriction rules associated with it"));
            }

            return errors;
        }
    }
}
