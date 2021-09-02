using Diplomain_Sargsyan_Gevorg.Models;
using entityentitiy.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Diplomain_Sargsyan_Gevorg.Controllers
{
    public class AccountController : Controller
    {
        private Context db;
        public AccountController(Context context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (ModelState.IsValid)
            {


                if (model.IsDoctor)
                {
                    //var user = db.Doctors.Include(x=>x.User).FirstOrDefault(u => u.User.Login == model.Login && u.User.Password == model.Password);
                    var user = db.Doctors.Include(x=>x.User).FirstOrDefault(u => u.User.Login == model.Login && u.User.Password == model.Password);
                    if (user != null)
                    {

                        await Authenticate(model.Login, user.User.IsDoctor); 


                        return RedirectToAction("Index", "Home");

                    }


                }
                if (!model.IsDoctor)
                {
                    var user = db.Patients.Include(x => x.User).FirstOrDefault(u => u.User.Login == model.Login && u.User.Password == model.Password);
                    if (user != null)
                    {
                        await Authenticate(model.Login, user.User.IsDoctor); 


                        return RedirectToAction("Index", "Home");

                    }

                }
                ModelState.AddModelError("", "Սխալ Մուտքանուն կամ Գաղտնաբառ");

            }
            return View(model);

        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (ModelState.IsValid)
            {
                if (model.file != null)
                {
                    byte[] imageData = null;
                    // считываем переданный файл в массив байтов
                    using (var binaryReader = new BinaryReader(model.file.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.file.Length);
                    }
                    // установка массива байтов
                    model.Avatar = imageData;
                }
                if (model.file == null)
                {
                    if (model.IsDoctor)
                    {

                        Image img = Image.FromFile(@"C:\Users\Gev\source\repos\entityentitiy\entityentitiy\wwwroot\images\Avatars\doc.jpg");
                        byte[] imageData = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));
                        model.Avatar = imageData;
                    }

                    if (!model.IsDoctor)
                    {

                        Image img = Image.FromFile(@"C:\Users\Gev\source\repos\entityentitiy\entityentitiy\wwwroot\images\Avatars\avatar.jpg");
                        byte[] imageData = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));
                        model.Avatar = imageData;
                    }


                }

                if (model.IsDoctor)
                {
                    User user = await db.Users.FirstOrDefaultAsync(x => x.Login == model.Login);


                    if (user == null)
                    {
                        var userr = new User { Login = model.Login, Password = model.Password, IsDoctor = model.IsDoctor };
                        db.Users.Add(userr);
                        db.Doctors.Add(new Doctor
                        {
                            User = userr,
                            Avatar = model.Avatar,
                            Speciality = model.Speciality,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Age = model.Age,
                            PhoneNumber = model.PhoneNumber,
                            Email=model.Email,
                            MiddleName=model.MiddleName



                        }) ;

                        await db.SaveChangesAsync();

                        await Authenticate(model.Login, model.IsDoctor); 


                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError("", "Некорректные логин и(или) пароль");


                }
                else if (!model.IsDoctor)
                {
                    {
                        User user = await db.Users.FirstOrDefaultAsync(x => x.Login == model.Login);


                        if (user == null)
                        {
                            var userr = new User { Login = model.Login, Password = model.Password, IsDoctor = model.IsDoctor };

                            db.Users.Add(userr);
                            db.Patients.Add(new Patient
                            {
                                User = userr,
                                Avatar = model.Avatar,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Age = model.Age,
                                PhoneNumber = model.PhoneNumber,
                                Email = model.Email,
                                MiddleName = model.MiddleName

                            }) ;

                            await db.SaveChangesAsync();

                            await Authenticate(model.Login, model.IsDoctor); 


                            return RedirectToAction("Index", "Home");
                        }
                        else
                            ModelState.AddModelError("", "This Login Is Not Available");


                    }

                }
            }

            return View(model);


        }

        private async Task Authenticate(string userName, bool isdoctor)
        {
            string docclaim = isdoctor == true ? "doctor" : "patient";

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType,userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,docclaim)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AddDoc(int id)
        {
            var patient = db.Patients.Include(x => x.Doctors).Include(x=>x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);
            var doctor = db.Doctors.FirstOrDefault(x => x.Id == id);

            if (patient != null)
            {

                patient.Doctors.Add(doctor);
                db.SaveChanges();

            }


            return RedirectToAction("MyDocs", "Account");


        }

        [Authorize]
        public IActionResult MyDocs()
        {
            if (User.Claims.Any(x => x.Value == "patient"))
            {

                var patient = db.Patients.Include(x => x.Doctors).Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);
                var docs = patient.Doctors.ToList();
                return View(docs);
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult MyPatients()
        {
            if (User.Claims.Any(x => x.Value == "doctor"))
            {

                var doc = db.Doctors.Include(x => x.Patients).Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);
                var patients = doc.Patients.ToList();
                return View(patients);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult MyPageDoctor()
        {
           
              var  user = db.Doctors.Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);
            return View(user);

        }
        public IActionResult MyPagePatient()
        {
           
              var  user = db.Patients.Include(x => x.User).FirstOrDefault(x => x.User.Login == User.Identity.Name);
            return View(user);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(string anun, string azg,string hayr, int age, string num,string mail, string spec, string log, string pas,IFormFile nkar)
        {
            if (spec == null)
            {
                var pat = await db.Patients.Include(x=>x.User).FirstOrDefaultAsync(x => x.User.Login == User.Identity.Name);

                pat.FirstName = anun;
                pat.LastName = azg;
                pat.Age = age;
                pat.PhoneNumber = num;
                pat.Speciality = spec;
                pat.User.Login = log;
                pat.User.Password = pas;
                pat.MiddleName = hayr;
                pat.Email = mail;

                db.Patients.Update(pat);
                await db.SaveChangesAsync();

                return RedirectToAction("MyPagePatient", "Account");

            }

            var current = await db.Doctors.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Login == User.Identity.Name);

            current.FirstName = anun;
            current.LastName = azg;
            current.MiddleName = hayr;
            current.Age = age;
            current.PhoneNumber = num;
            current.Email = mail;
            current.Speciality = spec;
            current.User.Login = log;
            current.User.Password = pas;

            db.Doctors.Update(current);
            await db.SaveChangesAsync();

            return RedirectToAction("MyPageDoctor", "Account");

        }


    }


}
