using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Agenda.Infra.Padronizar.CriacaoTabelas
{
    //[DebuggerStepThrough]
    public static class ListToDatatable
    {
        private static string MsgError { get; set; } = default!;
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            try
            {
                var tb = new DataTable(typeof(T).Name);
                var _ignore = new List<string>();

                PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance) ?? throw new Exception("Não existem propriedades na entidade");

                if (!props.Any())
                    throw new Exception("Não existem propriedades na entidade");

                foreach (PropertyInfo prop in props)
                {
                    var _notMapped = prop?.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault();
                    var _attribute = prop?.GetCustomAttributes(true).FirstOrDefault();
                    if (prop != null)
                    {
                        if (_attribute != null && _notMapped == null)
                            tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        else
                            _ignore.Add(prop.Name);
                    }
                }

                foreach (var item in items)
                {
                    var values = new object[tb.Columns.Count];
                    var objVIndex = 0;
                    for (var i = 0; i < props.Length; i++)
                    {
                        var _colname = _ignore.Where(j => j == props[i].Name).FirstOrDefault();
                        if (_colname == null)
                        {
                            var _valorObj = props[i].GetValue(item, null);
                            if (_valorObj != null)
                                values[objVIndex] = _valorObj;
                            objVIndex++;
                        }

                    }
                    tb.Rows.Add(values);
                }

                return tb;
            }
            catch
            {

                return new DataTable(typeof(T).Name);
            }

        }
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            IEnumerable<T> _lista = items;
            return _lista.ToDataTable();
        }

        public static async Task<bool> MapperDatatableToPostgres<T>(this DataTable table, string connectionString, string? tableName = null)
        {
            if (table == null || table.Columns.Count == 0)
                throw new ArgumentNullException("Tabela vazia - sem colunas definidas");

            tableName ??= typeof(T).Name.ToLower(CultureInfo.InvariantCulture);

            try
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                using var writer = conn.BeginBinaryImport(GenerateCopyCommand(table, tableName));

                foreach (DataRow row in table.Rows)
                {
                    writer.StartRow();
                    foreach (DataColumn col in table.Columns)
                    {
                        writer.Write(row[col]);
                    }
                }

                await writer.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir dados em massa no PostgreSQL: {ex.Message}");
                return false;
            }
        }

        private static string GenerateCopyCommand(DataTable table, string tableName)
        {
            var columns = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => "\"" + c.ColumnName + "\""));
            return $"COPY \"{tableName}\" ({columns}) FROM STDIN (FORMAT BINARY)";
        }
    }
}
