using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wyklad3.Models;

namespace Wyklad3.Services
{

    public interface IDbService
    {
        public List<object[]> Execute(SqlCommand command, bool commitTransaction = true);
        public object ExecuteScalar(SqlCommand command, bool commitTransaction = true);

        public SqlConnection GetConnection();

    }
}
