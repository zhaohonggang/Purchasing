﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.Helpers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Training class
    /// </summary>
    [Authorize]
    public class TrainingController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public TrainingController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        // GET: /Training/
        public ActionResult Index()
        {
            var users = _repositoryFactory.UserRepository.Queryable.Where(a => a.WorkgroupPermissions.Any());

            return View(users);
        }

        public ActionResult Wipe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Wipe(string nothing)
        {
            TrainingDbHelper.ResetDatabase();

            return Redirect("Index");
        }

        public ActionResult Setup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Setup(List<string> userIds)
        {
            var users = new List<User>();
            users.Add(new User("anlai") {FirstName = "Alan", LastName = "Last", Email = "anlai@ucdavis.edu", IsActive = true});

            TrainingDbHelper.ConfigureDatabase("RQ", users);

            return Redirect("Index");
        }

        public JsonNetResult GetOrderCount(string userId)
        {
            var ordercount = _repositoryFactory.OrderRepository.Queryable.Count(a => a.CreatedBy.Id == userId);

            return new JsonNetResult(ordercount);
        }

    }

	/// <summary>
    /// ViewModel for the Training class
    /// </summary>
    public class TrainingViewModel
	{
		public static TrainingViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new TrainingViewModel {};
 
			return viewModel;
		}
	}
}
