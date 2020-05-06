using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wyklad3.Services
{
    public class OracleDbService : IDbService
    {
        private readonly string _connString =
            "Data Source=10.1.1.36;Initial Catalog=s14940;Integrated Security=True";

        private SqlConnection connection;

        public OracleDbService()
        {
            connection = new SqlConnection(_connString);
            connection.Open();
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }

        public List<object[]> Execute(SqlCommand command, bool commitTransaction = true)
        {
            var transaction = command.Transaction ?? connection.BeginTransaction();
            List<object[]> result = new List<object[]>();

            try
            {
                command.Connection = connection;
                command.Transaction ??= transaction;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    object[] temp = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        temp[i] = reader[i];
                    }

                    result.Add(temp);
                }

                reader.Close();
            }
            catch (SqlException e)
            {
                transaction.Rollback();
                Console.WriteLine(e);
                throw;
            }

            if (commitTransaction)
            {
                transaction.Commit();
            }

            return result;
        }

        public object ExecuteScalar(SqlCommand command, bool commitTransaction = true)
        {
            var transaction = command.Transaction ?? connection.BeginTransaction();

            try
            {
                command.Connection = connection;
                command.Transaction ??= transaction;

                var result = command.ExecuteScalar();

                if (commitTransaction)
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (SqlException e)
            {
                transaction.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }
    }
}