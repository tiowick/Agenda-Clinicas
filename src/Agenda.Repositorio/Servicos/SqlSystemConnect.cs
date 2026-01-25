using Agenda.Dominio.Reflection;
using Dapper;
using Npgsql;
using System.Data;

namespace Agenda.Repositorio.Servicos
{
    public class SqlSystemConnect
    {
        private readonly string _connectionString = default!;

        public SqlSystemConnect(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "A string de conexão não pode ser nula ou vazia.");

            _connectionString = connectionString;
        }
        private bool ValidateStringConnection()
        {
            return !string.IsNullOrWhiteSpace(_connectionString);
        }

        public void Execute(string queryCommand, int commandTimeout = 1440)
        {
            try
            {
                if (!ValidateStringConnection())
                    throw new Exception("String de conexão inválida.");

                queryCommand ??= "";

                // <--- MUDANÇA: Conexão criada e descartada localmente
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Execute(queryCommand, commandTimeout: commandTimeout);
            }
            catch (NpgsqlException ex) { throw new TratamentoExcecao(ex.Message); }
            catch (Exception ex) { throw new TratamentoExcecao(ex.Message); }
        }
        public IEnumerable<TEntity> Query<TEntity>(string queryCommand, bool buffered = true, int commandTimeout = 1440)
        {
            try
            {
                if (!ValidateStringConnection())
                    throw new Exception("String de conexão inválida.");

                queryCommand ??= "";

                using var connection = new NpgsqlConnection(_connectionString);
                return connection.Query<TEntity>(queryCommand, commandTimeout: commandTimeout);
            }
            catch (NpgsqlException ex) { throw new TratamentoExcecao(ex.Message); }
            catch (Exception ex) { throw new TratamentoExcecao(ex.Message); }
        }
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string queryCommand, int commandTimeout = 1440)
        {
            try
            {
                if (!ValidateStringConnection())
                    throw new Exception("String de conexão inválida.");

                queryCommand ??= "";
                await using var connection = new NpgsqlConnection(_connectionString);
                return await connection.QueryAsync<TEntity>(queryCommand, commandTimeout: commandTimeout).ConfigureAwait(true);
            }
            catch (NpgsqlException ex) { throw new TratamentoExcecao(ex.Message); }
            catch (Exception ex) { throw new TratamentoExcecao(ex.Message); }
        }

        // MUDANÇA: Corrigido para usar 'await using'
        public async Task<int> ExecuteAsync(string queryCommand, int commandTimeout = 1440)
        {
            try
            {
                if (!ValidateStringConnection())
                    throw new Exception("String de conexão inválida.");

                queryCommand ??= "";

                await using var connection = new NpgsqlConnection(_connectionString);
                return await connection.ExecuteAsync(queryCommand, commandTimeout: commandTimeout).ConfigureAwait(false);
            }
            catch (NpgsqlException ex) { throw new TratamentoExcecao(ex.Message); }
            catch (Exception ex) { throw new TratamentoExcecao(ex.Message); }
        }
        public async Task<IEnumerable<TEntity>> QueryComParametrosAsync<TEntity>(string queryCommand, object param, CommandType commandType = CommandType.Text, int commandTimeout = 1440)
        {
            try
            {
                if (!ValidateStringConnection())
                    throw new Exception("String de conexão inválida.");

                if (string.IsNullOrWhiteSpace(queryCommand))
                    throw new ArgumentException("Query não pode ser vazia", nameof(queryCommand));

                await using var connection = new NpgsqlConnection(_connectionString); // <--- MUDANÇA

                return await connection.QueryAsync<TEntity>(
                    sql: queryCommand,
                    param: param,
                    commandType: commandType,
                    commandTimeout: commandTimeout
                ).ConfigureAwait(false);
            }
            catch (NpgsqlException ex) { throw new TratamentoExcecao(ex.Message); }
            catch (Exception ex) { throw new TratamentoExcecao(ex.Message); }
        }

        // MUDANÇA: Corrigido para usar 'await using'
        internal async Task<T> QuerySingleOrDefaultAsync<T>(string query, object? param = null, CommandType commandType = CommandType.Text, int commandTimeout = 1440)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("O comando SQL não pode ser nulo ou vazio.", nameof(query));

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString); // <--- MUDANÇA

                var result = await connection.QuerySingleOrDefaultAsync<T>(
                    query, param, commandType: commandType, commandTimeout: commandTimeout
                ).ConfigureAwait(false);

                return result;
            }
            catch (NpgsqlException ex) { throw new TratamentoExcecao($"Erro ao executar a consulta: {ex.Message}"); }
            catch (Exception ex) { throw new TratamentoExcecao($"Erro desconhecido: {ex.Message}"); }
        }
    }
}