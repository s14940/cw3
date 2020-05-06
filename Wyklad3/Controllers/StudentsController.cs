using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wyklad3.Models;
using Wyklad3.Services;
using System.Data.SqlClient;

namespace Wyklad3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private IDbService _dbService;
        string connString = "Data Source=10.1.1.36;Initial Catalog=s14940;Integrated Security=True";

        public StudentsController(IDbService service)
        {
            _dbService = service;
        }


        [HttpGet("{index}")]
        public IActionResult GetStudent(string index)
        {
            List<Student> result = new List<Student>();

            using SqlConnection con = new SqlConnection(connString);
            using SqlCommand com = new SqlCommand
            {
                Connection = con,
                CommandText =
                    "select s.FirstName, s.LastName, s.BirthDate from Student s where s.IndexNumber=@index"
            };
            com.Parameters.AddWithValue("index", index);

            con.Open();
            SqlDataReader dataReader = com.ExecuteReader();

            while (dataReader.Read())
            {
                var student = new Student
                {
                    FirstName = dataReader["FirstName"].ToString(),
                    LastName = dataReader["LastName"].ToString(),
                    // IndexNumber = dataReader["IndexNumber"].ToString(),
                    IndexNumber = ""
                };
                result.Add(student);
            }


            if (result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("Nie znaleziono");
            }
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<StudentInfoDTO>();


            using SqlConnection con = new SqlConnection(connString);
            using SqlCommand com = new SqlCommand
            {
                Connection = con,
                CommandText =
                    "select s.FirstName, s.LastName, s.BirthDate, st.Name, e.Semester from Student s join Enrollment e on e.IdEnrollment_ = s.IdEnrollment join Studies st on st.IdStudy = e.IdStudy"
            };

            con.Open();
            SqlDataReader dataReader = com.ExecuteReader();

            while (dataReader.Read())
            {
                var student = new StudentInfoDTO
                {
                    BirthDate = dataReader["BirthDate"].ToString(),
                    FirstName = dataReader["FirstName"].ToString(),
                    LastName = dataReader["LastName"].ToString(),
                    Semester = dataReader["Semester"].ToString(),
                    Name = dataReader["Name"].ToString(),
                };
                list.Add(student);
            }

            return Ok(list);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja zakończona");
        }


        [HttpDelete]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie zakończone");
        }
    }
}