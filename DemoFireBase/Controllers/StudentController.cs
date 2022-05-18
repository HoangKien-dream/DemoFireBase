using DemoFireBase.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoFireBase.Controllers
{
    public class StudentController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "uaDEYfryKIhw6msNC0kMFyTLYvxAN7nDCsfO1orr",
            BasePath = "https://demofirebasemvc-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        // GET: Student
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Student");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Student>();
            foreach(var student in data)
            {
                list.Add(JsonConvert.DeserializeObject<Student>(((JProperty)student).Value.ToString()));
            }
            return View(list);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Student student)
        {
            try
            {
                if(student.name != null && student.address != null)
                {
                    AddStudentToFireBase(student);
                    ModelState.AddModelError(string.Empty, "Added Successfully");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Added Failed");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

            }
           
            return View();
        }

        private void AddStudentToFireBase(Student student)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = student;
            PushResponse response = client.Push("Student/", data);
            data.id = response.Result.name;
            SetResponse setResponse = client.Set("Student/" + data.id, data);
           
        }

        [HttpGet]
        public ActionResult GetDetail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Student/"+id);
            Student data = JsonConvert.DeserializeObject<Student>(response.Body);  
            return View(data);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Student/" + id);
            Student data = JsonConvert.DeserializeObject<Student>(response.Body);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Student student)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Set("Student/" + student.id,student);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Student/" + id);
            return RedirectToAction("Index");
        }
    }
}