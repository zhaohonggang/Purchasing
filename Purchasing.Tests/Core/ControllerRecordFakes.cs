﻿using System;
using System.Collections.Generic;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing.Fakes;

namespace Purchasing.Tests.Core
{
    public class FakeAutoApprovals : ControllerRecordFakes<AutoApproval>
    {
        protected override AutoApproval CreateValid(int i)
        {
            return CreateValidEntities.AutoApproval(i);
        }
        public FakeAutoApprovals(int count, IRepository<AutoApproval> repository, List<AutoApproval> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeAutoApprovals(int count, IRepository<AutoApproval> repository)
        {
            Records(count, repository);
        }
        public FakeAutoApprovals()
        {

        }
    }

    public class FakeWorkgroupPermissions : ControllerRecordFakes<WorkgroupPermission>
    {
        protected override WorkgroupPermission CreateValid(int i)
        {
            return CreateValidEntities.WorkgroupPermission(i);
        }
        public FakeWorkgroupPermissions(int count, IRepository<WorkgroupPermission> repository, List<WorkgroupPermission> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeWorkgroupPermissions(int count, IRepository<WorkgroupPermission> repository)
        {
            Records(count, repository);
        }
        public FakeWorkgroupPermissions()
        {

        }
    }

    public class FakeConditionalApprovals : ControllerRecordFakes<ConditionalApproval>
    {
        protected override ConditionalApproval CreateValid(int i)
        {
            return CreateValidEntities.ConditionalApproval(i);
        }
        public FakeConditionalApprovals(int count, IRepository<ConditionalApproval> repository, List<ConditionalApproval> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeConditionalApprovals(int count, IRepository<ConditionalApproval> repository)
        {
            Records(count, repository);
        }
        public FakeConditionalApprovals()
        {

        }
    }


    public class FakeWorkgroupAccounts : ControllerRecordFakes<WorkgroupAccount>
    {
        protected override WorkgroupAccount CreateValid(int i)
        {
            return CreateValidEntities.WorkgroupAccount(i);
        }
        public FakeWorkgroupAccounts(int count, IRepository<WorkgroupAccount> repository, List<WorkgroupAccount> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeWorkgroupAccounts(int count, IRepository<WorkgroupAccount> repository)
        {
            Records(count, repository);
        }
        public FakeWorkgroupAccounts()
        {

        }
    }

    public class FakeOrders : ControllerRecordFakes<Order>
    {
        protected override Order CreateValid(int i)
        {
            return CreateValidEntities.Order(i);
        }
        public FakeOrders(int count, IRepository<Order> repository, List<Order> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeOrders(int count, IRepository<Order> repository)
        {
            Records(count, repository);
        }
        public FakeOrders()
        {

        }
    }

    public class FakeApprovals : ControllerRecordFakes<Approval>
    {
        protected override Approval CreateValid(int i)
        {
            return CreateValidEntities.Approval(i);
        }
        public FakeApprovals(int count, IRepository<Approval> repository, List<Approval> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeApprovals(int count, IRepository<Approval> repository)
        {
            Records(count, repository);
        }
        public FakeApprovals()
        {

        }
    }

    public class FakeOrderTracking : ControllerRecordFakes<OrderTracking>
    {
        protected override OrderTracking CreateValid(int i)
        {
            return CreateValidEntities.OrderTracking(i);
        }
        public FakeOrderTracking(int count, IRepository<OrderTracking> repository, List<OrderTracking> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeOrderTracking(int count, IRepository<OrderTracking> repository)
        {
            Records(count, repository);
        }
        public FakeOrderTracking()
        {

        }
    }

    public class FakeUsers : AbstractControllerRecordFakesStrings<User>
    {
        protected override User CreateValid(int i)
        {
            return CreateValidEntities.User(i);
        }
        public FakeUsers(int count, IRepositoryWithTypedId<User, string> repository, List<User> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeUsers(int count, IRepositoryWithTypedId<User, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeUsers()
        {

        }
    }

    public class FakeOrganizations : AbstractControllerRecordFakesStrings<Organization>
    {
        protected override Organization CreateValid(int i)
        {
            return CreateValidEntities.Organization(i);
        }
        public FakeOrganizations(int count, IRepositoryWithTypedId<Organization, string> repository, List<Organization> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeOrganizations(int count, IRepositoryWithTypedId<Organization, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeOrganizations()
        {

        }
    }

    public class FakeRoles : AbstractControllerRecordFakesStrings<Role>
    {
        protected override Role CreateValid(int i)
        {
            return CreateValidEntities.Role(i);
        }
        public FakeRoles(int count, IRepositoryWithTypedId<Role, string> repository, List<Role> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeRoles(int count, IRepositoryWithTypedId<Role, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeRoles()
        {

        }
    }

    public class FakeOrderStatusCodes : AbstractControllerRecordFakesStrings<OrderStatusCode>
    {
        protected override OrderStatusCode CreateValid(int i)
        {
            return CreateValidEntities.OrderStatusCode(i);
        }
        public FakeOrderStatusCodes(int count, IRepositoryWithTypedId<OrderStatusCode, string> repository, List<OrderStatusCode> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeOrderStatusCodes(int count, IRepositoryWithTypedId<OrderStatusCode, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeOrderStatusCodes()
        {

        }
    }

    public class FakeStates : AbstractControllerRecordFakesStrings<State>
    {
        protected override State CreateValid(int i)
        {
            return CreateValidEntities.State(i);
        }
        public FakeStates(int count, IRepositoryWithTypedId<State, string> repository, List<State> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeStates(int count, IRepositoryWithTypedId<State, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeStates()
        {

        }
    }

    public class FakeWorkgroups : ControllerRecordFakes<Workgroup>
    {
        protected override Workgroup CreateValid(int i)
        {
            return CreateValidEntities.Workgroup(i);
        }
        public FakeWorkgroups(int count, IRepository<Workgroup> repository, List<Workgroup> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeWorkgroups(int count, IRepository<Workgroup> repository)
        {
            Records(count, repository);
        }
        public FakeWorkgroups()
        {

        }
    }



    public class FakeWorkgroupVendors : ControllerRecordFakes<WorkgroupVendor>
    {
        protected override WorkgroupVendor CreateValid(int i)
        {
            return CreateValidEntities.WorkgroupVendor(i);
        }
        public FakeWorkgroupVendors(int count, IRepository<WorkgroupVendor> repository, List<WorkgroupVendor> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeWorkgroupVendors(int count, IRepository<WorkgroupVendor> repository)
        {
            Records(count, repository);
        }
        public FakeWorkgroupVendors()
        {

        }
    }

    public class FakeVendorAddresses : AbstractControllerRecordFakesGuids<VendorAddress>
    {
        protected override VendorAddress CreateValid(int i)
        {
            return CreateValidEntities.VendorAddress(i);
        }
        public FakeVendorAddresses(int count, IRepositoryWithTypedId<VendorAddress, Guid> repository, List<VendorAddress> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeVendorAddresses(int count, IRepositoryWithTypedId<VendorAddress, Guid> repository)
        {
            Records(count, repository, false);
        }
        public FakeVendorAddresses()
        {

        }
    }

    public class FakeVendors : AbstractControllerRecordFakesStrings<Vendor>
    {
        protected override Vendor CreateValid(int i)
        {
            return CreateValidEntities.Vendor(i);
        }
        public FakeVendors(int count, IRepositoryWithTypedId<Vendor, string> repository, List<Vendor> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeVendors(int count, IRepositoryWithTypedId<Vendor, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeVendors()
        {

        }
    }

    public class FakeEmailPreferences : AbstractControllerRecordFakesStrings<EmailPreferences>
    {
        protected override EmailPreferences CreateValid(int i)
        {
            return CreateValidEntities.EmailPreferences(i);
        }
        public FakeEmailPreferences(int count, IRepositoryWithTypedId<EmailPreferences, string> repository, List<EmailPreferences> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeEmailPreferences(int count, IRepositoryWithTypedId<EmailPreferences, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeEmailPreferences()
        {

        }
    }

    public class FakeOrganizationDescendants : ControllerRecordFakes<OrganizationDescendant>
    {
        protected override OrganizationDescendant CreateValid(int i)
        {
            return CreateValidEntities.OrganizationDescendant(i);
        }

        public FakeOrganizationDescendants(int count, IRepository<OrganizationDescendant> repository, List<OrganizationDescendant> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeOrganizationDescendants(int count, IRepository<OrganizationDescendant> repository)
        {
            Records(count, repository);
        }

        public FakeOrganizationDescendants()
        {

        }
    }
}
