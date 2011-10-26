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
        #region Approver Tests
        [TestMethod]
        public void TestOrdersForApprover1()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            SetupOrders1("bender");
            SetupApprovals1();

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("bender", results[0].CreatedBy.Id);
            Assert.AreEqual("moe", results[5].CreatedBy.Id);
            #endregion Assert
        }


        /// <summary>
        /// Same as the one above but new way of setting data
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover2()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
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
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("bender", results[0].CreatedBy.Id);
            Assert.AreEqual("moe", results[5].CreatedBy.Id);
            #endregion Assert
        }

        /// <summary>
        /// All the orders are at the Account Manager Stage
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover3()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.AccountManager), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }

        /// <summary>
        /// All the orders are at the Purchaser stage
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover4()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }

        /// <summary>
        /// 2 orders by moe (workgroup 1) are att approver stage
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover5()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("hsimpson").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Purchaser), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
            foreach(var result in results)
            {
                Assert.AreEqual("moe", result.CreatedBy.Id);
            }
            #endregion Assert
        }

        /// <summary>
        /// Burns is an approver for wg 1, but no orders
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover6()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("burns").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
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
            Assert.AreEqual(0, results.Count);
            #endregion Assert
        }

        /// <summary>
        /// Burns is an approver for wg 1, all order approvals have a null user for the approver
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover7()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("burns").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);

            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            foreach (var approval in approvals)
            {
                if(approval.StatusCode == orderStatusCode)
                {
                    approval.User = null;
                }
            }
            SetupOrders(orders,approvals, 0, null, null, 0, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(6, results.Count);
            #endregion Assert
        }

        /// <summary>
        /// Burns is an approver for wg 1, all order approvals have burns as a secondary user for the approver
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover8()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("burns").Repeat.Any();
            SetupUsers1();
            SetupWorkgroupPermissions1();
            var orders = new List<Order>();
            var approvals = new List<Approval>();
            SetupOrders(orders, approvals, 4, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);
            SetupOrders(orders, approvals, 1, "bender", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 2);
            SetupOrders(orders, approvals, 2, "moe", OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver), 1);

            var orderStatusCode = OrderStatusCodeRepository.GetNullableById(OrderStatusCode.Codes.Approver);
            foreach(var approval in approvals)
            {
                if(approval.StatusCode == orderStatusCode)
                {
                    approval.SecondaryUser = UserRepository.GetNullableById("burns");
                }
            }
            SetupOrders(orders, approvals, 0, null, null, 0, true);

            #endregion Arrange

            #region Act
            var results = OrderAccessService.GetListofOrders();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(6, results.Count);
            #endregion Assert
        }

        /// <summary>
        /// simpson is the approver for orders, but he is away, so burns can see them.
        /// </summary>
        [TestMethod]
        public void TestOrdersForApprover9()
        {
            #region Arrange
            UserIdentity.Expect(a => a.Current).Return("burns").Repeat.Any();
            var user = CreateValidEntities.User(2);
            user.FirstName = "Homer";
            user.LastName = "Simpson";
            user.SetIdTo("hsimpson");
            user.IsAway = true;

            SetupUsers1(user);
            SetupWorkgroupPermissions1();
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
            Assert.AreEqual(6, results.Count);
            #endregion Assert
        }


        #endregion Approver Tests
    }
}