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
        string conString = "Data Source=10.1.1.36;Initial Catalog=s14940;Integrated Security=True";

        public StudentsController(IDbService service)
        {
            _dbService = service;
        }

        //2. QueryString
        [HttpGet]
        public IActionResult GetStudents([FromServices]IDbService service)
        {
            var list = new List<StudentInfoDTO>();

            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand com = new SqlCommand())
            {


                com.Connection = con;
                com.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name, e.Semester from Student s join Enrollment e on e.IdEnrollment = s.IdEnrollment join Studies st on st.IdStudy = e.IdStudy";
                con.Open();

                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new StudentInfoDTO
                    {
                        
                        LastName = dr["LastName"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        Date = dr["BirthDate"].ToString(),
                        Name = dr["Name"].ToString(),
                        Semester = dr["Semester"].ToString()
                        
                    };
                    list.Add(st);
                }
            }
            return Ok(list);
        }

        //[FromRoute], [FromBody], [FromQuery]
        //1. URL segment
        [HttpGet("{id}")]
        public IActionResult GetStudent([FromRoute]int id) //action method
        {
            if (id == 1)
            {
                return Ok("Jan");
            }

            return NotFound("Student was not found");
        }

        //3. Body - cialo zadan
        [HttpPost]
        public IActionResult CreateStudent([FromBody]Student student)
        {
            student.IndexNumber=$"s{new Random().Next(1, 20000)}";
            //...
            return Ok(student); //JSON
        }


    }
}