﻿using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the User class
    /// </summary>
    public class UserController : ApplicationController
    {
	    private readonly IRepositoryWithTypedId<User,string> _userRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;
        private readonly IRepositoryWithTypedId<ColumnPreferences, string> _columnPreferencesRepository; 

        public UserController(IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository, IRepositoryWithTypedId<ColumnPreferences, string> columnPreferencesRepository )
        {
            _userRepository = userRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
            _columnPreferencesRepository = columnPreferencesRepository;
        }

        //
        // GET: /User/Profile
        /// <summary>
        /// Return the profile for the current user
        /// </summary>
        /// <returns></returns>
        public ActionResult Profile()
        {
            var user = GetCurrent();

            if (user == null)
            {
                ErrorMessage = "You do not have an account in this system.  Please contact application support.";
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        public ActionResult EmailPreferences(string id)
        {
            var userEmailPreferences = _emailPreferencesRepository.GetNullableById(id) ?? new EmailPreferences(id);

            return View(userEmailPreferences);
        }

        [HttpPost]
        public ActionResult EmailPreferences(EmailPreferences emailPreferences)
        {
            Check.Require(emailPreferences.Id == CurrentUser.Identity.Name,
                          string.Format("User {0} attempted to save the email preferences for {1}",
                                        CurrentUser.Identity.Name, emailPreferences.Id));

            if (!ModelState.IsValid)
            {
                return View(emailPreferences);
            }

            Message = "Your email preferences have been updated";

            _emailPreferencesRepository.EnsurePersistent(emailPreferences);

            return RedirectToAction("Profile");
        }

        public ActionResult AwayStatus(string id)
        {
            var user = GetCurrent();

            return View(user);
        }
        
        [HttpPost]
        public ActionResult AwayStatus(User user)
        {
            var currentUser = GetCurrent();
            currentUser.AwayUntil = user.AwayUntil;

            Message = "Your away status has been set";

            _userRepository.EnsurePersistent(currentUser);

            return RedirectToAction("Profile");
        }

        public ActionResult ColumnPreferences(string id)
        {
            var columnPreferences = _columnPreferencesRepository.GetNullableById(id) ?? new ColumnPreferences(id);

            return View(columnPreferences);
        }

        [HttpPost]
        public ActionResult ColumnPreferences(ColumnPreferences columnPreferences)
        {
            Check.Require(columnPreferences.Id == CurrentUser.Identity.Name,
                         string.Format("User {0} attempted to save the column preferences for {1}",
                                       CurrentUser.Identity.Name, columnPreferences.Id));

            if (!ModelState.IsValid)
            {
                return View(columnPreferences);
            }

            Message = "Your column preferences have been updated";

            _columnPreferencesRepository.EnsurePersistent(columnPreferences);
            
            return RedirectToAction("Profile");

        }

        private User GetCurrent()
        {
            return _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).SingleOrDefault();
        }
    }
}