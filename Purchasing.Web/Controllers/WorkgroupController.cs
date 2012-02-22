﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Attributes;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;
using System.Collections.Generic;
using UCDArch.Web.ActionResults;
using Purchasing.Web.Utility;
using MvcContrib;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
    [AuthorizeWorkgroupAccess]
    public class WorkgroupController : ApplicationController
    {
	    private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly ISecurityService _securityService;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepository<WorkgroupVendor> _workgroupVendorRepository;
        private readonly IRepositoryWithTypedId<Vendor, string> _vendorRepository;
        private readonly IRepositoryWithTypedId<VendorAddress, Guid> _vendorAddressRepository;
        private readonly IRepositoryWithTypedId<State, string> _stateRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IWorkgroupAddressService _workgroupAddressService;
        private readonly IWorkgroupService _workgroupService;

        public WorkgroupController(IRepository<Workgroup> workgroupRepository, 
            IRepositoryWithTypedId<User, string> userRepository, 
            IRepositoryWithTypedId<Role, string> roleRepository, 
            IRepository<WorkgroupPermission> workgroupPermissionRepository,
            ISecurityService securityService, IDirectorySearchService searchService,
            IRepository<WorkgroupVendor> workgroupVendorRepository, 
            IRepositoryWithTypedId<Vendor, string> vendorRepository, 
            IRepositoryWithTypedId<VendorAddress, Guid> vendorAddressRepository,
            IRepositoryWithTypedId<State, string> stateRepository,
            IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository, 
            IRepository<WorkgroupAccount> workgroupAccountRepository,
            IWorkgroupAddressService workgroupAddressService,
            IWorkgroupService workgroupService)
        {
            _workgroupRepository = workgroupRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _securityService = securityService;
            _searchService = searchService;
            _workgroupVendorRepository = workgroupVendorRepository;
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
            _stateRepository = stateRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
            _workgroupAccountRepository = workgroupAccountRepository;
            _workgroupAddressService = workgroupAddressService;
            _workgroupService = workgroupService;
        }

        

        #region Workgroup Actions
        /// <summary>
        /// Actions #1
        /// GET: /Workgroup/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var person =
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).Single();

            var orgIds = person.Organizations.Select(x => x.Id).ToArray();

            var workgroupList =
                _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

            return View(workgroupList.ToList());
        }

        /// <summary>
        /// Actions #2
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = WorkgroupModifyModel.Create(user);

            return View(model);
        }

        /// <summary>
        /// Actions #3
        /// </summary>
        /// <param name="workgroup"></param>
        /// <param name="selectedOrganizations"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Workgroup workgroup, string[] selectedOrganizations)
        {
            if (!ModelState.IsValid)
            {
                var model = WorkgroupModifyModel.Create(GetCurrentUser());
                model.Workgroup = workgroup;

                return View(model);
            }

            var createdWorkgroup = _workgroupService.CreateWorkgroup(workgroup, selectedOrganizations);

            Message = string.Format("{0} workgroup was created",
                                    createdWorkgroup.Name);

            return this.RedirectToAction(a => a.Index());
        }

        /// <summary>
        /// Actions #4
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorkgroupDetailsViewModel.Create(_workgroupPermissionRepository, workgroup);

            return View(viewModel);

        }

        /// <summary>
        /// Actions #5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var user = _userRepository.Queryable.Single(x => x.Id == CurrentUser.Identity.Name);
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = WorkgroupModifyModel.Create(user, workgroup);

            return View(model);
        }

        /// <summary>
        /// Actions #6
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="workgroup"></param>
        /// <param name="selectedOrganizations"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, Workgroup workgroup, string[] selectedOrganizations)
        {
            var workgroupToEdit = _workgroupRepository.GetNullableById(id);
            if(workgroupToEdit == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            Mapper.Map(workgroup, workgroupToEdit);
            workgroupToEdit.PrimaryOrganization = workgroup.PrimaryOrganization;

            if (selectedOrganizations != null)
            {                
                workgroupToEdit.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToEdit.Organizations.Contains(workgroupToEdit.PrimaryOrganization))
            {
                workgroupToEdit.Organizations.Add(workgroupToEdit.PrimaryOrganization);
            }

            //TODO: Test this.
            if(!ModelState.IsValid)
            {
                //Moved here because if you just pass workgroup, it doesn't have any selected organizations.
                return View(WorkgroupModifyModel.Create(GetCurrentUser(), workgroupToEdit));
            }

            _workgroupRepository.EnsurePersistent(workgroupToEdit);

            Message = string.Format("{0} was modified successfully",
                                    workgroup.Name);

            return this.RedirectToAction(a => a.Index());

        }

        /// <summary>
        /// Actions #7
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }

            return View(workgroup);
        }

        /// <summary>
        /// Actions #8
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="workgroup"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, Workgroup workgroup)
        {
            var workgroupToDelete = _workgroupRepository.GetNullableById(id);
            if(workgroupToDelete == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }
            workgroupToDelete.IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroupToDelete);

            Message = string.Format("{0} was disabled successfully",
                                    workgroup.Name);

            return this.RedirectToAction(a => a.Index());

        }
        #endregion

        #region Workgroup Accounts
        /// <summary>
        /// Accounts #1
        /// GET: Workgroup/Accounts/{Workgroup Id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult Accounts(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            return View(workgroup);
        }

        /// <summary>
        /// Accounts #2
        /// GET : Workgroup/AddAccount/{Workgroup Id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult AddAccount(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup);

            return View(viewModel);
        }

        /// <summary>
        /// Accounts #3
        /// POST: Workgroup/AddAccount
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="workgroupAccount">Workgroup Account Model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAccount(int id, WorkgroupAccount workgroupAccount)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }


            var workgroupAccountToCreate = new WorkgroupAccount() {Workgroup = workgroup};
            Mapper.Map(workgroupAccount, workgroupAccountToCreate);

            ModelState.Clear();
            workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);

            if(_workgroupAccountRepository.Queryable.Any(a => a.Workgroup.Id == workgroup.Id && a.Account.Id == workgroupAccountToCreate.Account.Id))
            {
                ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup");
            }

            if (ModelState.IsValid)
            {
                _workgroupAccountRepository.EnsurePersistent(workgroupAccountToCreate);
                Message = "Workgroup account saved.";
                //return this.RedirectToAction("Accounts", new {id = id});
                return this.RedirectToAction(a => a.Accounts(id));
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup, workgroupAccountToCreate);
            return View(viewModel);
        }

        /// <summary>
        /// Accounts #4
        /// GET: Workgroup/AccountDetails
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId">Workgroup Account Id</param>
        /// <returns></returns>
        public ActionResult AccountDetails(int id, int accountId)
        {
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if (account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index());
            }
            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            return View(account);
        }

        /// <summary>
        /// Accounts #5
        /// GET: Workgroup/AccountEdit
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId"> Workgroup Account Id</param>
        /// <returns></returns>
        public ActionResult EditAccount(int id, int accountId)
        {
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if (account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, account.Workgroup, account);
            return View(viewModel);
        }

        /// <summary>
        /// Accounts #6
        /// Post: Workgroup/AccountEdit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountId"> </param>
        /// <param name="workgroupAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAccount(int id, int accountId, WorkgroupAccount workgroupAccount)
        {
            var accountToEdit = _workgroupAccountRepository.GetNullableById(accountId);

            if (accountToEdit == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            if(accountToEdit.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            Mapper.Map(workgroupAccount, accountToEdit);

            ModelState.Clear();
            accountToEdit.TransferValidationMessagesTo(ModelState);

            if(_workgroupAccountRepository.Queryable.Any(a => a.Id != accountToEdit.Id &&  a.Workgroup.Id == accountToEdit.Workgroup.Id && a.Account.Id == accountToEdit.Account.Id ))
            {
                ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup");
            }

            if (ModelState.IsValid)
            {
                _workgroupAccountRepository.EnsurePersistent(accountToEdit);
                Message = "Workgroup account has been updated.";
                //return RedirectToAction("Accounts", new { id = accountToEdit.Workgroup.Id });
                return this.RedirectToAction(a => a.Accounts(accountToEdit.Workgroup.Id));
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, accountToEdit.Workgroup, accountToEdit);
            return View(viewModel);
        }

        /// <summary>
        /// Accounts #7
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId">WorkgroupAccount Id</param>
        /// <returns></returns>
        public ActionResult AccountDelete(int id, int accountId)
        {
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if(account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

           return View(account);
        }


        /// <summary>
        /// Accounts #8
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId">WorkgroupAccount Id</param>
        /// <param name="workgroupAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AccountDelete(int id, int accountId, WorkgroupAccount workgroupAccount)
        {
            var accountToDelete = _workgroupAccountRepository.GetNullableById(accountId);

            if(accountToDelete == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            if(accountToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            var saveWorkgroupId = accountToDelete.Workgroup.Id;

            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }

            _workgroupAccountRepository.Remove(accountToDelete);

            Message = "Account Removed from Workgroup";

            return this.RedirectToAction(a => a.Accounts(saveWorkgroupId));
        }

        #endregion

        #region Workgroup Vendors
        /// <summary>
        /// Vendors #1
        /// GET: Workgroup/Vendor/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult VendorList(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index());
            }

            var workgroupVendorList = _workgroupVendorRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsActive);
            ViewBag.WorkgroupId = id;
            return View(workgroupVendorList.ToList());
        }

        /// <summary>
        /// Vendors #2
        /// GET: Workgroup/Vendor/Create/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult CreateVendor(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index());
            }

            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, new WorkgroupVendor() { Workgroup = workgroup });

            return View(viewModel);
        }

        /// <summary>
        /// Vendors #3
        /// POST: Workgroup/Vendor/Create/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="workgroupVendor">Workgroup Vendor Object</param>
        /// <param name="newVendor">New Vendor or KFS Existing Vendor</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateVendor(int id, WorkgroupVendor workgroupVendor, bool newVendor)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index());
            }

            var workgroupVendorToCreate = new WorkgroupVendor();

            _workgroupService.TransferValues(workgroupVendor, ref workgroupVendorToCreate);

            workgroupVendorToCreate.Workgroup = workgroup;

            ModelState.Clear();
            workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return this.RedirectToAction(a => a.VendorList(id));
            }

            WorkgroupVendorViewModel viewModel;

            if (!newVendor)
            {
                var vendor = _vendorRepository.GetNullableById(workgroupVendor.VendorId);
                var vendorAddress = _vendorAddressRepository.Queryable.FirstOrDefault(a => a.Vendor == vendor && a.TypeCode == workgroupVendor.VendorAddressTypeCode);
                viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, vendor, vendorAddress, newVendor);
            }
            else
            {
                viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, newVendor: true);
            }


            return View(viewModel);
        }

        /// <summary>
        /// Vendors #4
        /// GET: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <param name="workgroupVendorId"> </param>
        /// <returns></returns>
        public ActionResult EditWorkgroupVendor(int id, int workgroupVendorId)
        {
            var workgroupVendor = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if (workgroupVendor == null)
            {
                ErrorMessage = "Workgroup Vendor not found.";
                return this.RedirectToAction(a => a.Index());
            }

            if(workgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            if (!string.IsNullOrWhiteSpace(workgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(workgroupVendor.VendorAddressTypeCode))
            {
                ErrorMessage = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
                return this.RedirectToAction(a => a.VendorList(workgroupVendor.Workgroup.Id));
            }

            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendor);
            return View(viewModel);
        }

        /// <summary>
        /// Vendors #5
        /// POST: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
        /// <param name="id"></param>
        /// <param name="workgroupVendorId"> </param>
        /// <param name="workgroupVendor"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditWorkgroupVendor(int id, int workgroupVendorId, WorkgroupVendor workgroupVendor)
        {
            var oldWorkgroupVendor = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if(oldWorkgroupVendor == null)
            {
                ErrorMessage = "Workgroup Vendor not found.";
                return this.RedirectToAction(a => a.Index());
            }

            if(oldWorkgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            if (!string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorAddressTypeCode))
            {
                ErrorMessage = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
                return this.RedirectToAction(a => a.VendorList(oldWorkgroupVendor.Workgroup.Id));
            }

            Check.Require(string.IsNullOrWhiteSpace(workgroupVendor.VendorId), "Can't have VendorId when editing");
            Check.Require(string.IsNullOrWhiteSpace(workgroupVendor.VendorAddressTypeCode), "Can't have vendorAddresstypeCode when editing");

            var newWorkgroupVendor = new WorkgroupVendor();
            newWorkgroupVendor.Workgroup = oldWorkgroupVendor.Workgroup;

            _workgroupService.TransferValues(workgroupVendor, ref newWorkgroupVendor);
            ModelState.Clear();
            newWorkgroupVendor.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                oldWorkgroupVendor.IsActive = false;
                _workgroupVendorRepository.EnsurePersistent(oldWorkgroupVendor);
                _workgroupVendorRepository.EnsurePersistent(newWorkgroupVendor);

                Message = "WorkgroupVendor Edited Successfully";

                //return RedirectToAction("VendorList", new { Id = newWorkgroupVendor.Workgroup.Id });
                return this.RedirectToAction(a => a.VendorList(newWorkgroupVendor.Workgroup.Id));
            }

            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, newWorkgroupVendor);

            return View(viewModel);
        }

        /// <summary>
        /// Vendors #6
        /// GET: Workgroup/DeleteWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <param name="workgroupVendorId"> </param>
        /// <returns></returns>
        public ActionResult DeleteWorkgroupVendor(int id, int workgroupVendorId)
        {
            var workgroupVendor = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if (workgroupVendor == null)
            {                
                return this.RedirectToAction(a => a.Index());
            }

            if(workgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            return View(workgroupVendor);
        }

        /// <summary>
        /// Vendors #7
        /// POST: /WorkgroupVendor/Delete/5
        /// </summary>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <param name="workgroupVendorId"> </param>
        /// <param name="workgroupVendor">ignored</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteWorkgroupVendor(int id, int workgroupVendorId, WorkgroupVendor workgroupVendor)
        {
            var workgroupVendorToDelete = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if (workgroupVendorToDelete == null)
            {
                ErrorMessage = "WorkgroupVendor not found.";
                return this.RedirectToAction(a => a.Index());
            }

            if(workgroupVendorToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index());
            }

            workgroupVendorToDelete.IsActive = false;

            _workgroupVendorRepository.EnsurePersistent(workgroupVendorToDelete);

            Message = "WorkgroupVendor Removed Successfully";

            return this.RedirectToAction(a => a.VendorList(workgroupVendorToDelete.Workgroup.Id));
        }


        public ActionResult BulkVendor()
        {
            return View();
        }

        // This action handles the form POST and the upload
        [HttpPost]
        public ActionResult BulkVendor(int id, HttpPostedFileBase file)
        {
            // Verify that the user selected a file

            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index());
            }

            if (file != null && file.ContentLength > 0)
            {
                Stream uploadFileStream = file.InputStream;

                /*
                HSSFWorkbook wBook = new HSSFWorkbook(uploadFileStream);
                int successCount = 0;
                var sheet = wBook.GetSheetAt(0);
                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    var workgroupVendorToCreate = new WorkgroupVendor();
                    if (sheet.GetRow(row).GetCell(0).StringCellValue == "Name")
                    {
                        continue;
                    }
                    workgroupVendorToCreate.Name = sheet.GetRow(row).GetCell(0).ToString();
                    workgroupVendorToCreate.Line1 = sheet.GetRow(row).GetCell(1).ToString();
                    workgroupVendorToCreate.Line2 = sheet.GetRow(row).GetCell(2) != null ? sheet.GetRow(row).GetCell(2).ToString() : null;
                    workgroupVendorToCreate.Line3 = sheet.GetRow(row).GetCell(3) != null ? sheet.GetRow(row).GetCell(3).ToString() : null;
                    workgroupVendorToCreate.City = sheet.GetRow(row).GetCell(4).StringCellValue;
                    workgroupVendorToCreate.State = sheet.GetRow(row).GetCell(5).StringCellValue;
                    workgroupVendorToCreate.Zip = sheet.GetRow(row).GetCell(6).ToString();
                    workgroupVendorToCreate.CountryCode = sheet.GetRow(row).GetCell(7).ToString();
                    workgroupVendorToCreate.Phone = sheet.GetRow(row).GetCell(8) != null ? sheet.GetRow(row).GetCell(8).ToString() : null;
                    workgroupVendorToCreate.Email = sheet.GetRow(row).GetCell(9) != null ? sheet.GetRow(row).GetCell(9).ToString() : null;
                    
                    workgroupVendorToCreate.Workgroup = workgroup;

                    ModelState.Clear();
                    workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

                    if (ModelState.IsValid)
                    {
                        _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                        successCount++;

                        //return this.RedirectToAction(a => a.VendorList(id));
                    }
                }

                Message = string.Format("Successfully added {0} vendors to workgroup. ", successCount);
                 */
                //{2} not added because of duplicated role.

            }
            // redirect back to the index action to show the form once again
            return this.RedirectToAction(a=>a.VendorList(workgroup.Id));
        }

       

        #endregion

        #region Workgroup Addresses
        /// <summary>
        /// Address #1
        /// </summary>
        /// <param name="id">Workgroup id</param>
        /// <returns></returns>
        public ActionResult Addresses(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressListModel.Create(workgroup);
            return View(viewModel);
        }

        /// <summary>
        /// Address #2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddAddress(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = new WorkgroupAddress();
            viewModel.WorkgroupAddress.Workgroup = workgroup;
            return View(viewModel);

        }

        /// <summary>
        /// Address #3
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAddress(int id, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            workgroupAddress.Workgroup = workgroup;
            ModelState.Clear();
            workgroupAddress.TransferValidationMessagesTo(ModelState);
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Address not valid";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                viewModel.WorkgroupAddress.Workgroup = workgroup;
                return View(viewModel);
            }
            var matchFound = 0;
            foreach (var address in workgroup.Addresses)
            {
                matchFound = _workgroupAddressService.CompareAddress(workgroupAddress, address);
                if (matchFound > 0)
                {
                    break;
                }
            }
            if (matchFound > 0)
            {
                var matchedAddress = workgroup.Addresses.Single(a => a.Id == matchFound);
                if (!matchedAddress.IsActive)
                {
                    Message = "Address created.";
                    matchedAddress.IsActive = true;
                    _workgroupRepository.EnsurePersistent(workgroup);
                }
                else
                {
                    Message = "This Address already exists.";
                }
            }
            else
            {
                Message = "Address created";
                workgroup.AddAddress(workgroupAddress);
                _workgroupRepository.EnsurePersistent(workgroup);
            }
            return this.RedirectToAction(a => a.Addresses(id));
        }

        /// <summary>
        /// Address #4
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <returns></returns>
        public ActionResult DeleteAddress(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository);
            viewModel.WorkgroupAddress = workgroupAddress;
            viewModel.State = _stateRepository.GetNullableById(workgroupAddress.State);
            return View(viewModel);
        }

        /// <summary>
        /// Address #5
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <param name="workgroupAddress">Dummy parm</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAddress(int id, int addressId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddressToDelete = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddressToDelete == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddressToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index());
            }
            workgroupAddressToDelete.IsActive = false;
            _workgroupRepository.EnsurePersistent(workgroup);
            Message = "Address deleted.";
            return this.RedirectToAction(a => a.Addresses(id));
        }

        /// <summary>
        /// Address #6
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <returns></returns>
        public ActionResult AddressDetails(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository);
            viewModel.WorkgroupAddress = workgroupAddress;
            viewModel.State = _stateRepository.GetNullableById(workgroupAddress.State);
            return View(viewModel);
        }

        /// <summary>
        /// Address #7
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <returns></returns>
        public ActionResult EditAddress(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = workgroupAddress;
            return View(viewModel);
        }

        /// <summary>
        /// Address #8
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <param name="workgroupAddress">address's new values</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAddress(int id, int addressId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var workgroupAddressToEdit = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddressToEdit == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddressToEdit.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index());
            }
            workgroupAddress.Workgroup = workgroup;
            ModelState.Clear();
            workgroupAddress.TransferValidationMessagesTo(ModelState);
            if(!ModelState.IsValid)
            {
                ErrorMessage = "Unable to save due to errors.";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                return View(viewModel);
            }

            if(_workgroupAddressService.CompareAddress(workgroupAddress, workgroup.Addresses.Where(a => a.Id == addressId).Single()) > 0)
            {
                Message = "No changes made";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                return View(viewModel);
            }


            foreach (var activeAddress in workgroup.Addresses.Where(a => a.IsActive && a.Id != addressId))
            {
                var activeMatchFound = _workgroupAddressService.CompareAddress(workgroupAddress, activeAddress);
                if(activeMatchFound > 0)
                {
                    ErrorMessage = "The address you are changing this to already exists. Unable to save.";
                    var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                    viewModel.WorkgroupAddress = workgroupAddress;
                    return View(viewModel);
                }
            }

            var matchFound = 0;
            foreach(var activeAddress in workgroup.Addresses.Where(a => !a.IsActive && a.Id != addressId))
            {
                matchFound = _workgroupAddressService.CompareAddress(workgroupAddress, activeAddress);
                if(matchFound > 0)
                {
                    break;
                }
            }

            if(matchFound > 0)
            {
                Message = "Address updated";
                workgroup.Addresses.Where(a => a.Id == matchFound).Single().IsActive = true;                
            }
            else
            {
                Message = "Address updated.";
                var newAddress = new WorkgroupAddress();
                Mapper.Map(workgroupAddress, newAddress);
                newAddress.Workgroup = workgroup;
                workgroup.AddAddress(newAddress);
            }
            workgroup.Addresses.Where(a => a.Id == addressId).Single().IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);
            return this.RedirectToAction(a => a.Addresses(id));

        }
        #endregion

        #region Workgroup People

        /// <summary>
        /// People #1
        /// List of all people within a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult People(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup==null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorgroupPeopleListModel.Create(_workgroupPermissionRepository, _roleRepository, workgroup, roleFilter);
            ViewBag.rolefilter = roleFilter;
            return View(viewModel);
        }

        /// <summary>
        /// People #2
        /// GET: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult AddPeople(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
            if(!string.IsNullOrWhiteSpace(roleFilter))
            {
                viewModel.Role = _roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == roleFilter).SingleOrDefault();
            }
            ViewBag.rolefilter = roleFilter;

            return View(viewModel);
        }

        /// <summary>
        /// People #3
        /// POST: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupPeoplePostModel"></param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
       [HttpPost]
        public ActionResult AddPeople(int id, WorkgroupPeoplePostModel workgroupPeoplePostModel, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }


            //Ensure role picked is valid.
            if (workgroupPeoplePostModel.Role != null)
            {
                if(!_roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == workgroupPeoplePostModel.Role.Id).Any())
                {
                    ModelState.AddModelError("Role", "Invalid Role Selected");
                }

            }

            if (!ModelState.IsValid)
            {
                var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);

                if (workgroupPeoplePostModel.Role != null)
                {
                    viewModel.Role = workgroupPeoplePostModel.Role;
                }
                if (workgroupPeoplePostModel.Users != null && workgroupPeoplePostModel.Users.Count > 0)
                {
                    var users = new List<IdAndName>();
                    foreach (var user in workgroupPeoplePostModel.Users)
                    {
                        var temp = _userRepository.GetNullableById(user);
                        if (temp != null)
                        {
                            users.Add(new IdAndName(temp.Id, temp.FullName));
                        }
                        else
                        {
                            var ldapuser = _searchService.FindUser(user);
                            if (ldapuser != null)
                            {
                                users.Add(new IdAndName(ldapuser.LoginId, string.Format("{0} {1}", ldapuser.FirstName, ldapuser.LastName)));
                            }
                        }
                    }
                    viewModel.Users = users;
                }
                ViewBag.rolefilter = roleFilter;
                return View(viewModel);
            }

            int successCount = 0;
            int failCount = 0;
            //var notAddedSb = new StringBuilder();
            var notAddedKvp = new List<KeyValuePair<string, string>>();
            foreach (var u in workgroupPeoplePostModel.Users)
            {
                successCount = _workgroupService.TryToAddPeople(id, workgroupPeoplePostModel.Role, workgroup, successCount, u, ref failCount, notAddedKvp);
            }

            Message = string.Format("Successfully added {0} people to workgroup as {1}. {2} not added because of duplicated role.", successCount,
                                    workgroupPeoplePostModel.Role.Name, failCount);

            return this.RedirectToAction(a => a.People(id, workgroupPeoplePostModel.Role.Id));

        }

        /// <summary>
        /// People #4
        /// GET: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id">Workgroup ID</param>
       /// <param name="workgroupPermissionId">Workgroup Permission ID</param>
        /// <param name="rolefilter"></param>
        /// <returns></returns>
        public ActionResult DeletePeople(int id, int workgroupPermissionId, string rolefilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }

            var workgroupPermission = _workgroupPermissionRepository.GetNullableById(workgroupPermissionId);
            if(workgroupPermission == null)
            {
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }

            if(workgroupPermission.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermission);

            ViewBag.rolefilter = rolefilter;
            return View(viewModel);

        }

        /// <summary>
        /// People #5
        /// POST: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id">Workgroup ID</param>
        /// <param name="workgroupPermissionId">Workgroup Permission ID</param>
        /// <param name="rolefilter"></param>
        /// <param name="workgroupPermission"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePeople(int id, int workgroupPermissionId, string rolefilter, WorkgroupPermission workgroupPermission, string[] roles)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index());
            }

            var workgroupPermissionToDelete = _workgroupPermissionRepository.GetNullableById(workgroupPermissionId);
            if (workgroupPermissionToDelete == null)
            {
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }

            if(workgroupPermissionToDelete.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var availableWorkgroupPermissions = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Level >= 1 && a.Role.Level <= 4).ToList();
            if (availableWorkgroupPermissions.Count() == 1)
            {
                // TODO: Check for pending/open orders for this person. Set order to workgroup.
                _workgroupPermissionRepository.Remove(workgroupPermissionToDelete);
                Message = "Person successfully removed from role.";
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }
            else
            {
                if (roles == null || roles.Count() == 0)
                {
                    Message = "Must select at least 1 role to delete";
                    var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermissionToDelete);

                    ViewBag.rolefilter = rolefilter;
                    return View(viewModel);
                }
                var removedCount = 0;
                foreach (var role in roles)
                {
                    // TODO: Check for pending/open orders for this person. Set order to workgroup.
                    var wp = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Id == role).Single();
                    _workgroupPermissionRepository.Remove(wp);
                    removedCount++;
                }

                Message = string.Format("{0} {1} removed from {2}", removedCount, removedCount == 1 ? "role" : "roles", workgroupPermissionToDelete.User.FullName);
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }


        }

        #endregion
        
        #region Ajax Helpers

        /// <summary>
        /// Vendors #8
        /// Ajax action for retrieving kfs vendor addresses
        /// </summary>
        /// <returns></returns>
        public JsonNetResult GetVendorAddresses(string vendorId)
        {
            var vendorAddresses = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).ToList();

            var results = vendorAddresses.Select(a => new { TypeCode = a.TypeCode, Name = string.Format("{0} ({1}, {2}, {3} {4})", a.Name, a.Line1, a.City, a.State, a.Zip) }).ToList();

            return new JsonNetResult(results);
        }

        ///// <summary>
        ///// TODO: Don't think this is being used. Remove when confirmed.
        ///// </summary>
        ///// <param name="searchTerm"></param>
        ///// <returns></returns>
        //public JsonNetResult SearchOrganizations(string searchTerm)
        //{
        //    var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

        //    return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        //}

        /// <summary>
        /// People #6
        /// Search Users in the User Table and LDAP lookup if none found.
        /// </summary>
        /// <param name="searchTerm">Email or LoginId</param>
        /// <returns></returns>
        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count == 0)
            {
                var ldapuser = _searchService.FindUser(searchTerm);
                if (ldapuser != null)
                {
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.LoginId));
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.EmailAddress));

                    var user = new User(ldapuser.LoginId);
                    user.Email = ldapuser.EmailAddress;
                    user.FirstName = ldapuser.FirstName;
                    user.LastName = ldapuser.LastName;

                    users.Add(user);
                }
            }

            //We don't want to show users that are not active
            var results =
                users.Where(a => a.IsActive).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }
        #endregion
    }
}
