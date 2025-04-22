using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HTTP5125Cummulative1.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HTTP5125Cummulative1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Teachers in the system
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"TeacherId":3,"TeacherFname":"Sam","TeacherLName":"Cooper", "HireDate":"20/02/2020", "salary":"23.00"},{"TeacherId":1,"TeacherFname":"Eddy","TeacherLName":"Bond", "HireDate":"20/02/2010", "salary":"29.00"},..]
        /// </example>
        /// <returns>
        /// A list of teacher objects 
        /// </returns>
        [HttpGet]
        [Route("ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            List<Teacher> Teachers = new List<Teacher>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "select * from teachers";

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId = Id,
                            TeacherFName = FirstName,
                            TeacherLName = LastName,
                            HireDate = HireDate,
                            Salary = Salary
                        };

                        Teachers.Add(CurrentTeacher);
                    }
                }
            }

            return Teachers;
        }

        /// <summary>
        /// Returns a teacher in the database by their ID
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/3 -> {"TeacherId":3,"TeacherFname":"Sam","TeacherLName":"Cooper", "HireDate":"20/02/2020", "salary":"23.00"}
        /// </example>
        /// <returns>
        /// A matching teacher object by its ID. Empty object if Teacher not found
        /// </returns>
        [HttpGet]
        [Route("FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher SelectedTeacher = new Teacher();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "select * from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                        SelectedTeacher.TeacherId = Id;
                        SelectedTeacher.TeacherFName = FirstName;
                        SelectedTeacher.TeacherLName = LastName;
                        SelectedTeacher.HireDate = HireDate;
                        SelectedTeacher.Salary = Salary;
                    }
                }
            }

            return SelectedTeacher;
        }


        /// <summary>
        /// Adds a new teacher to the database.
        /// </summary>
        /// <param name="teacher">The teacher to add.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult AddTeacher([FromBody] Teacher teacher)
        {
            if (string.IsNullOrEmpty(teacher.TeacherFName) || string.IsNullOrEmpty(teacher.TeacherLName))
            {
                return BadRequest("Teacher name cannot be empty.");
            }

            if (teacher.HireDate > DateTime.Now)
            {
                return BadRequest("Hire date cannot be in the future.");
            }

            using (var conn = _context.AccessDatabase())
            {
                conn.Open();
                var command = new MySqlCommand("INSERT INTO teachers (teacherfname, teacherlname, hiredate, salary) VALUES (@TeacherFName, @TeacherLName, @HireDate, @Salary)", conn);
                command.Parameters.AddWithValue("@TeacherFName", teacher.TeacherFName);
                command.Parameters.AddWithValue("@TeacherLName", teacher.TeacherLName);
                command.Parameters.AddWithValue("@HireDate", teacher.HireDate);
                command.Parameters.AddWithValue("@Salary", teacher.Salary);
                command.ExecuteNonQuery();
            }

            return Ok("Teacher added successfully.");
        }



        /// <summary>
        /// Deletes a teacher from the database.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            using (var conn = _context.AccessDatabase())
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM teachers WHERE teacherid = @TeacherId", conn);
                cmd.Parameters.AddWithValue("@TeacherId", id);
                var rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound("Teacher not found.");
                }
            }

            return Ok("Teacher deleted successfully.");
        }


        /// <summary>
        /// Updates an existing teacher in the database.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="teacher">The updated teacher object.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                return BadRequest("Teacher ID mismatch.");
            }

            if (string.IsNullOrEmpty(teacher.TeacherFName) || string.IsNullOrEmpty(teacher.TeacherLName))
            {
                return BadRequest("Teacher name cannot be empty.");
            }

            if (teacher.HireDate > DateTime.Now)
            {
                return BadRequest("Hire date cannot be in the future.");
            }

            if (teacher.Salary < 0)
            {
                return BadRequest("Salary cannot be less than 0.");
            }

            using (var conn = _context.AccessDatabase())
            {
                conn.Open();
                var command = new MySqlCommand(
                    "UPDATE teachers SET teacherfname = @TeacherFName, teacherlname = @TeacherLName, hiredate = @HireDate, salary = @Salary WHERE teacherid = @TeacherId",
                    conn
                );
                command.Parameters.AddWithValue("@TeacherId", teacher.TeacherId);
                command.Parameters.AddWithValue("@TeacherFName", teacher.TeacherFName);
                command.Parameters.AddWithValue("@TeacherLName", teacher.TeacherLName);
                command.Parameters.AddWithValue("@HireDate", teacher.HireDate);
                command.Parameters.AddWithValue("@Salary", teacher.Salary);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound("Teacher not found.");
                }
            }

            return Ok("Teacher updated successfully.");
        }


    }
}



 