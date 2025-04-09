using Microsoft.AspNetCore.Http;
using HTTP5125Cummulative1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HTTP5125Cummulative1.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }

        //GET : TeacherPage/List
        [HttpGet]
        [Route("[controller]/List")]
        public IActionResult List()
        {
            List<Teacher> Teachers = _api.ListTeachers();
            return View(Teachers);
        }

        //GET : TeacherPage/Show/{id}
        [HttpGet]
        [Route("[controller]/Show/{id}")]
        public IActionResult Show(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }



        // GET: TeacherPage/New
        [HttpGet]
        [Route("[controller]/New")]
        public IActionResult New()
        {
            return View();
        }


        // POST: TeacherPage/Create
        [HttpPost]
        [Route("[controller]/Create")]
        public IActionResult Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _api.AddTeacher(teacher);
                return RedirectToAction("List");
            }
            return View("New", teacher);
        }


        // GET: TeacherPage/Delete/{id}
        [HttpGet]
        [Route("[controller]/DeleteConfirm/{id}")]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        // POST: TeacherPage/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [Route("[controller]/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            _api.DeleteTeacher(id);
            return RedirectToAction("List");
        }
       
        

    }
}

