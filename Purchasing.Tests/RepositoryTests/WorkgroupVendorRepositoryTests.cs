﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		WorkgroupVendor
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class WorkgroupVendorRepositoryTests : AbstractRepositoryTests<WorkgroupVendor, int, WorkgroupVendorMap>
    {
        /// <summary>
        /// Gets or sets the WorkgroupVendor repository.
        /// </summary>
        /// <value>The WorkgroupVendor repository.</value>
        public IRepository<WorkgroupVendor> WorkgroupVendorRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkgroupVendorRepositoryTests"/> class.
        /// </summary>
        public WorkgroupVendorRepositoryTests()
        {
            WorkgroupVendorRepository = new Repository<WorkgroupVendor>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override WorkgroupVendor GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.WorkgroupVendor(counter);
            rtValue.Workgroup = Repository.OfType<Workgroup>().Queryable.First();

            return rtValue;
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<WorkgroupVendor> GetQuery(int numberAtEnd)
        {
            return WorkgroupVendorRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(WorkgroupVendor entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(WorkgroupVendor entity, ARTAction action)
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
            Repository.OfType<Workgroup>().DbContext.BeginTransaction();
            LoadOrganizations(3);
            LoadWorkgroups(3);
            Repository.OfType<Workgroup>().DbContext.CommitTransaction();

            WorkgroupVendorRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	

        #region Workgroup Tests

        [TestMethod]
        [ExpectedException(typeof (ApplicationException))]
        public void TestWorkgroupVendorsFieldWorkgroupWithAValueOfNullDoesNotSave()
        {
            WorkgroupVendor record = null;
            try
            {
                #region Arrange
                record = GetValid(9);
                record.Workgroup = null;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(record);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                Assert.AreEqual(record.Workgroup, null);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("The Workgroup field is required.");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof (TransientObjectException))]
        public void TestWorkgroupVendorNewWorkgroupDoesNotSave()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var record = GetValid(9);
                record.Workgroup = new Workgroup();
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(record);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: Purchasing.Core.Domain.Workgroup, Entity: Purchasing.Core.Domain.Workgroup", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestWorkgroupVendorWithExistingWorkgroupSaves()
        {
            #region Arrange
            var record = GetValid(9);
            record.Workgroup = Repository.OfType<Workgroup>().Queryable.Single(a => a.Id == 3);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(record);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(3, record.Workgroup.Id);
            Assert.IsFalse(record.IsTransient());
            Assert.IsTrue(record.IsValid());
            #endregion Assert		
        }

        #endregion Workgroup Tests

        #region VendorId Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the VendorId with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestVendorIdWithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.VendorId = "x".RepeatTimes((10 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(10 + 1, workgroupVendor.VendorId.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Kfs Vendor", "10"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the VendorId with null value saves.
        /// </summary>
        [TestMethod]
        public void TestVendorIdWithNullValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorId = null;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorId with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestVendorIdWithEmptyStringSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorId = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorId with one space saves.
        /// </summary>
        [TestMethod]
        public void TestVendorIdWithOneSpaceSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorId = " ";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorId with one character saves.
        /// </summary>
        [TestMethod]
        public void TestVendorIdWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorId = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorId with long value saves.
        /// </summary>
        [TestMethod]
        public void TestVendorIdWithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorId = "x".RepeatTimes(10);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(10, workgroupVendor.VendorId.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion VendorId Tests

        #region VendorAddressTypeCode Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the VendorAddressTypeCode with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestVendorAddressTypeCodeWithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.VendorAddressTypeCode = "x".RepeatTimes((4 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(4 + 1, workgroupVendor.VendorAddressTypeCode.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Vendor Address", "4"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the VendorAddressTypeCode with null value saves.
        /// </summary>
        [TestMethod]
        public void TestVendorAddressTypeCodeWithNullValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorAddressTypeCode = null;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorAddressTypeCode with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestVendorAddressTypeCodeWithEmptyStringSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorAddressTypeCode = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorAddressTypeCode with one space saves.
        /// </summary>
        [TestMethod]
        public void TestVendorAddressTypeCodeWithOneSpaceSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorAddressTypeCode = " ";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorAddressTypeCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestVendorAddressTypeCodeWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorAddressTypeCode = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the VendorAddressTypeCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestVendorAddressTypeCodeWithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.VendorAddressTypeCode = "x".RepeatTimes(4);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(4, workgroupVendor.VendorAddressTypeCode.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion VendorAddressTypeCode Tests

        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Name = null;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
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
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Name = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
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
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Name = " ";
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Name"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
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
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Name = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(40 + 1, workgroupVendor.Name.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Name", "40"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
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
            var workgroupVendor = GetValid(9);
            workgroupVendor.Name = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Name = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, workgroupVendor.Name.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region Line1 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Line1 with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithNullValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Line1 = null;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Line1"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Line1 with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithEmptyStringDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Line1 = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Line1"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Line1 with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithSpacesOnlyDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Line1 = " ";
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Line1"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Line1 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine1WithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Line1 = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(40 + 1, workgroupVendor.Line1.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Line1", "40"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Line1 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLine1WithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line1 = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line1 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLine1WithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line1 = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, workgroupVendor.Line1.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Line1 Tests

        #region Line2 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Line2 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine2WithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Line2 = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(40 + 1, workgroupVendor.Line2.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Line2", "40"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Line2 with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithNullValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line2 = null;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithEmptyStringSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line2 = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with one space saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithOneSpaceSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line2 = " ";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line2 = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line2 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLine2WithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line2 = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, workgroupVendor.Line2.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Line2 Tests

        #region Line3 Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Line3 with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestLine3WithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Line3 = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(40 + 1, workgroupVendor.Line3.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Line3", "40"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Line3 with null value saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithNullValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line3 = null;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with empty string saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithEmptyStringSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line3 = string.Empty;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with one space saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithOneSpaceSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line3 = " ";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with one character saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line3 = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Line3 with long value saves.
        /// </summary>
        [TestMethod]
        public void TestLine3WithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Line3 = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, workgroupVendor.Line3.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Line3 Tests

        #region City Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the City with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithNullValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.City = null;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithEmptyStringDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.City = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithSpacesOnlyDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.City = " ";
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "City"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the City with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCityWithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.City = "x".RepeatTimes((40 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(40 + 1, workgroupVendor.City.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "City", "40"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the City with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.City = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the City with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCityWithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.City = "x".RepeatTimes(40);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(40, workgroupVendor.City.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion City Tests

        #region State Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the State with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithNullValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.State = null;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithEmptyStringDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.State = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithSpacesOnlyDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.State = " ";
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "State"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the State with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestStateWithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.State = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(2 + 1, workgroupVendor.State.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "State", "2"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the State with one character saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.State = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the State with long value saves.
        /// </summary>
        [TestMethod]
        public void TestStateWithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.State = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, workgroupVendor.State.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion State Tests

        #region Zip Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Zip with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithNullValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Zip = null;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithEmptyStringDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Zip = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithSpacesOnlyDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Zip = " ";
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Zip"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Zip with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestZipWithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.Zip = "x".RepeatTimes((11 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(11 + 1, workgroupVendor.Zip.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Zip", "11"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Zip with one character saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Zip = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Zip with long value saves.
        /// </summary>
        [TestMethod]
        public void TestZipWithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.Zip = "x".RepeatTimes(11);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(11, workgroupVendor.Zip.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Zip Tests

        #region CountryCode Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the CountryCode with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCountryCodeWithNullValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.CountryCode = null;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Country Code"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the CountryCode with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCountryCodeWithEmptyStringDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.CountryCode = string.Empty;
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Country Code"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the CountryCode with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCountryCodeWithSpacesOnlyDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.CountryCode = " ";
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                var results = workgroupVendor.ValidationResults().AsMessageList();
                results.AssertErrorsAre(string.Format("The {0} field is required.", "Country Code"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the CountryCode with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCountryCodeWithTooLongValueDoesNotSave()
        {
            WorkgroupVendor workgroupVendor = null;
            try
            {
                #region Arrange
                workgroupVendor = GetValid(9);
                workgroupVendor.CountryCode = "x".RepeatTimes((2 + 1));
                #endregion Arrange

                #region Act
                WorkgroupVendorRepository.DbContext.BeginTransaction();
                WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                WorkgroupVendorRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(workgroupVendor);
                Assert.AreEqual(2 + 1, workgroupVendor.CountryCode.Length);
                var results = workgroupVendor.ValidationResults().AsMessageList();		
                results.AssertErrorsAre(string.Format("The field {0} must be a string with a maximum length of {1}.", "Country Code", "2"));
                Assert.IsTrue(workgroupVendor.IsTransient());
                Assert.IsFalse(workgroupVendor.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the CountryCode with one character saves.
        /// </summary>
        [TestMethod]
        public void TestCountryCodeWithOneCharacterSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.CountryCode = "x";
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the CountryCode with long value saves.
        /// </summary>
        [TestMethod]
        public void TestCountryCodeWithLongValueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.CountryCode = "x".RepeatTimes(2);
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(2, workgroupVendor.CountryCode.Length);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion CountryCode Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange
            WorkgroupVendor workgroupVendor = GetValid(9);
            workgroupVendor.IsActive = false;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(workgroupVendor.IsActive);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange
            var workgroupVendor = GetValid(9);
            workgroupVendor.IsActive = true;
            #endregion Arrange

            #region Act
            WorkgroupVendorRepository.DbContext.BeginTransaction();
            WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
            WorkgroupVendorRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsTrue(workgroupVendor.IsActive);
            Assert.IsFalse(workgroupVendor.IsTransient());
            Assert.IsTrue(workgroupVendor.IsValid());
            #endregion Assert
        }

        #endregion IsActive Tests

        #region DisplayName Tests

        [TestMethod]
        public void TestDisplayName()
        {
            #region Arrange
            var record = CreateValidEntities.WorkgroupVendor(8);
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.AreEqual("Name8 (Line18, City8, CA 95616, US)", record.DisplayName);
            #endregion Assert		
        }
        
        #endregion DisplayName Tests

        #region Constructor Tests

        [TestMethod]
        public void TestConstructor()
        {
            #region Arrange
            var record = new WorkgroupVendor();
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Assert.IsTrue(record.IsActive);
            #endregion Assert		
        }
        #endregion Constructor Tests
        
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
            expectedFields.Add(new NameAndType("City", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]",
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("CountryCode", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Country Code\")]",
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)2)]"
            }));
            expectedFields.Add(new NameAndType("DisplayName", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Line1", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("Line2", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("Line3", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)40)]"
            }));
            expectedFields.Add(new NameAndType("State", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)2)]"
            }));
            expectedFields.Add(new NameAndType("VendorAddressTypeCode", "System.String", new List<string>
            {      
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Vendor Address\")]",
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)4)]"
            }));
            expectedFields.Add(new NameAndType("VendorId", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.DisplayAttribute(Name = \"Kfs Vendor\")]",
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)10)]"
            }));
            expectedFields.Add(new NameAndType("Workgroup", "Purchasing.Core.Domain.Workgroup", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"                  
            }));
            expectedFields.Add(new NameAndType("Zip", "System.String", new List<string>
            {
                 "[System.ComponentModel.DataAnnotations.RequiredAttribute()]", 
                 "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)11)]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(WorkgroupVendor));

        }

        #endregion Reflection of Database.	
		
		
    }
}