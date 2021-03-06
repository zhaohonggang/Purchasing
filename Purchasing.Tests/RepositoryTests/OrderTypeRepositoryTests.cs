﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		OrderType
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class OrderTypeRepositoryTests : AbstractRepositoryTests<OrderType, string, OrderTypeMap>
    {
        /// <summary>
        /// Gets or sets the OrderType repository.
        /// </summary>
        /// <value>The OrderType repository.</value>
        public IRepository<OrderType> OrderTypeRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderTypeRepositoryTests"/> class.
        /// </summary>
        public OrderTypeRepositoryTests()
        {
            OrderTypeRepository = new Repository<OrderType>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override OrderType GetValid(int? counter)
        {
            return CreateValidEntities.OrderType(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<OrderType> GetQuery(int numberAtEnd)
        {
            return OrderTypeRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(OrderType entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(OrderType entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            OrderTypeRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            OrderTypeRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            OrderType orderType = null;
            try
            {
                #region Arrange
                orderType = GetValid(9);
                orderType.Name = null;
                #endregion Arrange

                #region Act
                OrderTypeRepository.DbContext.BeginTransaction();
                OrderTypeRepository.EnsurePersistent(orderType);
                OrderTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderType);
                var results = orderType.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(orderType.IsTransient());
                Assert.IsFalse(orderType.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            OrderType orderType = null;
            try
            {
                #region Arrange
                orderType = GetValid(9);
                orderType.Name = string.Empty;
                #endregion Arrange

                #region Act
                OrderTypeRepository.DbContext.BeginTransaction();
                OrderTypeRepository.EnsurePersistent(orderType);
                OrderTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderType);
                var results = orderType.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(orderType.IsTransient());
                Assert.IsFalse(orderType.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            OrderType orderType = null;
            try
            {
                #region Arrange
                orderType = GetValid(9);
                orderType.Name = " ";
                #endregion Arrange

                #region Act
                OrderTypeRepository.DbContext.BeginTransaction();
                OrderTypeRepository.EnsurePersistent(orderType);
                OrderTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderType);
                var results = orderType.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                //Assert.IsTrue(orderType.IsTransient());
                Assert.IsFalse(orderType.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            OrderType orderType = null;
            try
            {
                #region Arrange
                orderType = GetValid(9);
                orderType.Name = "x".RepeatTimes((50 + 1));
                #endregion Arrange

                #region Act
                OrderTypeRepository.DbContext.BeginTransaction();
                OrderTypeRepository.EnsurePersistent(orderType);
                OrderTypeRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(orderType);
                Assert.AreEqual(50 + 1, orderType.Name.Length);
                var results = orderType.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "50"));
                //Assert.IsTrue(orderType.IsTransient());
                Assert.IsFalse(orderType.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.Name = "x";
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.Name = "x".RepeatTimes(50);
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(50, orderType.Name.Length);
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests   

        #region Enum Tests

        [TestMethod]
        public void TestTypes()
        {
            #region Arrange
            
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("DPO", OrderType.Types.DepartmentalPurchaseOrder);
            Assert.AreEqual("DRO", OrderType.Types.DepartmentalRepairOrder);
            Assert.AreEqual("OR", OrderType.Types.OrderRequest);
            Assert.AreEqual("PC", OrderType.Types.PurchasingCard);
            Assert.AreEqual("PR", OrderType.Types.PurchaseRequest);
            Assert.AreEqual("UCB", OrderType.Types.UcdBuyOrder);
            #endregion Assert		
        }
        #endregion Enum Tests

        #region PurchaserAssignable Tests

        /// <summary>
        /// Tests the PurchaserAssignable is false saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserAssignableIsFalseSaves()
        {
            #region Arrange
            OrderType orderType = GetValid(9);
            orderType.PurchaserAssignable = false;
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderType.PurchaserAssignable);
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the PurchaserAssignable is true saves.
        /// </summary>
        [TestMethod]
        public void TestPurchaserAssignableIsTrueSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.PurchaserAssignable = true;
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(orderType.PurchaserAssignable);
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        #endregion PurchaserAssignable Tests

        #region DocType Tests

        #region Valid Tests

        /// <summary>
        /// Tests the DocType with null value saves.
        /// </summary>
        [TestMethod]
        public void TestDocTypeWithNullValueSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.DocType = null;
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DocType with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestDocTypeWithEmptyStringSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.DocType = string.Empty;
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DocType with one space saves.
        /// </summary>
        [TestMethod]
        public void TestDocTypeWithOneSpaceSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.DocType = " ";
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DocType with one character saves.
        /// </summary>
        [TestMethod]
        public void TestDocTypeWithOneCharacterSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.DocType = "x";
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the DocType with long value saves.
        /// </summary>
        [TestMethod]
        public void TestDocTypeWithLongValueSaves()
        {
            #region Arrange
            var orderType = GetValid(9);
            orderType.DocType = "x".RepeatTimes(999);
            #endregion Arrange

            #region Act
            OrderTypeRepository.DbContext.BeginTransaction();
            OrderTypeRepository.EnsurePersistent(orderType);
            OrderTypeRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(999, orderType.DocType.Length);
            Assert.IsFalse(orderType.IsTransient());
            Assert.IsTrue(orderType.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion DocType Tests

        
        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();
            expectedFields.Add(new NameAndType("DocType", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.String", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("PurchaserAssignable", "System.Boolean", new List<string>()));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(OrderType));

        }

        #endregion Reflection of Database.	
		
		
    }
}