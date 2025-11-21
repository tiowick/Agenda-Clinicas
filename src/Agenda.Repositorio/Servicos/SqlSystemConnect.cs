using Agenda.Dominio.Reflection;
using Dapper;
using Npgsql;
using System.Data;

// Removido namespace CRM_Repositorio.Servico para bater com o BaseRepositorio
namespace Agenda.Repositorio.Servicos
{
    // <--- MUDANÇA: Removido IDisposable, não é mais necessário
    public class SqlSystemConnect
    {
        private readonly string _connectionString = default!;

        public SqlSystemConnect(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "A string de conexão não pode ser nula ou vazia.");

            _connectionString = connectionString;
        }

        // Método de validação simplificado
        private bool ValidateStringConnection()
        {
            // A validação real já foi feita no construtor
            // Se precisar de mais lógica, pode adicionar aqui.
            return !string.IsNullOrWhiteSpace(_connectionString);
        }

        // MUDANÇA: Método síncrono corrigido para usar 'using'
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
            // <--- MUDANÇA: Captura NpgsqlException
            catch (NpgsqlException ex) { throw new TratamentoExcecao(ex.Message); }
            catch (Exception ex) { throw new TratamentoExcecao(ex.Message); }
        }

        // MUDANÇA: Método síncrono corrigido para usar 'using'
        public IEnumerable<TEntity> Query<TEntity>(string queryCommand, bool buffered = true, int commandTimeout = 1440)
        {
            try
            {
                if (!ValidateStringConnection())
                    throw new Exception("String de conexão inválida.");

                queryCommand ??= "";

                // <--- MUDANÇA: Conexão criada e descartada localmente
                using var connection = new NpgsqlConnection(_connectionString);
                return connection.Query<TEntity>(queryCommand, commandTimeout: commandTimeout);
            }
            catch (NpgsqlException ex) { throw new TratamentoExcecao(ex.Message); }
            catch (Exception ex) { throw new TratamentoExcecao(ex.Message); }
        }

        // MUDANÇA: Corrigido para usar 'await using'
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string queryCommand, int commandTimeout = 1440)
        {
            try
            {
                if (!ValidateStringConnection())
                    throw new Exception("String de conexão inválida.");

                queryCommand ??= "";

                // <--- MUDANÇA: Conexão assíncrona criada e descartada localmente
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

        // Este método já estava quase correto, só troquei a Conexão
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
        internal async Task<T> QuerySingleOrDefaultAsync<T>(string query, object param = null, CommandType commandType = CommandType.Text, int commandTimeout = 1440)
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

        // Removi os métodos duplicados (Query2, QueryAsync2) e o ExecuteCommandNonQuery (que era para SqlServer)
        // Removi o Dispose(), pois a classe não gerencia mais a conexão.



    }
}