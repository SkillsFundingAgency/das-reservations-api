using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.Managers;

namespace SFA.DAS.Reservations.Infrastructure.DevConfiguration
{
    public class DevUnitOfWorkContext(ILogger<string> logger) : IUnitOfWorkContext
    {
        private readonly ILogger _logger = logger;

        public void AddEvent<T>(T message) where T : class
        {
            _logger.LogInformation($"Added Event of type {typeof(T).FullName}");
        }

        public void AddEvent<T>(Func<T> messageFactory) where T : class
        {
            _logger.LogInformation($"Added Event of type {typeof(T).FullName}");
        }

        public T Find<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public T Get<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetEvents()
        {
            throw new NotImplementedException();
        }

        public void Set<T>(T value) where T : class
        {
            throw new NotImplementedException();
        }
    }

    public class DevUnitOfWorkManager : IUnitOfWorkManager
    {
        public Task BeginAsync()
        {
            return Task.CompletedTask;
        }

        public Task EndAsync(Exception ex = null)
        {
            return Task.CompletedTask;
        }
    }
}
