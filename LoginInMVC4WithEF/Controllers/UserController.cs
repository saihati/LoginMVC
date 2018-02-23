﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LoginInMVC4WithEF.Models;

namespace LoginInMVC4WithEF.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(Models.Registration userr)
        {
            //if (ModelState.IsValid)
            //{
                if (IsValid(userr.Email, userr.Password))
                {
                    FormsAuthentication.SetAuthCookie(userr.Email, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login details are wrong.");
                }
            //}
            //else
            //{
            //    ModelState.AddModelError("", "Error, Check data");
            //}
            return View(userr);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Models.Registration user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new LoginInMVC4WithEF.Models.UserEntities2())
                    {
                        var crypto = new SimpleCrypto.PBKDF2();
                        var encrypPass = crypto.Compute(user.Password);
                        var newUser = db.Registrations.Create();
                        newUser.Email = user.Email;
                        newUser.Password = encrypPass;
                        newUser.PasswordSalt = crypto.Salt;
                        newUser.FirstName = user.FirstName;
                        newUser.LastName = user.LastName;
                        newUser.UserType = "User";
                        newUser.CreatedDate = DateTime.Now;
                        newUser.IsActive = true;
                        newUser.IPAddress = "642 White Hague Avenue";
                        db.Registrations.Add(newUser);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }





            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool IsValid(string email, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool IsValid = false;

            using (var db = new LoginInMVC4WithEF.Models.UserEntities2())
            {
                var user = db.Registrations.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    if (user.Password == crypto.Compute(password, user.PasswordSalt))
                    {
                        IsValid = true;
                    }
                }
            }
            return IsValid;
        }

    }
}
