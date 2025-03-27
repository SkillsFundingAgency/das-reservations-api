using System;

namespace SFA.DAS.Reservations.Domain.Exceptions;
public class StartDateException(string errorMessage) : Exception(errorMessage);