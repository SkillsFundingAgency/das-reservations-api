using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class ReservationWithCourseAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.ParameterType != typeof(Reservation))
            {
                throw new ArgumentException(nameof(parameter));
            }

            return new ArrangeReservationWithCourseCustomisation();
        }
    }

    public class ArrangeReservationWithCourseCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Reservation>(composer =>
                composer.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
        }
    }
}