using System;

namespace SFA.DAS.Reservations.Domain.Exceptions;
public class StartDateException : Exception
{
    public StartDateException(string errorMessage) : base(errorMessage)
    {
    }
}