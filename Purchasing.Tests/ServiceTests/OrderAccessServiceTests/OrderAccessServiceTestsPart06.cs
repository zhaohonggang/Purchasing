﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
{
    public partial class OrderAccessServiceTests
    {
        #region User With Multiple Roles Tests

        [TestMethod]
        public void TestWhenRequestorAndApproverDifferentWorkGroups1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();

            var workgroupPermissions = new List<WorkgroupPermission>();
            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
            workgroupPermissions.Add(workgroupPermission);
            SetupWorkgroupPermissions1(workgroupPermissions);

            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(6, results.Count, "Should only see 6, 4 + 2 for wg 1 where simpson is the approver.");

            #endregion Assert		
        }

        /// <summary>
        /// Should see 7. 6 from wg 1 where simpson is the approver. 1 from wg 2 where simpson is the requestor
        /// </summary>
        [TestMethod]
        public void TestWhenRequestorAndApproverDifferentWorkGroups2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();

            var workgroupPermissions = new List<WorkgroupPermission>();
            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Requester);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
            workgroupPermissions.Add(workgroupPermission);
            SetupWorkgroupPermissions1(workgroupPermissions);

            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
            SetupOrders(orders, approvals, 1, "hsimpson", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(7, results.Count);

            #endregion Assert
        }

        /// <summary>
        /// Should see 7 because is an approver for both wg
        /// </summary>
        [TestMethod]
        public void TestWhenApproverForDifferentWorkGroups1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();

            var workgroupPermissions = new List<WorkgroupPermission>();
            var workgroupPermission = CreateValidEntities.WorkgroupPermission(2);
            workgroupPermission.User = UserRepository.GetNullableById("hsimpson");
            workgroupPermission.Role = RoleRepository.GetNullableById(Role.Codes.Approver);
            workgroupPermission.Workgroup = WorkgroupRepository.GetNullableById(2);
            workgroupPermissions.Add(workgroupPermission);
            SetupWorkgroupPermissions1(workgroupPermissions);

            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(7, results.Count);

            #endregion Assert
        }
        #endregion User With Multiple Roles Tests
    }
}