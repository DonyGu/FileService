using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Comm100.Framework.Common
{
    public static class DbContextExtension
    {
        public static DbTransaction GetDbTransaction(this IDbContextTransaction source)
        {
            return (source as IInfrastructure<DbTransaction>).Instance;
        }

        private static DbCommand GetCommand(this DbContext context, string commandText, List<DbParameter> parameters)
        {
            var connection = context.Database.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            var command = connection.CreateCommand();
            if (context.Database.CurrentTransaction != null)
                command.Transaction = context.Database.CurrentTransaction.GetDbTransaction();

            command.CommandType = CommandType.Text;
            command.CommandText = commandText;

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }
            return command;
        }

        public static DbDataReader GetDataReader(this DbContext context, string sql)
        {
            DbDataReader ds;

            DbCommand command = context.GetCommand(sql, null);

            ds = command.ExecuteReader();

            return ds;
        }

        public static DbDataReader GetDataReader(this DbContext context, string sql, List<DbParameter> parameters)
        {
            DbDataReader ds;

            DbCommand command = context.GetCommand(sql, parameters);

            ds = command.ExecuteReader();
            return ds;
        }

        public static int Execute(this DbContext context, string sql, List<DbParameter> parameters)
        {
            DbCommand command = context.GetCommand(sql, parameters);

            return command.ExecuteNonQuery();
        }

        public static object ExecuteScalar(this DbContext context, string sql, List<DbParameter> parameters)
        {
            DbCommand command = context.GetCommand(sql, parameters);

            var toReturn = command.ExecuteScalar();

            return toReturn;
        }
    }
}
