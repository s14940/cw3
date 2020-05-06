using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Wyklad3.Models;
using Wyklad3.Services;


namespace Wyklad3.Controllers

{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IDbService _dbService;

        public EnrollmentController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult EnrollStudent(Enrollment enrollment)
        {
            if (
                !string.IsNullOrEmpty(enrollment.FirstName) &&
                !string.IsNullOrEmpty(enrollment.LastName) &&
                !string.IsNullOrEmpty(enrollment.BirthDate) &&
                !string.IsNullOrEmpty(enrollment.IndexNumber) &&
                !string.IsNullOrEmpty(enrollment.Studies)
            )
            {
                try
                {
                    var command = new SqlCommand()
                    {
                        CommandText = "select s.IdStudy from Studies s where s.Name=@studies"
                    };
                    command.Parameters.AddWithValue("Studies", enrollment.Studies);

                    List<object[]> result = _dbService.Execute(command);


                    if (result.Count == 0)
                    {
                        return BadRequest("Wrong studies name");
                    }

                    var idStudy = result[0][0];


                    command = new SqlCommand()
                    {
                        CommandText = "select * from Student s where s.IndexNumber=@indexNumber"
                    };
                    command.Parameters.AddWithValue("indexNumber", enrollment.IndexNumber);

                    if (_dbService.Execute(command).Count == 0)
                    {
                        command = new SqlCommand()
                        {
                            CommandText =
                                "select * from Enrollment e JOIN Student s ON e.IdEnrollment=s.IdEnrollment where e.Semester=1 and e.IdStudy=1 and IndexNumber=@indexNumber",
                        };

                        command.Parameters.AddWithValue("idStudy", idStudy);
                        command.Parameters.AddWithValue("indexNumber", enrollment.IndexNumber);

                        result = _dbService.Execute(command);

                        if (result.Count == 0)
                        {
                            var transaction = _dbService.GetConnection().BeginTransaction();

                            command = new SqlCommand()
                            {
                                CommandText =
                                    "INSERT INTO Enrollment(StartDate, IdStudy, Semester) VALUES (@startDate, @idStudy, @semester);SELECT SCOPE_IDENTITY();",
                                Transaction = transaction
                            };

                            command.Parameters.AddWithValue("startDate",
                                SqlDateTime.Parse(DateTime.Now.ToLongDateString()));
                            command.Parameters.AddWithValue("idStudy", idStudy);
                            command.Parameters.AddWithValue("semester", 1);

                            int idEnrollment = Convert.ToInt32(_dbService.ExecuteScalar(command, false).ToString());

                            command = new SqlCommand()
                            {
                                CommandText =
                                    "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@index, @firstName, @lastName, @birthDate, @idEnrollment)",
                                Transaction = transaction
                            };

                            command.Parameters.AddWithValue("index", enrollment.IndexNumber);
                            command.Parameters.AddWithValue("firstName", enrollment.FirstName);
                            command.Parameters.AddWithValue("lastName", enrollment.LastName);
                            command.Parameters.AddWithValue("birthDate", enrollment.BirthDate);
                            command.Parameters.AddWithValue("idEnrollment", idEnrollment);

                            _dbService.ExecuteScalar(command, false);

                            transaction.Commit();

                            return Created("", enrollment);
                        }
                        else
                        {
                            return BadRequest("Student already enrolled");
                        }
                    }
                    else
                    {
                        return BadRequest("Index taken");
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                return BadRequest("All fields are required");
            }
        }

        [HttpPost]
        public IActionResult Promotions(Promotion promotion)
        {
            if (promotion.Semester >= 0 && string.IsNullOrEmpty(promotion.Studies))
            {
                var command = new SqlCommand()
                {
                    CommandText = "SELECT s.IdStudy FROM Studies s WHERE s.Name = @studyName"
                };
                command.Parameters.AddWithValue("studyName", promotion.Studies);

                int idStudy = Convert.ToInt32(_dbService.ExecuteScalar(command).ToString());

                command = new SqlCommand()
                {
                    CommandText = "SELECT * FROM Enrollment e WHERE e.Semester = @semester and e.IdStudy = @idStudy"
                };
                command.Parameters.AddWithValue("semester", promotion.Semester);
                command.Parameters.AddWithValue("Studies", idStudy);

                if (_dbService.Execute(command).Count > 0)
                {
                    command = new SqlCommand()
                    {
                        CommandText = "sp_promote_students",
                        CommandType = CommandType.StoredProcedure,
                    };

                    command.Parameters.AddWithValue("@Studies", idStudy);
                    command.Parameters.AddWithValue("@Semester", promotion.Semester);

                    _dbService.ExecuteScalar(command);

                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }
    }
}