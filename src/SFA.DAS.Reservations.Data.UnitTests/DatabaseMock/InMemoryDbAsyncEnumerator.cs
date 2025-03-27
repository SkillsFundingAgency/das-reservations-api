using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.UnitTests.DatabaseMock
{
    public class InMemoryDbAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
    {
        private bool disposed = false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }

        public T Current => enumerator.Current;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    enumerator.Dispose();
                }

                this.disposed = true;
            }
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(enumerator.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            if (!this.disposed)
            {
                enumerator.Dispose();
                this.disposed = true;
            }

            return new ValueTask();
        }
    }
}