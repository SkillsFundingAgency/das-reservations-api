﻿using Newtonsoft.Json;
using NServiceBus.Persistence.Sql;
using NServiceBus.Persistence;
using NServiceBus.Settings;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Models;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System;
using SFA.DAS.NServiceBus.SqlServer.Data;

namespace SFA.DAS.Reservations.Api.AppStart
{
    public class ClientOutboxPersisterV2(IDateTimeService dateTimeService, ReadOnlySettings settings)
        : IClientOutboxStorageV2
    {
        public const string GetCommandText = "SELECT EndpointName, Operations FROM dbo.ClientOutboxData WHERE MessageId = @MessageId";

        public const string SetAsDispatchedCommandText = "UPDATE dbo.ClientOutboxData SET Dispatched = 1, DispatchedAt = @DispatchedAt, Operations = '[]' WHERE MessageId = @MessageId";

        public const string GetAwaitingDispatchCommandText = "SELECT MessageId, EndpointName FROM dbo.ClientOutboxData WHERE Dispatched = 0 AND CreatedAt <= @CreatedAt AND PersistenceVersion = '2.0.0' ORDER BY CreatedAt";

        public const string StoreCommandText = "INSERT INTO dbo.ClientOutboxData (MessageId, EndpointName, CreatedAt, PersistenceVersion, Operations) VALUES (@MessageId, @EndpointName, @CreatedAt, '2.0.0', @Operations)";

        public const string RemoveEntriesOlderThanCommandText = "DELETE TOP(@BatchSize) FROM dbo.ClientOutboxData WHERE Dispatched = 1 AND DispatchedAt < @DispatchedBefore AND PersistenceVersion = '2.0.0'";

        public const int CleanupBatchSize = 10000;

        private readonly Func<DbConnection> _connectionBuilder = settings.Get<Func<DbConnection>>("SqlPersistence.ConnectionBuilder");

        public async Task<IClientOutboxTransaction> BeginTransactionAsync()
        {
            DbConnection obj = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(continueOnCapturedContext: false);
            DbTransaction transaction = obj.BeginTransaction();
            return new SqlClientOutboxTransaction(obj, transaction);
        }

        public async Task<ClientOutboxMessageV2> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession)
        {
            ISqlStorageSession sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
            using DbCommand command = sqlStorageSession.Connection.CreateCommand();
            command.CommandText = "SELECT EndpointName, Operations FROM dbo.ClientOutboxData WHERE MessageId = @MessageId";
            command.CommandType = CommandType.Text;
            command.Transaction = sqlStorageSession.Transaction;
            command.AddParameter("MessageId", messageId);
            using DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(continueOnCapturedContext: false);
            if (await reader.ReadAsync().ConfigureAwait(continueOnCapturedContext: false))
            {
                string @string = reader.GetString(0);
                List<TransportOperation> transportOperations = JsonConvert.DeserializeObject<List<TransportOperation>>(reader.GetString(1), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                return new ClientOutboxMessageV2(messageId, @string, transportOperations);
            }

            throw new KeyNotFoundException($"Client outbox data not found where MessageId = '{messageId}'");
        }

        public async Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync()
        {
            using DbConnection connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(continueOnCapturedContext: false);
            using DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT MessageId, EndpointName FROM dbo.ClientOutboxData WHERE Dispatched = 0 AND CreatedAt <= @CreatedAt AND PersistenceVersion = '2.0.0' ORDER BY CreatedAt";
            command.CommandType = CommandType.Text;
            command.AddParameter("CreatedAt", dateTimeService.UtcNow.AddSeconds(-10.0));
            using DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(continueOnCapturedContext: false);
            List<IClientOutboxMessageAwaitingDispatch> clientOutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();
            while (await reader.ReadAsync().ConfigureAwait(continueOnCapturedContext: false))
            {
                Guid guid = reader.GetGuid(0);
                string @string = reader.GetString(1);
                ClientOutboxMessageV2 item = new ClientOutboxMessageV2(guid, @string);
                clientOutboxMessages.Add(item);
            }

            return clientOutboxMessages;
        }

        public async Task SetAsDispatchedAsync(Guid messageId)
        {
            using DbConnection connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(continueOnCapturedContext: false);
            using DbCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE dbo.ClientOutboxData SET Dispatched = 1, DispatchedAt = @DispatchedAt, Operations = '[]' WHERE MessageId = @MessageId";
            command.CommandType = CommandType.Text;
            command.AddParameter("MessageId", messageId);
            command.AddParameter("DispatchedAt", dateTimeService.UtcNow);
            await command.ExecuteNonQueryAsync().ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task SetAsDispatchedAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession)
        {
            ISqlStorageSession sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
            using DbCommand dbCommand = sqlStorageSession.Connection.CreateCommand();
            dbCommand.CommandText = "UPDATE dbo.ClientOutboxData SET Dispatched = 1, DispatchedAt = @DispatchedAt, Operations = '[]' WHERE MessageId = @MessageId";
            dbCommand.CommandType = CommandType.Text;
            dbCommand.Transaction = sqlStorageSession.Transaction;
            dbCommand.AddParameter("MessageId", messageId);
            dbCommand.AddParameter("DispatchedAt", dateTimeService.UtcNow);
            await dbCommand.ExecuteNonQueryAsync();
        }

        public async Task StoreAsync(ClientOutboxMessageV2 clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction)
        {
            SqlClientOutboxTransaction sqlClientOutboxTransaction = (SqlClientOutboxTransaction)clientOutboxTransaction;
            using DbCommand dbCommand = sqlClientOutboxTransaction.Connection.CreateCommand();
            dbCommand.CommandText = "INSERT INTO dbo.ClientOutboxData (MessageId, EndpointName, CreatedAt, PersistenceVersion, Operations) VALUES (@MessageId, @EndpointName, @CreatedAt, '2.0.0', @Operations)";
            dbCommand.CommandType = CommandType.Text;
            dbCommand.Transaction = sqlClientOutboxTransaction.Transaction;
            dbCommand.AddParameter("MessageId", clientOutboxMessage.MessageId);
            dbCommand.AddParameter("EndpointName", clientOutboxMessage.EndpointName);
            dbCommand.AddParameter("CreatedAt", dateTimeService.UtcNow);
            dbCommand.AddParameter("Operations", JsonConvert.SerializeObject(clientOutboxMessage.TransportOperations, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }));
            
            await dbCommand.ExecuteNonQueryAsync();
        }

        public async Task RemoveEntriesOlderThanAsync(DateTime oldest, CancellationToken cancellationToken)
        {
            using DbConnection connection = await _connectionBuilder.OpenConnectionAsync().ConfigureAwait(continueOnCapturedContext: false);
            bool flag = false;
            while (!cancellationToken.IsCancellationRequested && !flag)
            {
                using DbCommand command = connection.CreateCommand();
                command.CommandText = "DELETE TOP(@BatchSize) FROM dbo.ClientOutboxData WHERE Dispatched = 1 AND DispatchedAt < @DispatchedBefore AND PersistenceVersion = '2.0.0'";
                command.AddParameter("DispatchedBefore", oldest);
                command.AddParameter("BatchSize", 10000);
                flag = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false) < 10000;
            }
        }
    }
}
