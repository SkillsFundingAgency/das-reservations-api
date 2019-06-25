using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.Reservations.Infrastructure.DevConfiguration
{
    public class DevUnitOfWorkContext : IUnitOfWorkContext
    {
        private readonly ILogger _logger;

        public DevUnitOfWorkContext(ILogger<string> logger)
        {
            _logger = logger;
        }

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
        public async Task BeginAsync()
        {
            
        }

        public async Task EndAsync(Exception ex = null)
        {
            
        }
    }
}
