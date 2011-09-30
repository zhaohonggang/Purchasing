﻿using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;
using System.Collections.Generic;
using UCDArch.Web.ActionResults;
using Purchasing.Web.Utility;
using System.Web.UI.WebControls;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    public class WorkgroupController : ApplicationController
    {
	    private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<User> _userRepository;

        public WorkgroupController(IRepository<Workgroup> workgroupRepository, IRepository<User> userRepository)
        {
            _workgroupRepository = workgroupRepository;
            _userRepository = userRepository;
        }

        #region Workgroup Actions
        //
        // GET: /Workgroup/
        public ActionResult Index()
        {
            var person =
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).Single();

            var orgIds = person.Organizations.Select(x => x.Id).ToArray();

            var workgroupList =
                _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

            return View(workgroupList.ToList());
        }

        public ActionResult Create()
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = WorkgroupModifyModel.Create(Repository, user);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Workgroup workgroup, string[] selectedOrganizations)
        {
            if (!ModelState.IsValid)
            {
                var model = WorkgroupModifyModel.Create(Repository, GetCurrentUser());
                model.Workgroup = workgroup;

                return View(model);
            }

            var workgroupToCreate = new Workgroup();

            Mapper.Map(workgroup, workgroupToCreate);

            if (selectedOrganizations != null)
            {
                workgroupToCreate.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToCreate.Organizations.Contains(workgroupToCreate.PrimaryOrganization))
            {
                workgroupToCreate.Organizations.Add(workgroupToCreate.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToCreate);

            Message = string.Format("{0} workgroup was created",
                                    workgroup.Name);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            //var workgroup = _workgroupRepository.GetNullableById(Id);

            //var model = new WorkgroupViewModel
            //{
            //    Workgroup = workgroup
            //};

            //return View(model);

            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return RedirectToAction("Index");
            }

            var viewModel = WorkgroupDetailsViewModel.Create(Repository, workgroup);

            return View(viewModel);

        }

        public ActionResult Edit(int id)
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = WorkgroupModifyModel.Create(Repository, user, workgroup);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Workgroup workgroup, string[] selectedOrganizations)
        {

            if (!ModelState.IsValid)
            {
                return View(new WorkgroupModifyModel { Workgroup = workgroup });
            }

            var workgroupToEdit = _workgroupRepository.GetNullableById(workgroup.Id);

            Mapper.Map(workgroup, workgroupToEdit);

            if (selectedOrganizations != null)
            {
                workgroupToEdit.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToEdit.Organizations.Contains(workgroupToEdit.PrimaryOrganization))
            {
                workgroupToEdit.Organizations.Add(workgroupToEdit.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToEdit);

            Message = string.Format("{0} was modified successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = new WorkgroupViewModel
            {
                Workgroup = workgroup
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(WorkgroupViewModel workgroupViewModel)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupViewModel.Workgroup.Id);

            workgroup.IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);

            Message = string.Format("{0} was disabled successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }
        #endregion

        #region Workgroup Accounts
        #endregion

        #region Workgroup Vendors
        #endregion

        #region Addresses
        #endregion

        #region Workgroup People
        #endregion

        #region Private Helpers

        #endregion

        #region Ajax Helpers
        public JsonNetResult SearchOrganizations(string searchTerm)
        {
            var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        }
        #endregion
    }

    /// <summary>
    /// ModifyModel for the Workgroup class
    /// </summary>
    public class WorkgroupModifyModel
    {
        public Workgroup Workgroup { get; set; }
        public List<ListItem> Organizations { get; set; } 

        public static WorkgroupModifyModel Create(IRepository repository, User user, Workgroup workgroup = null)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var modifyModel = new WorkgroupModifyModel { Workgroup = workgroup ?? new Workgroup() };

            modifyModel.Organizations = workgroup != null ? workgroup.Organizations.Select(x => new ListItem(x.Name, x.Id, true) {Selected = true}).ToList() : new List<ListItem>();

            var userOrgs = user.Organizations.Where(x => !modifyModel.Organizations.Select(y => y.Value).Contains(x.Id));
            modifyModel.Organizations.AddRange(userOrgs.Select(x => new ListItem(x.Name, x.Id, true)));

            return modifyModel;
        }
    }

	/// <summary>
    /// ViewModel for the Workgroup class
    /// </summary>
    public class WorkgroupViewModel
	{
		public Workgroup Workgroup { get; set; }
 
		public static WorkgroupViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new WorkgroupViewModel {Workgroup = new Workgroup()};
 
			return viewModel;
		}
	}

    public class WorkgroupDetailsViewModel
    {
        public Workgroup Workgroup { get; set; }
        public virtual int OrganizationCount { get; set; }
        public virtual int AccountCount { get; set; }
        public virtual int VendorCount { get; set; }
        public virtual int AddressCount { get; set; }
        public virtual int UserCount { get; set; }
        public virtual int ApproverCount { get; set; }
        public virtual int AccountManagerCount { get; set; }
        public virtual int PurchaserCount { get; set; }

        public static WorkgroupDetailsViewModel Create(IRepository repository, Workgroup workgroup)
        {
            Check.Require(repository != null, "Repository is required.");

            var workgroupPermsByGroup = (from wp in repository.OfType<WorkgroupPermission>().Queryable
                                         where wp.Workgroup.Id == workgroup.Id
                                         group wp.Role by wp.Role.Id
                                             into role
                                             select new { count = role.Count(), name = role.Key }).ToList();

            var viewModel = new WorkgroupDetailsViewModel()
                                {
                                    Workgroup = workgroup,
                                    OrganizationCount = workgroup.Organizations.Count(),
                                    AccountCount = workgroup.Accounts.Count(),
                                    VendorCount = workgroup.Vendors.Count(),
                                    AddressCount = workgroup.Addresses.Count(),
                                    UserCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.Requester).Select(x => x.count).
                                        SingleOrDefault(),
                                    ApproverCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.Approver).Select(x => x.count)
                                        .SingleOrDefault(),
                                    AccountManagerCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.AccountManager).Select(
                                            x => x.count).SingleOrDefault(),
                                    PurchaserCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.Purchaser).Select(x => x.count)
                                        .SingleOrDefault()
                                };

            return viewModel;
        }
    }
}
