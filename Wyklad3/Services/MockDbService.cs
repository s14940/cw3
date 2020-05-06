using System.Collections.Generic;
using Wyklad3.Models;

using System.Data.SqlClient;
using System.Globalization;


namespace Wyklad3.Services
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;
        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student {FirstName = "Jan", LastName = "Nowak"},
                new Student {FirstName = "Jan", LastName = "Kowalski"},
                new Student {FirstName = "Zbigniew", LastName = "Nowak"},
            };
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public List<object[]> Execute(SqlCommand command, bool commitTransaction = true)
        {
            throw new System.NotImplementedException();
        }

        public object ExecuteScalar(SqlCommand command, bool commitTransaction = true)
        {
            throw new System.NotImplementedException();
        }

        public SqlConnection GetConnection()
        {
            throw new System.NotImplementedException();
        }
    }
}