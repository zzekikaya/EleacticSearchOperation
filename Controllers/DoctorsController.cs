using CRUD_Operations;
using CRUD_Operations.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AdminPanelTutorial
{
    public class DoctorsController : Controller
    {
        ElasticContext elaticClient = new ElasticContext();
        public ActionResult Index()
        {
            var result = elaticClient.GetAllDoctorsTask();
            return View(result.Result);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateDoctor(Doctors doctor)
        {
            doctor.Id = Guid.NewGuid();
            elaticClient.InsertDoctor(doctor);
            return RedirectToAction("Index", "Doctors");
        }
        [HttpPost]
        public bool Delete(Guid id)
        {
            try
            {
                elaticClient.Delete(id.ToString());
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        public ActionResult Update(Guid id)
        {
            var doctor = elaticClient.Get(id.ToString());
            return View(doctor.Result.First());
        }
        [HttpPost]
        public ActionResult UpdateDoctor(Doctors doctor)
        {

            elaticClient.InsertUpdate(doctor);
            return RedirectToAction("Index", "Doctors");
        }
    }
}