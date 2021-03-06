﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Rhino.Mocks;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    public partial class OrderServiceTests
    {
        #region EditExistingOrder Tests

        [TestMethod]
        public void TestEditExistingOrder()
        {
            #region Arrange
            var order = CreateValidEntities.Order(7);
            var order2 = CreateValidEntities.Order(8);
            #endregion Arrange

            #region Act
            OrderService.EditExistingOrder(order);
            #endregion Act

            #region Assert
            EventService.AssertWasCalled(a => a.OrderEdited(order));
            EventService.AssertWasNotCalled(a => a.OrderEdited(order2));
            #endregion Assert		
        }

        #endregion EditExistingOrder Tests

        #region ReRouteSingleApprovalForExistingOrder Tests

        [TestMethod]
        public void TestReRouteSingleApprovalForExistingOrder()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        }
        #endregion ReRouteSingleApprovalForExistingOrder Tests
    }
}
