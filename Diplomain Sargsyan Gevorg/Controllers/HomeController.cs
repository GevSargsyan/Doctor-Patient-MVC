using Diplomain_Sargsyan_Gevorg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diplomain_Sargsyan_Gevorg.Controllers
{
    public class HomeController : Controller
    {
        private Context db;
        public HomeController(Context context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Doctors()
        {
            if (User.Claims.Any(x => x.Value == "doctor"))
            {
                ViewBag.IsDoctor = true;


            }

            if (User.Claims.Any(x => x.Value == "patient"))
            {
                ViewBag.IsDoctor = false;

            }

            var doctors = db.Doctors.Include(x=>x.User).ToList();

            return View(doctors);

        }
        [Authorize]
        [HttpPost]
        public IActionResult Doctors(string option)
        {
            if (User.Claims.Any(x => x.Value == "doctor"))
            {
                ViewBag.IsDoctor = true;


            }

            if (User.Claims.Any(x => x.Value == "patient"))
            {
                ViewBag.IsDoctor = false;

            }

            var doctors = db.Doctors.Include(x => x.User).Where(x => x.Speciality == option).ToList();
            return View(doctors);

        }


        public async Task<IActionResult> DeleteDocAsync(int id)
        {
            var doc = db.Doctors.FirstOrDefault(x => x.Id == id);
            if (doc != null)
            {
                var user = db.Patients.Include(x=>x.User).Include(x => x.Doctors).FirstOrDefault(x => x.User.Login == User.Identity.Name);
                user.Doctors.Remove(doc);
                await db.SaveChangesAsync();

            }
            return RedirectToAction("MyDocs", "Account");
        }
        public async Task<IActionResult> DeletePatientAsync(int id)
        {
            var pat = db.Patients.FirstOrDefault(x => x.Id == id);
            if (pat != null)
            {
                var user = db.Doctors.Include(x=>x.User).Include(x => x.Patients).FirstOrDefault(x => x.User.Login == User.Identity.Name);
                user.Patients.Remove(pat);
                await db.SaveChangesAsync();

            }
            return RedirectToAction("MyPatients", "Account");
        }



        public IActionResult ChatPatientAsync(int id)
        {
            //if (User.Claims.Any(x => x.Value == "doctor"))
            //{
            //    var userfrom = await db.Doctors.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
            //    ViewBag.Id = userfrom.Id;
            //    ViewBag.Mes = db.Messages.Include(x => x.Doctor).Where(x => (x.IdTo == id && x.IdFrom == userfrom.Id) || (x.IdTo == userfrom.Id && x.IdFrom == id)).ToList();

            //}
            //else if(User.Claims.Any(x => x.Value == "patient"))
            //{
            //var userfrom = await db.Patients.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
            //ViewBag.Id = userfrom.Id;

            //ViewBag.Mes = db.Messages.Where(x => (x.IdTo == id && x.IdFrom == userfrom.Id) || (x.IdTo == userfrom.Id && x.IdFrom == id)).ToList();
            ////}

            //var userto = await db.Doctors.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == id);

          
            //IHuman userto = null;


            //if (User.Claims.Any(x => x.Value == "doctor"))
            //{
               

            //  var  userfrom =  db.Doctors.Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name) ;
            //    ViewBag.Id = userfrom.User.Id;
            //    //ViewBag.Mes = db.Messages.Where(x => (x.IdTo == id && x.IdFrom == userfrom.Id) || (x.IdTo == userfrom.Id && x.IdFrom == id)).ToList();
            //    ViewBag.Mes = db.Messages.Where(x => (x.IdTo == userfrom.User.Id && x.IdFrom == id) || (x.IdTo == id && x.IdFrom == userfrom.User.Id)).ToList();
            //    userto =(IHuman)db.Patients.Include(x => x.User).FirstOrDefault(x => x.User.Id == id);
            //    ViewBag.IsDoctor = true;

            //return View(userto);
            //}
          

                var  userfrom =  db.Patients.Include(x=>x.Doctors).Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);
                ViewBag.Id = userfrom.User.Id;
                ViewBag.Mes = db.Messages.Include(x=>x.Patient).Include(x => x.User).Where(x => (x.IdTo == id && x.IdFrom == userfrom.User.Id) || (x.IdTo == userfrom.User.Id && x.IdFrom == id)).ToList();
                
                 var  userto =  db.Doctors.Include(x => x.User).FirstOrDefault(x => x.User.Id == id);
                ViewBag.IsDoctor = "0";
            ViewBag.Doctors = userfrom.Doctors.ToList();
            ViewBag.Fname=userfrom.FirstName+" "+userfrom.LastName;
                 return View(userto);
           

        





        }

        public IActionResult ChatDoctor(int id)
        {

            var userfrom = db.Doctors.Include(x => x.Patients).Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);
            ViewBag.Id = userfrom.User.Id;
            //ViewBag.Mes = db.Messages.Where(x => (x.IdTo == id && x.IdFrom == userfrom.Id) || (x.IdTo == userfrom.Id && x.IdFrom == id)).ToList();
            ViewBag.Mes = db.Messages.Include(x => x.Doctor).Include(x => x.User).Where(x => (x.IdTo == id && x.IdFrom == userfrom.User.Id) || (x.IdTo == userfrom.User.Id && x.IdFrom == id)).ToList();
           var userto = db.Patients.Include(x => x.User).FirstOrDefault(x => x.User.Id == id);
            ViewBag.IsDoctor = "1";
            ViewBag.Patients = userfrom.Patients.ToList();
            ViewBag.Fname = userfrom.FirstName + " " + userfrom.LastName;

            return View(userto);


        }

        public IActionResult DoctorPersonal(int id)
        {

            var user = db.Patients.Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);

            ViewBag.Doctor= db.Doctors.FirstOrDefault(x => x.Id == id);


            return View(user);
        }

        public IActionResult PatientPersonal(int id)
        {

            var user = db.Doctors.Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);

            ViewBag.Patient= db.Patients.FirstOrDefault(x => x.Id == id);


            return View(user);
        }


    }
}
