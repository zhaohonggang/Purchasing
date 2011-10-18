﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Dapper;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using Purchasing.Web.Services;
using UCDArch.Data.NHibernate;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Helpers
{
    public class DbHelper
    {
        /// <summary>
        /// Resets the database to a consistant state for testing
        /// </summary>
        public static void ResetDatabase(bool demoMode = false)
        {
            var dbService = ServiceLocator.Current.GetInstance<IDbService>();

            // wipe all the tables
            WipeTables(dbService);

            // reset the seed values
            ReseedTables(dbService);

            // insert the all the static codes
            InsertOrderStatusCodes(dbService);
            InsertStates(dbService);
            InsertRoles(dbService);
            InsertOrderTypes(dbService);
            InsertShippingTypes(dbService);

            // insert the kfs data
            InsertOrganizations(dbService);
            InsertAccounts(dbService);
            InsertSubAccounts(dbService);
            InsertCommodityCodes(dbService);
            InsertUnitOfMeasures(dbService);
            InsertVendors(dbService);
            
            // insert the dev data
            var session = NHibernateSessionManager.Instance.GetSession();
            session.BeginTransaction();

            if (!demoMode)
            {
                InsertData(session);
            }
            else
            {
                InsertDemoData(session);   
            }
            

            session.Transaction.Commit();
        }

        /*
         * Preparation Functions
         */
        private static void WipeTables(IDbService dbService)
        {
            //First, delete all the of existing data
            var tables = new[]
                             {
                                 "ApprovalsXSplits", "Splits", "Approvals", "ConditionalApproval", "AutoApprovals",
                                 "LineItems", "OrderTracking", "Attachments", "ControlledSubstanceInformation", "Orders", "OrderTypes", "OrderStatusCodes",
                                 "ShippingTypes", "WorkgroupPermissions", "WorkgroupAccounts",
                                 "WorkgroupsXOrganizations", "WorkgroupVendors", "WorkgroupAddresses", "Workgroups",
                                 "Permissions", "UsersXOrganizations", "EmailPreferences", "Users", "Roles", "vAccounts"
                                 , "vOrganizations", "vVendorAddresses", "vVendors", "vCommodities", "vCommodityGroups", "UnitOfMeasures", "States", "vSubAccounts"
                             };

            using (var conn = dbService.GetConnection())
            {
                foreach (var table in tables)
                {
                    conn.Execute("delete from " + table);
                }
            }
        }

        private static void ReseedTables(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute("DBCC CHECKIDENT(workgroups, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(orders, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(workgroupvendors, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(workgroupaccounts, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(approvals, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(splits, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(ordertracking, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(lineitems, RESEED, 0)");
            }
        }

        /*
         * Static Codes
         */
        private static void InsertOrderStatusCodes(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into OrderStatusCodes ([Id],[Name],[Level], [IsComplete], [KfsStatus]) VALUES (@id,@name,@level, @isComplete, @kfsstatus)",
                    new[]
                        {
                            new { id="RQ", Name="Requester", Level=1, IsComplete=false, KfsStatus=false },
                            new { id="AP", Name="Approver", Level=2, IsComplete=false, KfsStatus=false},
                            new { id="CA", Name="Conditional Approval", Level=2, IsComplete=false, KfsStatus=false},
                            new { id="AM", Name="Account Manager", Level=3, IsComplete=false, KfsStatus=false},
                            new { id="PR", Name="Purchaser", Level=4, IsComplete=false, KfsStatus=false},
                            new { id="CN", Name="Complete-Not Uploaded KFS", Level=5, IsComplete=true, KfsStatus=false},
                            new { id="CP", Name="Complete", Level=5, IsComplete=true, KfsStatus=false}
                        });

                conn.Execute(@"update OrderStatusCodes set Level = null where Level = -1");
            }
        }

        private static void InsertStates(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into states (id, name) values (@id, @name)",
                    new[]
                        {
                            new {id = "AK", name = "ALASKA "},
                            new {id = "AL", name = "ALABAMA"},
                            new {id = "AR", name = "ARKANSAS"},
                            new {id = "AZ", name = "ARIZONA"},
                            new {id = "CA", name = "CALIFORNIA"},
                            new {id = "CO", name = "COLORADO"},
                            new {id = "CT", name = "CONNECTICUT"},
                            new {id = "DC", name = "DISTRICT OF COLUMBIA"},
                            new {id = "DE", name = "DELAWARE"},
                            new {id = "FL", name = "FLORIDA"},
                            new {id = "GA", name = "GEORGIA"},
                            new {id = "HI", name = "HAWAII "},
                            new {id = "IA", name = "IOWA"},
                            new {id = "ID", name = "IDAHO"},
                            new {id = "IL", name = "ILLINOIS"},
                            new {id = "IN", name = "INDIANA"},
                            new {id = "KS", name = "KANSAS "},
                            new {id = "KY", name = "KENTUCKY"},
                            new {id = "LA", name = "LOUISIANA"},
                            new {id = "MA", name = "MASSACHUSETTS"},
                            new {id = "MD", name = "MARYLAND"},
                            new {id = "ME", name = "MAINE"},
                            new {id = "MI", name = "MICHIGAN"},
                            new {id = "MN", name = "MINNESOTA"},
                            new {id = "MO", name = "MISSOURI"},
                            new {id = "MS", name = "MISSISSIPPI"},
                            new {id = "MT", name = "MONTANA"},
                            new {id = "NC", name = "NORTH CAROLINA"},
                            new {id = "ND", name = "NORTH DAKOTA"},
                            new {id = "NE", name = "NEBRASKA"},
                            new {id = "NH", name = "NEW HAMPSHIRE"},
                            new {id = "NJ", name = "NEW JERSEY"},
                            new {id = "NM", name = "NEW MEXICO"},
                            new {id = "NV", name = "NEVADA "},
                            new {id = "NY", name = "NEW YORK"},
                            new {id = "OH", name = "OHIO"},
                            new {id = "OK", name = "OKLAHOMA"},
                            new {id = "OR", name = "OREGON "},
                            new {id = "PA", name = "PENNSYLVANIA"},
                            new {id = "PR", name = "PUERTO RICO"},
                            new {id = "RI", name = "RHODE ISLAND"},
                            new {id = "SC", name = "SOUTH CAROLINA"},
                            new {id = "SD", name = "SOUTH DAKOTA"},
                            new {id = "TN", name = "TENNESSEE"},
                            new {id = "TX", name = "TEXAS"},
                            new {id = "UT", name = "UTAH"},
                            new {id = "VA", name = "VIRGINIA"},
                            new {id = "VI", name = "U.S. VIRGIN ISLANDS"},
                            new {id = "VT", name = "VERMONT"},
                            new {id = "WA", name = "WASHINGTON"},
                            new {id = "WI", name = "WISCONSIN"},
                            new {id = "WV", name = "WEST VIRGINIA"},
                            new {id = "WY", name = "WYOMING"}
                        }
                    );
            }
        }

        private static void InsertRoles(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {

                conn.Execute(@"insert into Roles ([Id], [Name], [Level]) VALUES (@id, @name, @level)",
                    new[]
                        {
                            new { id="AD", Name="Admin", Level = 0},
                            new { id="DA", Name="Departmental Admin", Level=0},
                            new { id="RQ", Name="Requester", Level = 1},
                            new { id="AR", Name="Approver", Level = 2},
                            new { id="AM", Name="Account Manager", Level = 3},
                            new { id="PR", Name="Purchaser", Level = 4}
                        });
            }
        }

        private static void InsertOrderTypes(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(@"insert into OrderTypes ([Id], [Name]) values (@id, @name)", new[]
                        {
                            new { id="OR", Name="Order Request" },
                            new { id="PR", Name="Purchase Request"},
                            new { id="DPO", Name="Departmental Purchase Order"},
                            new { id="DRO", Name="Departmental Repair Order"},
                            new { id="UCB", Name="UCD Buy Order"},
                            new { id="PC", Name = "Purchasing Card"}
                        });
            }
        }

        private static void InsertShippingTypes(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(@"insert into ShippingTypes ([Id], [Name], [Warning]) values (@id, @name, @warning)", new[]
                        {
                            new { id="ST", Name="Standard", Warning=string.Empty},
                            new { id="EX", Name="Expedited", Warning = string.Empty},
                            new { id="ON", Name="Overnight", Warning = "This shipping may cost a lot of money."}                                 
                        });
            }
        }

        /*
         * Kfs Data
         */
        /// <summary>
        /// Inserting in all the current APLS accounts
        /// </summary>
        /// <param name="dbService"></param>
        private static void InsertOrganizations(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vOrganizations ([Id],[Name],[TypeCode],[TypeName],[ParentId],[IsActive]) VALUES (@id,@name,@typecode,@typename,@parent,@active)",
                    new[]
                        {
                            new {id = "AAES", name = "COLLEGE OF AG & ENVIRONMENTAL SCIENCES", typecode = "1", typename = "", parent = (string)null, active = true},
                            new {id = "AABL", name = "CA&ES ANIMAL BIOLOGY", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AADM", name = "CA&ES ADMINISTRATION", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AAFP", name = "AQUACULTURE & FISHERIES PROGRAM", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "AANS", name = "ANIMAL SCIENCE", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "AARC", name = "ANIMAL AGRICULTURE RESEARCH CENTER", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "AARE", name = "AGRICULTURAL & RESOURCE ECONOMICS", typecode = "4   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AAVS", name = "AVIAN SCIENCES", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "ABAE", name = "BIOLOGICAL & AG ENGINEERING", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "ABML", name = "BODEGA MARINE LABORATORY", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "ACAB", name = "CENTER FOR AVIAN BIOLOGY", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "ACAW", name = "CENTER FOR ANIMAL WELFARE", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "ACBD", name = "AES CBS DEAN'S OFFICE", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "ACCR", name = "CENTER FOR CONSUMER RESEARCH", typecode = "4   ", typename = "", parent = "AHHD", active = true},
                            new {id = "ACEH", name = "CENTER FOR ECOLOGICAL HEALTH RESEARCH", typecode = "4   ", typename = "", parent = "AERS", active = true},
                            new {id = "ACEP", name = "CEPRAP", typecode = "4   ", typename = "", parent = "APSC", active = true},
                            new {id = "ACL1", name = "FOOD CHAIN cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL2", name = "METRO cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL3", name = "BFTV CLUSTER", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL4", name = "PHOENIX cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL5", name = "CHEDDAR cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACWU", name = "CA&ES COLLEGE WIDE UNITS", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "ADES", name = "ENVIRONMENTAL SCIENCE & POLICY (ESP)", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "ADNO", name = "DEAN'S OFFICE", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "AEDS", name = "ENVIRONMENTAL DESIGN", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AENT", name = "ENTOMOLOGY", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "AERS", name = "CA&ES ENV & RES SCIENCES & POLICY", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AETX", name = "ENVIRONMENTAL TOXICOLOGY", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "AEVE", name = "AES EVOLUTION & ECOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "AFDS", name = "FOUNDATION PLANT SERVICES", typecode = "4   ", typename = "", parent = "APSC", active = true},
                            new {id = "AFST", name = "FOOD SCIENCE & TECHNOLOGY", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AGED", name = "AGRICULTURAL EDUCATION", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "AHCD", name = "COMMUNITY & REGIONAL DEVELOPMENT", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AHCH", name = "HUMAN DEVELOPMENT AND FAMILY SERVICES", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AHHD", name = "CA&ES HUMAN HEALTH & DEVELOPMENT", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AHIS", name = "AGRICULTURAL HISTORY", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "ALAB", name = "UC DAVIS ANALYTICAL LAB", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "ALAR", name = "LANDSCAPE ARCHITECTURE", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "ALAW", name = "LAWR", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "AMCB", name = "AES MOLECULAR & CELLULAR BIOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "AMIC", name = "AES MICROBIOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "ANEM", name = "NEMATOLOGY", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "ANPB", name = "AES NEUROBIOLOGY, PHYSIOLOGY, & BEHAVI", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "ANUT", name = "NUTRITION", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "APLB", name = "AES PLANT BIOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "APLS", name = "PLANT SCIENCES", typecode = "D   ", typename = "", parent = "APSC", active = true},
                            new {id = "APPA", name = "PLANT PATHOLOGY", typecode = "D   ", typename = "", parent = "APSC", active = true},
                            new {id = "APRV", name = "PROVISION FOR RECRUITMENT", typecode = "4   ", typename = "", parent = "AADM", active = true},
                            new {id = "APSC", name = "CA&ES PLANT SCIENCES", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "ARSV", name = "CA&ES RESERVES", typecode = "4   ", typename = "", parent = "AADM", active = true},
                            new {id = "ASPG", name = "SPECIAL PROGRAMS", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "ATXC", name = "TEXTILES & CLOTHING", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AVIT", name = "VITCULTURE & ENOLOGY", typecode = "D   ", typename = "", parent = "APSC", active = true},
                            new {id = "AWFC", name = "WILDLIFE, FISH, & CONSERVATION BIOLOGY", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "AWRD", name = "CA&ES AWARDS", typecode = "4   ", typename = "", parent = "AADM", active = true},
                            new {id = "B000", name = "MCB:CLOSED OR DEAD ACCOUNTS", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BAFP", name = "ADULT FITNESS PROGRAM", typecode = "4   ", typename = "", parent = "BEXB", active = true},
                            new {id = "BCEF", name = "PB CONTROLLED ENVIRONMENT FACILITY", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BCGD", name = "CENTER FOR GENETICS AND DEVELOPMENT", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "BCNG", name = "CNS GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BCNS", active = true},
                            new {id = "BCPB", name = "CENTER FOR POPULATION BIOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BCPO", name = "CENTER FOR POPULATION BIOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BCPX", name = "CENTER FOR POPULATION BIOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BDNO", name = "CBS DEAN'S FUNDING ACCOUNTS", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BE00", name = "EXB CLOSED ACCOUNTS", typecode = "4   ", typename = "", parent = "BEXB", active = true},
                            new {id = "BEE0", name = "EDUCATION ENRICH & OUTREACH PROGRAMS", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BENG", name = "NCMHD CTR  EXCELLENCE IN NUTRI GENOMICS", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BETS", name = "MCB: ELECTRONICS SHOP", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BEVO", name = "EVOLUTION AND ECOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BEVX", name = "EVOLUTION AND ECOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BEXO", name = "EXERCISE BIOLOGY OPERATING ORG", typecode = "4   ", typename = "", parent = "BEXB", active = true},
                            new {id = "BGIF", name = "COLLEGE OF BIO SCI ENDOWMENTS & GIFTS", typecode = "D   ", typename = "", parent = "CBSD", active = true},
                            new {id = "BGRE", name = "PLANT BIOLOGY GREENHOUSE", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BHRB", name = "PLANT BIOLOGY HERBARIUM", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BINF", name = "BIOINFORMATICS", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "BMCO", name = "MCB OPERATING EXPENSES", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BMGG", name = "MICROBIOLOGY GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "BMIO", name = "MICROBIOLOGY", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "BN00", name = "NPB CLOSED ACCOUNTS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "BNPO", name = "NPB GEN OPERATING ORG", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "BOTA", name = "PLANT BIOLOGY CONSERVATORY", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BPBG", name = "POPULATION BIOLOGY GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BPGG", name = "PLANT BIOLOGY GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BPIA", name = "PLANT BIOLOGY-INACTIVE FOR CLOSED ACCTS", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BPLO", name = "PLANT BIOLOGY OPERATING FUNDS", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BSBB", name = "COLLEGE OF BIO SCI BUDGET BALANCE", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BSCW", name = "CBS COLLEGE-WIDE EXPENDITURE ACCTS", typecode = "D   ", typename = "", parent = "CBSD", active = true},
                            new {id = "BSDF", name = "DEAN'S OFFICE COLLEGE OF BIOL SCIENCES", typecode = "D   ", typename = "", parent = "CBSD", active = true},
                            new {id = "BSRD", name = "CBS DEAN'S 10-11 I&R REDUCTIONS", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BSRE", name = "CBS RETENTION FUNDING", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BSSU", name = "CBS FACULTY START-UP FUNDING", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "EXPR", name = "GENOME AND BIOMEDICAL SCIENCES", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GBSF", name = "GENOME AND BIOMEDICAL SCIENCES FACILITY", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GCCF", name = "GENOME CENTER CORE FACILITIES", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GCOP", name = "GENOME CENTER OPERATING FUNDS", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GENO", name = "GENOME AND BIOMEDICAL SCIENCES", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "MOLD", name = "MICROBIOLOGY", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "NCNS", name = "ORG FOR CENTER FOR NEUROSCIENCE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAG", name = "ORG FOR ALDRIN GOMES", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAH", name = "ORG FOR ANN HEDRICK", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAI", name = "ORG FOR ANDY ISHIDA", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAM", name = "ORG FOR ALEX MOGILNER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAS", name = "ORG FOR ARNIE SILLMAN", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPBH", name = "ORG FOR BARBARA HORWITZ", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPBM", name = "ORG FOR BRIAN MULLONEY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPCF", name = "NEW ORG CHUCK FULLER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPCW", name = "ORG FOR CRAIG WARDEN", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPDF", name = "NEW ORG FOR DAVE FURLOW", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPDH", name = "ORG FOR DAVID HAWKINS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPDW", name = "ORG FOR DAVE WARLAND", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPEB", name = "ORG FOR ERWIN BAUTISTA", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPEC", name = "ORG FOR EARL CARSTENS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPEW", name = "ORG FOR DOROTHY E. WOOLLEY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPGN", name = "NEW ORG FOR GABY NEVITT", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPGR", name = "ORG FOR GRACE ROSENQUIST", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJG", name = "ORG FOR JACK GOLDBERG", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJH", name = "ORG FOR JOHN HOROWITZ", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJS", name = "ORG FOR JIM SHAFFATH", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJT", name = "ORG FOR JAMES TRIMMER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJW", name = "ORG FOR JOHN WINGFIELD", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPKB", name = "ORG FOR KEITH BAAR", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPKW", name = "ORG FOR KEITH WILLIAMS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPLC", name = "ORG FOR LEO M CHALUPA", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPMR", name = "ORG FOR MARILYN RAMNOFSKY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPMW", name = "ORG FOR MARTIN WILSON", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPPP", name = "ORG FOR PAM PAPPONE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPPS", name = "ORG FOR PAUL SALITSKY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPPW", name = "ORG FOR PHYLLIS WISE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPRE", name = "ORG FOR GREGG RECANZONE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPSB", name = "ORG FOR SUE BODINE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPSD", name = "ORG FOR STUDENT FELLOWSHIP", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPSH", name = "ORG FOR SAMANTHA HARRIS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPTH", name = "ORG FOR THOMAS COOMS-HAHN", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPWW", name = "ORG FOR JEFF W. WEIDNER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "PLBI", name = "PLANT BIOLOGY INACTIVE ACCTS", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "PRSL", name = "MOLECULAR STRUCTURE FACILITY", typecode = "4   ", typename = "", parent = "BGEN", active = true}
                        }
                    );
            }
        }

        private static void InsertAccounts(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vAccounts ([Id],[Name],[IsActive],[AccountManager],[AccountManagerID],[PrincipalInvestigator],[PrincipalInvestigatorID],[OrganizationId]) VALUES (@id,@name,@active,@manager,@managerid,@pi,@piid,@org)",
                    new[]
                        {
                            new {id = "3-6851000", name = "UNEXPENDED BALANCE: AG & ES: CONFERENCES", active = true, managerid = "VETTE", manager = "MADDERRA,DEIDRA A", pi = "", piid = "", org = "APLS"},
                            new {id = "3-APSAC37", name = "HUTMACHER: COTTON INC. 04-448CA", active = true, managerid = "CDMILLS", manager = "MILLS,CAROL D.", pi = "HUTMACHER,ROBERT B", piid = "RBHUTMAC", org = "APLS"},
                            new {id = "3-APSAR24", name = "PS:RRB:RM-2:2011:HILL", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "HILL,JAMES E", piid = "JEHILL", org = "APLS"},
                            new {id = "3-APSD102", name = "CRB: Generalized Reagentless Sensor to D", active = true, managerid = "SSSSING", manager = "SINGH-VIDAL,SALLY", pi = "DANDEKAR,ABHAYA M", piid = "FZDANDE", org = "APLS"},
                            new {id = "3-APSE095", name = "F. JACKSON HILLS MEMORIAL SCHOLARSHIP", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "", piid = "", org = "APLS"},
                            new {id = "3-APSF376", name = "PS:USDA:GLOBAL RES ALL:58-3148-0-181:SIX", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "SIX,JOHAN W.U.A.", piid = "JOSIX", org = "APLS"},
                            new {id = "3-APSM077", name = "JACKSON:CCIA EVALUATION OF SMALL GRAINS", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "JACKSON,LELAND F", piid = "X#056112", org = "APLS"},
                            new {id = "3-APSM152", name = "PUTNAM:CCIA:EXPERIMENTAL VARIETY AND GER", active = true, managerid = "SZDLCHAV", manager = "CHAVEZ,DANA L", pi = "PUTNAM,DANIEL H", piid = "FZPUTNAM", org = "APLS"},
                            new {id = "3-APSM170", name = "CLOSE ACCT / CCIA / SEED PHYSIOL", active = true, managerid = "BRITTNI", manager = "REID,SERENA N", pi = "BRADFORD,KENT J", piid = "KJB123", org = "APLS"},
                            new {id = "3-APSM326", name = "PUTNAM:CCIA:ALFALFA EXPERIMENTAL VARIETY", active = true, managerid = "SZDLCHAV", manager = "CHAVEZ,DANA L", pi = "PUTNAM,DANIEL H", piid = "FZPUTNAM", org = "APLS"},
                            new {id = "3-APSO013", name = "PS:DRY BEAN FIELD DAY:TEMPLE", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "TEMPLE,STEVEN R", piid = "SZTEMPLE", org = "APLS"},
                            new {id = "3-APSPR12", name = "FERGUSON:CA PISTACHIO RES BRD:DEVELOPING", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "FERGUSON,LOUISE", piid = "WEEZIE", org = "APLS"},
                            new {id = "3-APSPR15", name = "PS:CA PISTACHIO:BLOOMCAST:FERGUSON", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "FERGUSON,LOUISE", piid = "WEEZIE", org = "APLS"},
                            new {id = "3-APSRSTR", name = "PLANT SCIENCE PROFESS DEVELOPMENT AWARDS", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "", piid = "", org = "APLS"},
                            new {id = "3-GENAKH2", name = "PS: HJELMELAND: JASTRO SHIELDS AWD", active = true, managerid = "JOHNSTON", manager = "MORGAN,SABRINA", pi = "HJELMELAND,ANNA K", piid = "AKHAKH", org = "APLS"},
                            new {id = "3-PR68510", name = "PROVISION FOR ALLOCATION", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "", piid = "", org = "APLS"},
                            new {id = "L-APSAC20", name = "COTTON INC:HUTMACHER MCA:WRIGHT", active = true, managerid = "KAWAKAMI", manager = "KAWAKAMI,HEATHER ERIKA", pi = "HUTMACHER,ROBERT B", piid = "RBHUTMAC", org = "APLS"},
                            new {id = "L-APSRSTR", name = "STAFF TRAINING AND DEVELOPMENT FUNDING", active = true, managerid = "VETTE", manager = "MADDERRA,DEIDRA A", pi = "", piid = "", org = "APLS"}
                        }
                    );
            }
        }

        private static void InsertSubAccounts(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vSubAccounts ([AccountNumber],[SubAccountNumber],[Name]) VALUES (@account,@subaccount,@name)",
                    new[]
                        {
                            new { account = "3-APSAC37", subaccount = "FRBH2", name = "HUTMACHER: COTTON INC; 04-448CA"},
                            new { account = "3-APSAR24", subaccount = "FJEH2", name = "JAMES E HILL - RRB RM-2 2011"},
                            new { account = "3-APSF376", subaccount = "FJWS2", name = "SIX - HUNG DAM"},
                            new { account = "3-APSM077", subaccount = "AMSHR", name = "CCIA:SHARED EQUIPMENT"},
                            new { account = "3-APSM077", subaccount = "FLFJ2", name = "JACKSON:EVALUATION OF SMALL GRAINS IN CA"},
                            new { account = "3-APSM152", subaccount = "AMSHR", name = "PUTNAM:SHARED EQUIPMENT"},
                            new { account = "3-APSM152", subaccount = "FDHP2", name = "PUTNAM:CCIA:ALFALFA EXPERIMENTAL VARIETY"},
                            new { account = "3-APSM170", subaccount = "FKJB2", name = "KENT BRADFORD / CCIA / SEED PHYSIOLOGY R"},
                            new { account = "3-APSM170", subaccount = "FKJB3", name = "KENT BRADFORD / CCIA / SEED PHYSIOLOGY R"},
                            new { account = "3-APSM326", subaccount = "AMSHR", name = "CCIA:SHARED EQUIPMENT"},
                            new { account = "3-APSM326", subaccount = "FDHP2", name = "PUTNAM:CCIA:ALFALFA EXPERIMENTAL VARIETY"},
                            new { account = "3-APSPR12", subaccount = "FLXF2", name = "FERGUSON:CA PISTACHIO RES BRD:DEVELOPING"},
                            new { account = "3-APSPR15", subaccount = "FLXF2", name = "LOUISE FERGUSON - BLOOMCAST"},
                            new { account = "3-APSRSTR", subaccount = "AEMST", name = "PLANT SCIENCES STAFF TRAINING FUNDS"},
                            new { account = "3-GENAKH2", subaccount = "GAKH2", name = "DE-ACTIVATED"}
                        }
                    );
            }
        }

        private static void InsertCommodityCodes(IDbService dbService)
        {
            //Getting commodity groups and codes for groups 10 & 40
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vCommodityGroups ([GroupCode],[Name],[SubGroupCode],[SubGroupName]) VALUES (@groupcode,@name,@subgroupcode,@subgroupname)",
                    new[]
                        {
                            new {groupcode = "10", name = "Hardware", subgroupcode = "11", subgroupname = "Paint and Supplies"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "14", subgroupname = "Metals"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "18", subgroupname = "Carpentry, Hardware"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "99", subgroupname = "Miscellaneous"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "EQ", subgroupname = "Motors, Generators & Miscellaneous"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "26", subgroupname = "Plumbing"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "35", subgroupname = "Tools (Non Inventorial)"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "32", subgroupname = "Electrical"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "00", subgroupname = "Chemicals, Miscellaneous"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "EQ", subgroupname = "Equipment"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "13", subgroupname = "Gases, Compressed"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "50", subgroupname = "Plasticware"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "59", subgroupname = "Furniture"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "99", subgroupname = "Miscellaneous"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "40", subgroupname = "Glassware & Ceramics"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "14", subgroupname = "Gases, Liquid"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "SE", subgroupname = "Non-Inventorial Equipment"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "19", subgroupname = "Carpentry, Lumber"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "20", subgroupname = "Carpentry, Floor & Wall Materials"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "21", subgroupname = "Carpentry, Ladders & Scaffolding"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "40", subgroupname = "Lighting Fixtures & Accessories"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "11", subgroupname = "Measuring & Testing Devices"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "50", subgroupname = "Adhesives and Sealants"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "RE", subgroupname = "Rental"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "01", subgroupname = "Chemicals, Restricted"}
                        });

                conn.Execute(
                    @"insert into vCommodities ([Id],[Name],[GroupCode],[SubGroupCode]) VALUES (@id,@name,@groupcode,@subgroupcode)",
                    new[]
                        {
                            new {id = "13165", name = "ADHESIVE HARDENER/ACCELERATOR", groupcode = "10", subgroupcode = "50"},
                            new {id = "14051", name = "CARBON STEEL, FORMED, SLIDING DOOR TRACK", groupcode = "10", subgroupcode = "18"},
                            new {id = "32490", name = "POWER SOURCE, NOT 32400-32418, INCL BIN OR VOLTAGE CLAMP", groupcode = "10", subgroupcode = "32"},
                            new {id = "11700", name = "STAIN/DYE", groupcode = "10", subgroupcode = "11"},
                            new {id = "33453", name = "CAP/RECEPTACLE/BODY, 4 WIRE, 30A/125/250/600V & 20A/250V", groupcode = "10", subgroupcode = "32"},
                            new {id = "48371", name = "ELECTRICAL CURVE TRACER & ACCESSORIES", groupcode = "10", subgroupcode = "32"},
                            new {id = "19341-1", name = "MORTAR, GROUNDS", groupcode = "10", subgroupcode = "99"},
                            new {id = "27220", name = "VALVE, DIRECTION CONTROL, NOT SOLENOID", groupcode = "10", subgroupcode = "26"},
                            new {id = "1XXXX", name = "PRIMARY MATERIALS, LUMBER/GLASS/METALS/ ROPE/WIRE", groupcode = "10", subgroupcode = "99"},
                            new {id = "36332", name = "PLANING/SHAPING, SCRAPER, INCL PUTTY KNIFE", groupcode = "10", subgroupcode = "35"},
                            new {id = "36711", name = "TONGS, NONPOWERED", groupcode = "10", subgroupcode = "35"},
                            new {id = "12060", name = "LUBRICANT FOR SPECIFIC FUNCTIONS, NOT PULLING COMPOUND", groupcode = "10", subgroupcode = "99"},
                            new {id = "3XXXX", name = "GENERAL PURPOSE HARDWARE & TOOLS", groupcode = "10", subgroupcode = "35"},
                            new {id = "32170", name = "LAMP/TUBE, FLUORESCENT, STARTER REQ, BIAXIAL OR TWIN TUBE", groupcode = "10", subgroupcode = "32"},
                            new {id = "C6174-1", name = "VACUUM SYSTEM, INCL VACUUM CHAMBER", groupcode = "10", subgroupcode = "EQ"},
                            new {id = "27126", name = "GLOBE VALVE, ANGLE", groupcode = "10", subgroupcode = "26"},
                            new {id = "18037", name = "WOOD, MAPLE, TRIM/FINISH", groupcode = "10", subgroupcode = "18"},
                            new {id = "C6171-06", name = "PUMP, POSITIVE DISPLACEMENT/PROGRESSIVE CAVITY, INDUSTRIAL", groupcode = "10", subgroupcode = "EQ"},
                            new {id = "32191", name = "LAMP/TUBE, FLUORESCENT, NO STARTER REQ, MEDIUM BIPIN", groupcode = "10", subgroupcode = "32"},
                            new {id = "D8632-2", name = "SOLDERING/DESOLDERING KIT/SYSTEM, POWERED, INDUSTRIAL", groupcode = "10", subgroupcode = "EQ"},
                            new {id = "40053-57", name = "CITRONELLOL, 95% 100ML 106-22-9", groupcode = "40", subgroupcode = "00"},
                            new {id = "40024-82", name = "CARBON ACTIVATED USP POW 2.5KG", groupcode = "40", subgroupcode = "00"},
                            new {id = "40152-19", name = "ETHYLENE GLYCOL, P.A.  1L 107-21-1", groupcode = "40", subgroupcode = "00"},
                            new {id = "43600-33", name = "CARCINOGEN, TALC CONTAINING ASBESTIFORM FIBERS, CLASS II", groupcode = "40", subgroupcode = "00"},
                            new {id = "E8143-1", name = "LINEAR DISPLACEMENT ENCODER/SENSOR, LAB", groupcode = "40", subgroupcode = "EQ"},
                            new {id = "40035-63", name = "PROPYLENE GLYCOL USP/FCC 4L 57-55-6", groupcode = "40", subgroupcode = "00"},
                            new {id = "40054-44", name = "CYCLOHEXENE SULFIDE, TEC   5GR 286-28-2", groupcode = "40", subgroupcode = "00"},
                            new {id = "46325", name = "MICROSCOPE, OPTICS, ILLUMINATOR/CONDENSER", groupcode = "40", subgroupcode = "99"},
                            new {id = "40151-67", name = "3,4-DICHLOROBENZALDEHYDE 25GR 6287-38-3", groupcode = "40", subgroupcode = "00"},
                            new {id = "40128-99", name = "ACETIC-D3 ACID-D, 100.0    5GR 1186-52-3", groupcode = "40", subgroupcode = "00"},
                            new {id = "40091-80", name = "2-NAPHTHALENESULFONYL CH 100GR 93-11-8", groupcode = "40", subgroupcode = "00"},
                            new {id = "40135-39", name = "BARIUM SULFATE EXTRA PU 500GR 7727-43-7", groupcode = "40", subgroupcode = "00"},
                            new {id = "46271", name = "BULB, RUBBER/PLASTIC/LATEX, LAB", groupcode = "40", subgroupcode = "99"},
                            new {id = "40096-25", name = "5-METHOXYSALICYLIC ACID,  10GR 2612-02-4", groupcode = "40", subgroupcode = "00"},
                            new {id = "40070-17", name = "1-NAPHTHYLACETIC ACID, 9 100GR 86-87-3", groupcode = "40", subgroupcode = "00"},
                            new {id = "40104-09", name = "4-NITRO-M-XYLENE, 99% 100GR 89-87-2", groupcode = "40", subgroupcode = "00"},
                            new {id = "40098-87", name = "DECANOIC ACID, 99+%      500GR 334-48-5", groupcode = "40", subgroupcode = "00"},
                            new {id = "40145-95", name = "LITHIUM DIISOPROPYLAMIDE 100ML 4111-54-0", groupcode = "40", subgroupcode = "00"},
                            new {id = "40131-85", name = "PHENYL CHLOROFORMATE, 97 250ML 1885-14-9", groupcode = "40", subgroupcode = "00"},
                            new {id = "40017-14", name = "ALUM CHLOR HEX USP CRYST 500GM", groupcode = "40", subgroupcode = "00"}
                        });
            }
        }

        private static void InsertUnitOfMeasures(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into UnitOfMeasures ([Id], [Name]) VALUES (@id, @name)",
                    new[]
                        {
                            new {id="BT", name="BOTTLE"},
                            new {id="BX", name="BOX"},
                            new {id="C", name="HUNDRED"},
                            new {id="CM", name="CENTIMETER"},
                            new {id="CS", name="CASE"},
                            new {id="CT", name="CARTON"},
                            new {id="DA", name="DAY"},
                            new {id="DR", name="DRUM"},
                            new {id="DZ", name="DOZEN"},
                            new {id="EA", name="EACH"},
                            new {id="FT", name="FOOT"},
                            new {id="G", name="GRAM"},
                            new {id="GA", name="GALLON"},
                            new {id="GR", name="GROSS"},
                            new {id="HR", name="HOUR"},
                            new {id="IN", name="INCH"},
                            new {id="JR", name="JAR"},
                            new {id="KG", name="KILOGRAM"},
                            new {id="KT", name="KIT"},
                            new {id="L", name="LITER"},
                            new {id="LB", name="POUND"},
                            new {id="LF", name="LINEAL FEET"},
                            new {id="LG", name="LENGTH"},
                            new {id="LT", name="LOT"},
                            new {id="LY", name="LINEAL YARDS"},
                            new {id="M", name="THOUSAND"},
                            new {id="MG", name="MILLIGRAM"},
                            new {id="ML", name="MILLILITER"},
                            new {id="MM", name="MILLIMETER"},
                            new {id="MO", name="MONTH"},
                            new {id="MT", name="METER"},
                            new {id="OZ", name="OUNCE"},
                            new {id="PCS", name="PIECES"},
                            new {id="PK", name="PACKAGE"},
                            new {id="PL", name="PAIL"},
                            new {id="PR", name="PAIR"},
                            new {id="PT", name="PINT"},
                            new {id="QT", name="QUART"},
                            new {id="RL", name="ROLL"},
                            new {id="RM", name="REAM"},
                            new {id="SH", name="SHEET"},
                            new {id="SL", name="SLEEVE"},
                            new {id="SP", name="SPOOL"},
                            new {id="SQ", name="SQ FOOT"},
                            new {id="ST", name="SET"},
                            new {id="SY", name="SQ YARD"},
                            new {id="TN", name="TON"},
                            new {id="TU", name="TUBE"},
                            new {id="UL", name="MICROLITER"},
                            new {id="UN", name="UNIT"},
                            new {id="VL", name="VIAL"},
                            new {id="WK", name="WEEK"},
                            new {id="YD", name="YARD"},
                            new {id="YR", name="YEAR"}
                        });
            }
        }

        private static void InsertVendors(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                //Adding in 25 random vendors
                conn.Execute(@"insert into vVendors ([Id],[Name],[OwnershipCode],[BusinessTypeCode]) VALUES (@id,@name,@ownership,@business)",
                    new[]
                        {
                            new {id = "0000247673", name = "ELLIOTT & NELSON", ownership = "U", business = "S"},
                            new {id = "0000005439", name = "HOGUE & ASSOCIATES INC", ownership = "C", business = "S"},
                            new {id = "0000057767", name = "SEA CHALLENGERS", ownership = "C", business = "S"},
                            new {id = "0000006829", name = "PENINSULA VALVE & FITTING INC", ownership = "C", business = "S"},
                            new {id = "0000006831", name = "PENN-AIR & HYDRAULICS CORPORATION", ownership = "O", business = "S"},
                            new {id = "0000006836", name = "PENN TOOL COMPANY", ownership = "H", business = "S"},
                            new {id = "0000006832", name = "PACIFIC DOOR & CLOSER COMPANY INC", ownership = "C", business = "S"},
                            new {id = "0000006837", name = "PENTERA INC", ownership = "C", business = "S"},
                            new {id = "0000006842", name = "PINNACLE BAY RESOURCE GROUP INC", ownership = "W", business = "S"},
                            new {id = "0000006848", name = "PINPOINT SOLUTIONS", ownership = "C", business = "S"},
                            new {id = "0000006847", name = "PACIFIC ENVIRONMENTAL CONTROL INC", ownership = "C", business = "S"},
                            new {id = "0000006849", name = "PERCIVAL SCIENTIFIC INC", ownership = "W", business = "S"},
                            new {id = "0000006853", name = "PERKINS WELDING WORKS", ownership = "C", business = "S"},
                            new {id = "0000006855", name = "PINOEER COATINGS COMPANY", ownership = "C", business = "S"},
                            new {id = "0000006856", name = "PERROTT DESKTOP PUBLISHING", ownership = "W", business = "S"},
                            new {id = "0000006859", name = "PERRYS ART SUPPLIES & FRAMING", ownership = "H", business = "S"},
                            new {id = "0000006867", name = "PET CETERA", ownership = "C", business = "S"},
                            new {id = "0000008573", name = "WINE PUBLICATIONS", ownership = "W", business = "S"},
                            new {id = "0000008574", name = "WOODS & POOLE ECONOMICS INC", ownership = "C", business = "S"},
                            new {id = "0000008575", name = "Z D WINES", ownership = "C", business = "S"},
                            new {id = "0000008577", name = "WOODWORKERS STORE", ownership = "W", business = "S"},
                            new {id = "0000008578", name = "V W EIMICKE ASSOCIATES INC", ownership = "W", business = "S"},
                            new {id = "0000008580", name = "ZELTEX INC", ownership = "H", business = "S"},
                            new {id = "0000008579", name = "THE YARDAGE SHOP", ownership = "W", business = "S"},
                            new {id = "0000008583", name = "ZENON COMPUTER SYSTEM INC", ownership = "A", business = "S"}
                        }
                    );

                //Adding the 43 addresses for those 25 vendors
                conn.Execute(
                    @"insert into vVendorAddresses ([VendorId],[TypeCode],[Name],[Line1],[Line2],[Line3],[City],[State],[Zip],[CountryCode]) 
                                            VALUES (@id,@typecode,@name,@line1,@line2,@line3,@city,@state,@zip,@country)",
                    new[]
                        {
                            new {id = "0000006849", typecode = "0005", name = "PERCIVAL SCIENTIFIC INC", line1 = "PO BOX 18", line2 = "", line3 = "", city = "DES MOINES", state = "IA", zip = "50301", country = "US"},
                            new {id = "0000008573", typecode = "0002", name = "WINE PUBLICATIONS", line1 = "ELIZABETH MARCUS", line2 = "", line3 = "", city = "BERKELEY", state = "CA", zip = "94708", country = "US"},
                            new {id = "0000006832", typecode = "0002", name = "PACIFIC DOOR & CLOSER COMPANY INC", line1 = "2112 ADAMS AVE", line2 = "", line3 = "", city = "SAN LEANDRO", state = "CA", zip = "94577", country = "US"},
                            new {id = "0000008573", typecode = "0001", name = "WINE PUBLICATIONS", line1 = "96 PARNASSUS RD", line2 = "", line3 = "", city = "BERKELEY", state = "CA", zip = "94708", country = "US"},
                            new {id = "0000008574", typecode = "0001", name = "WOODS & POOLE ECONOMICS INC", line1 = "1794 COLUMBIA RD NW", line2 = "", line3 = "", city = "WASHINGTON", state = "DC", zip = "20009-2808", country = "US"},
                            new {id = "0000008575", typecode = "0001", name = "Z D WINES", line1 = "8383 SILVERADO TRAIL", line2 = "", line3 = "", city = "NAPA", state = "CA", zip = "94558", country = "US"},
                            new {id = "0000008577", typecode = "0001", name = "WOODWORKERS STORE", line1 = "4365 WILLOW DR", line2 = "", line3 = "", city = "MEDINA", state = "MN", zip = "55340-9701", country = "US"},
                            new {id = "0000008578", typecode = "0001", name = "V W EIMICKE ASSOCIATES INC", line1 = "PO BOX 160", line2 = "", line3 = "", city = "BRONXVILLE", state = "NY", zip = "10708", country = "US"},
                            new {id = "0000008580", typecode = "0001", name = "ZELTEX INC", line1 = "130 WESTERN MARYLAND PKWY", line2 = "", line3 = "", city = "HAGERSTOWN", state = "MD", zip = "21740", country = "US"},
                            new {id = "0000006831", typecode = "0002", name = "PENN-AIR & HYDRAULICS CORPORATION", line1 = "PO BOX 132", line2 = "", line3 = "", city = "YORK", state = "PA", zip = "17405", country = "US"},
                            new {id = "0000005439", typecode = "0001", name = "HOGUE & ASSOCIATES INC", line1 = "550 KEARNY ST", line2 = "", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000006842", typecode = "0001", name = "PINNACLE BAY RESOURCE GROUP INC", line1 = "2934 GOLD PAN CT", line2 = "", line3 = "", city = "RANCHO CORDOVA", state = "CA", zip = "95670", country = "US"},
                            new {id = "0000006855", typecode = "0001", name = "PINOEER COATINGS COMPANY", line1 = "10054 D MILL STATION RD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95827", country = "US"},
                            new {id = "0000006859", typecode = "0001", name = "PERRYS ART SUPPLIES & FRAMING", line1 = "128 GREENSFIELD AVE", line2 = "", line3 = "", city = "SAN ANSELMO", state = "CA", zip = "94960", country = "US"},
                            new {id = "0000006853", typecode = "0001", name = "PERKINS WELDING WORKS", line1 = "8524 FLORIN RD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95829", country = "US"},
                            new {id = "0000006853", typecode = "0002", name = "PERKINS WELDING WORKS", line1 = "PO BOX 292580", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95829", country = "US"},
                            new {id = "0000006836", typecode = "0001", name = "PENN TOOL COMPANY", line1 = "1776 SPRINGFIELD AVE", line2 = "", line3 = "", city = "MAPLEWOOD", state = "NJ", zip = "07040", country = "US"},
                            new {id = "0000006829", typecode = "0001", name = "PENINSULA VALVE & FITTING INC", line1 = "1260 PEAR AVE", line2 = "", line3 = "", city = "MOUNTAIN VIEW", state = "CA", zip = "94043", country = "US"},
                            new {id = "0000006831", typecode = "0001", name = "PENN-AIR & HYDRAULICS CORPORATION", line1 = "1750 INDUSTRIAL WY", line2 = "", line3 = "", city = "YORK", state = "PA", zip = "17402", country = "US"},
                            new {id = "0000006837", typecode = "0001", name = "PENTERA INC", line1 = "8650 COMMERCE PARK PL", line2 = "", line3 = "", city = "INDIANAPOLIS", state = "IN", zip = "46268", country = "US"},
                            new {id = "0000006832", typecode = "0001", name = "PACIFIC DOOR & CLOSER COMPANY INC", line1 = "395 MENDELL ST", line2 = "", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "94124", country = "US"},
                            new {id = "0000008579", typecode = "0001", name = "THE YARDAGE SHOP", line1 = "3016 J ST", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000008577", typecode = "0002", name = "WOODWORKERS STORE", line1 = "PO BOX 500", line2 = "", line3 = "", city = "MEDINA", state = "MN", zip = "55340", country = "US"},
                            new {id = "0000006849", typecode = "0001", name = "PERCIVAL SCIENTIFIC INC", line1 = "1805 E FOURTH ST", line2 = "", line3 = "", city = "BOONE", state = "IA", zip = "50036-0249", country = "US"},
                            new {id = "0000006849", typecode = "0004", name = "PERCIVAL SCIENTIFIC INC", line1 = "505 RESEARCH DR", line2 = "", line3 = "", city = "PERRY", state = "IA", zip = "50220", country = "US"},
                            new {id = "0000006848", typecode = "0001", name = "PINPOINT SOLUTIONS", line1 = "9647 FOLSOM BLVD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95827", country = "US"},
                            new {id = "0000006856", typecode = "0001", name = "PERROTT DESKTOP PUBLISHING", line1 = "17560 HIGHLANDS BLVD", line2 = "", line3 = "", city = "SONOMA", state = "CA", zip = "95476", country = "US"},
                            new {id = "0000006856", typecode = "0002", name = "PERROTT DESKTOP PUBLISHING", line1 = "PO BOX 296", line2 = "", line3 = "", city = "SONOMA", state = "CA", zip = "95476", country = "US"},
                            new {id = "0000006867", typecode = "0001", name = "PET CETERA", line1 = "612 4TH ST", line2 = "", line3 = "", city = "DAVIS", state = "CA", zip = "95616", country = "US"},
                            new {id = "0000006847", typecode = "0001", name = "PACIFIC ENVIRONMENTAL CONTROL INC", line1 = "3420 FOSTORIA WAY", line2 = "", line3 = "", city = "SAN RAMON", state = "CA", zip = "94583", country = "US"},
                            new {id = "0000057767", typecode = "0001", name = "SEA CHALLENGERS", line1 = "35 VERSAILLES CT", line2 = "", line3 = "", city = "DANVILLE", state = "CA", zip = "94506-4454", country = "US"},
                            new {id = "0000005439", typecode = "0005", name = "HOGUE & ASSOCIATES INC", line1 = "C/O HOGUE & ASSOCIATES INC", line2 = "", line3 = "", city = "EAST GREENVILLE", state = "PA", zip = "18041", country = "US"},
                            new {id = "0000006849", typecode = "0002", name = "PERCIVAL SCIENTIFIC INC", line1 = "C/O HART/LATIMER ASSOCIATES INC", line2 = "", line3 = "", city = "SAN CARLOS", state = "CA", zip = "94070", country = "US"},
                            new {id = "0000057767", typecode = "0002", name = "SEA CHALLENGERS", line1 = "5091 DEBBIE CRT", line2 = "", line3 = "", city = "GIG HARBOR", state = "WA", zip = "98335", country = "US"},
                            new {id = "0000006849", typecode = "0003", name = "PERCIVAL SCIENTIFIC INC", line1 = "PO BOX 249", line2 = "", line3 = "", city = "BOONE", state = "IA", zip = "50036", country = "US"},
                            new {id = "0000005439", typecode = "0004", name = "HOGUE & ASSOCIATES INC", line1 = "PO BOX 841366", line2 = "", line3 = "", city = "DALLAS", state = "TX", zip = "75284-1366", country = "US"},
                            new {id = "0000005439", typecode = "0002", name = "HOGUE & ASSOCIATES INC", line1 = "1515 30 TH ST", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", typecode = "0007", name = "HOGUE & ASSOCIATES INC", line1 = "7300 FOLSOM BLVD STE 103", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95826", country = "US"},
                            new {id = "0000005439", typecode = "0008", name = "HOGUE & ASSOCIATES INC", line1 = "GUNLOCKE/E&I CNR01172", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", typecode = "0003", name = "HOGUE & ASSOCIATES INC", line1 = "C/O HOGUE & ASSOCIATES INC", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", typecode = "0006", name = "HOGUE & ASSOCIATES INC", line1 = "COMMERCIAL FURNISHING DIV", line2 = "", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "94108", country = "US"},
                            new {id = "0000247673", typecode = "0001", name = "ELLIOTT & NELSON", line1 = "PO BOX 195", line2 = "", line3 = "", city = "AVERY", state = "CA", zip = "95224", country = "US"}
                        }
                    );
            }
        }
        
        /*
         * Dev Data
         */
        private static void InsertData(ISession session)
        {
            //Now insert new data
            var scott = new User("postit") { FirstName = "Scott", LastName = "Kirkland", Email = "srkirkland@ucdavis.edu", IsActive = true };
            var approverUser = new User("approver") { FirstName = "Approver", LastName = "Approver", Email = "srkirkland@ucdavis.edu", IsActive = true };
            var acctMgrUser = new User("acctmgr") { FirstName = "Account", LastName = "Manager", Email = "srkirkland@ucdavis.edu", IsActive = true };
            var alan = new User("anlai") { FirstName = "Alan", LastName = "Lai", Email = "anlai@ucdavis.edu", IsActive = true };
            var ken = new User("taylorkj") { FirstName = "Ken", LastName = "Taylor", Email = "taylorkj@ucdavis.edu", IsActive = true };
            var chris = new User("cthielen") { FirstName = "Christopher", LastName = "Thielen", Email = "cmthielen@ucdavis.edu", IsActive = true };
            var scottd = new User("sadyer") { FirstName = "Scott", LastName = "Dyer", Email = "sadyer@ucdavis.edu", IsActive = true };
            var jscub = new User("jscub")
                            {
                                FirstName = "James",
                                LastName = "Cubbage",
                                Email = "jscubbage@ucdavis.edu",
                                IsActive = true
                            };
            var jsylvest = new User("jsylvest")
            {
                FirstName = "Jason",
                LastName = "Sylvestre",
                Email = "jsylvest@ucdavis.edu",
                IsActive = true
            };

            var admin = session.Load<Role>("AD");
            var deptAdmin = session.Load<Role>("DA");
            var user = session.Load<Role>("RQ");
            var approver = session.Load<Role>("AR");
            var acctMgr = session.Load<Role>("AM");
            var purchaser = session.Load<Role>("PR");

            scott.Organizations.Add(session.Load<Organization>("AANS"));
            scott.Organizations.Add(session.Load<Organization>("AAES"));
            ken.Organizations.Add(session.Load<Organization>("AANS"));
            ken.Organizations.Add(session.Load<Organization>("ABAE"));
            scottd.Organizations.Add(session.Load<Organization>("AANS"));
            scottd.Organizations.Add(session.Load<Organization>("AAES"));
            scottd.Organizations.Add(session.Load<Organization>("AFST"));
            chris.Organizations.Add(session.Load<Organization>("AANS"));
            chris.Organizations.Add(session.Load<Organization>("AAES"));
            chris.Organizations.Add(session.Load<Organization>("AFST"));
            jscub.Organizations.Add(session.Load<Organization>("AFST"));
            jscub.Organizations.Add(session.Load<Organization>("AAES"));
            jsylvest.Organizations.Add(session.Load<Organization>("AFST"));
            jsylvest.Organizations.Add(session.Load<Organization>("AAES"));
            alan.Organizations.Add(session.Load<Organization>("AANS"));
            alan.Organizations.Add(session.Load<Organization>("AAES"));

            var testWorkgroup = new Workgroup() { Name = "Test Workgroup", IsActive = true, };
            var workGroupAccount = new WorkgroupAccount() { };
            workGroupAccount.Account = session.Load<Account>("3-6851000");
            testWorkgroup.AddAccount(workGroupAccount);
            var workgroupAccountWithUsers = new WorkgroupAccount
                                                {
                                                    Account = session.Load<Account>("3-APSO013"),
                                                    Approver = approverUser,
                                                    AccountManager = acctMgrUser
                                                };
            testWorkgroup.AddAccount(workgroupAccountWithUsers);

            testWorkgroup.PrimaryOrganization = session.Load<Organization>("AAES");
            testWorkgroup.Organizations.Add(session.Load<Organization>("AAES"));
            testWorkgroup.Organizations.Add(session.Load<Organization>("APLS"));

            // Add a vendor to the test workgroup
            var testWorkgroupVendor = new WorkgroupVendor
            {
                Name = "Office Max Corp.",
                City = "Sacramento",
                CountryCode = "US",
                Line1 = "3 1/2 External Drive",// (get it?)
                State = "CA",
                Zip = "95616"
            };
            
            var testWorkgroupVendor2 = new WorkgroupVendor
                                          {
                                              Name = "Manually Added Vendor Corp.",
                                              City = "Sacramento",
                                              CountryCode = "US",
                                              Line1 = "5 1/4 External Drive",// (get it?)
                                              State = "CA",
                                              Zip = "95816"
                                          };
            
            
            testWorkgroup.AddVendor(testWorkgroupVendor);
            testWorkgroup.AddVendor(testWorkgroupVendor2);

            var testWorkgroupAddress = new WorkgroupAddress()
                                           {
                                               Name = "The Office",
                                               Address = "One Shields Ave",
                                               Building = "Mrak",
                                               City = "Davis",
                                               State = "CA",
                                               Zip = "95616",
                                               IsActive = true
                                           };

            testWorkgroup.AddAddress(testWorkgroupAddress);

            var workgroupPerm = new WorkgroupPermission() { User = scott, Role = deptAdmin, Workgroup = testWorkgroup };
            var workgroupPerm2 = new WorkgroupPermission() { User = jsylvest, Role = deptAdmin, Workgroup = testWorkgroup };
            var workgroupPerm3 = new WorkgroupPermission() { User = jsylvest, Role = user, Workgroup = testWorkgroup };
            var workgroupPerm4 = new WorkgroupPermission() { User = jscub, Role = deptAdmin, Workgroup = testWorkgroup };
            var workgroupPerm5 = new WorkgroupPermission() { User = alan, Role = deptAdmin, Workgroup = testWorkgroup };
            var workgroupPerm6 = new WorkgroupPermission() { User = chris, Role = acctMgr, Workgroup = testWorkgroup };
            var workgroupPerm7 = new WorkgroupPermission() { User = scott, Role = approver, Workgroup = testWorkgroup };
            var workgroupPerm8 = new WorkgroupPermission() { User = alan, Role = approver, Workgroup = testWorkgroup };

            var shippingType = session.Load<ShippingType>("ST");
            var orderType = session.Load<Role>("OR");

            var autoApproval = new AutoApproval //Approve anything scott sends to alan under $1000 for the next 2 years
                                   {
                                       TargetUser = scott,
                                       User = alan,
                                       IsActive = true,
                                       Expiration = DateTime.Now.AddYears(2),
                                       LessThan = true,
                                       MaxAmount = 1000.00M
                                   };

            var autoApprovalAccount = new AutoApproval //Approve anything from account 3-APSO013 associated with approverUser under $1000 for 2 years
            {
                Account = session.Load<Account>("3-APSO013"),
                User = approverUser,
                IsActive = true,
                Expiration = DateTime.Now.AddYears(2),
                LessThan = true,
                MaxAmount = 1000.00M
            };

            session.Save(testWorkgroup);

            session.Save(scott);
            session.Save(alan);
            session.Save(ken);
            session.Save(jscub);
            session.Save(chris);
            session.Save(scottd);
            session.Save(jsylvest);

            session.Save(approverUser);
            session.Save(acctMgrUser);

            session.Save(workgroupPerm);
            session.Save(workgroupPerm2);
            session.Save(workgroupPerm3);
            session.Save(workgroupPerm4);
            session.Save(workgroupPerm5);
            session.Save(workgroupPerm6);
            session.Save(workgroupPerm7);
            session.Save(workgroupPerm8);

            session.Save(autoApproval);
            session.Save(autoApprovalAccount);
            
            Roles.AddUsersToRole(new[] { "postit", "anlai", "cthielen" }, "AD");
            Roles.AddUserToRole("anlai", "RQ");
            Roles.AddUserToRole("anlai", "DA");
            Roles.AddUserToRole("anlai", "AR");
            Roles.AddUserToRole("taylorkj", "DA");
            Roles.AddUserToRole("postit", "DA");
            Roles.AddUserToRole("jscub", "DA");
            Roles.AddUserToRole("jsylvest", "DA");

            session.Flush(); //Flush out the changes
        }

        /*
         * Demo Data
         */
        private static void InsertDemoData(ISession session)
        {
            // create the users
            var admin = new User("anlai") { FirstName = "Alan", LastName = "Lai", Email = "anlai@ucdavis.edu", IsActive = true };

            var user1 = new User("pjfry") { FirstName = "Philip", LastName = "Fry", Email = "pjfry@fake.com", IsActive = true };
            var user2 = new User("hsimpson") { FirstName = "Homer", LastName = "Simpson", Email="hsimpson@fake.com", IsActive = true};
            var user3 = new User("brannigan") { FirstName = "Zapp", LastName = "Brannigan", Email = "zbrannigan@fake.com", IsActive = true };
            var user4 = new User("awong") { FirstName = "Amy", LastName = "Wong", Email = "awong@fake.com", IsActive = true };
            var user5 = new User("zoidberg") { FirstName = "John", LastName = "Zoidberg", Email = "zoidberg@fake.com", IsActive = true };
            var user6 = new User("moe") { FirstName = "Moe", LastName = "Szyslak", Email = "moe@fake.com", IsActive = true };
            var user7 = new User("burns") { FirstName = "Monty", LastName = "Burns", Email = "burns@fake.com", IsActive = true };
            var user8 = new User("flanders") { FirstName = "Ned", LastName = "Flanders", Email = "flanders@fake.com", IsActive = true };
            var user9 = new User("grimes") { FirstName = "Frank", LastName = "Grimes", Email = "hsimpson@fake.com", IsActive = true };

            // setup the workgroup
            var org1 = session.Load<Organization>("AANS");
            var org2 = session.Load<Organization>("AAES");
            var org3 = session.Load<Organization>("ABAE");
            
            var orgset1 = new List<Organization>();
            orgset1.Add(org1);
            orgset1.Add(org2);
            var orgset2 = new List<Organization>();
            orgset2.Add(org2);
            orgset2.Add(org3);

            var workgroup1 = new Workgroup() {Name = "People that Work", IsActive = true, PrimaryOrganization = org1, Organizations = orgset1};
            var workgroup2 = new Workgroup() { Name = "Lazy People", IsActive = true, PrimaryOrganization = org3, Organizations = orgset2 };

            var acct1 = session.Load<Account>("3-APSAC37");
            var acct2 = session.Load<Account>("3-APSM170");
            var acct3 = session.Load<Account>("3-APSRSTR"); // has sub account
            var acct4 = session.Load<Account>("3-APSM152"); // has sub account
            var acct5 = session.Load<Account>("3-APSM326"); // has sub account
            
            workgroup1.AddAccount(new WorkgroupAccount() { Account = acct1 });
            workgroup1.AddAccount(new WorkgroupAccount() { Account = acct3 });
            workgroup1.AddAccount(new WorkgroupAccount() { Account = acct5 });

            workgroup2.AddAccount(new WorkgroupAccount() { Account = acct2 });
            workgroup2.AddAccount(new WorkgroupAccount() { Account = acct4 });

            var vendor1 = session.Load<Vendor>("0000247673");
            var vendoraddr1 = session.QueryOver<VendorAddress>().Where(a => a.Vendor == vendor1).Take(1).SingleOrDefault();
            var wv1 = new WorkgroupVendor() { VendorId = vendor1.Id, VendorAddressTypeCode = vendoraddr1.TypeCode, Name = vendor1.Name, Line1 = vendoraddr1.Line1, City = vendoraddr1.City, State = vendoraddr1.State, Zip = vendoraddr1.Zip, CountryCode = vendoraddr1.CountryCode };
            workgroup1.AddVendor(wv1);

            var vendor2 = session.Load<Vendor>("0000008573");
            var vendoraddr2 = session.QueryOver<VendorAddress>().Where(a => a.Vendor == vendor2).Take(1).SingleOrDefault();
            var wv2 = new WorkgroupVendor(){VendorId = vendor2.Id, VendorAddressTypeCode = vendoraddr2.TypeCode, Name = vendor2.Name, Line1 = vendoraddr2.Line1, City = vendoraddr2.City, State = vendoraddr2.State, Zip = vendoraddr2.Zip, CountryCode = vendoraddr2.CountryCode};
            workgroup2.AddVendor(wv2);

            var vendor3 = session.Load<Vendor>("0000006849");
            var vendoraddr3 = session.QueryOver<VendorAddress>().Where(a => a.Vendor == vendor2).Take(1).SingleOrDefault();
            var wv3 = new WorkgroupVendor(){VendorId = vendor3.Id, VendorAddressTypeCode = vendoraddr3.TypeCode, Name = vendor3.Name, Line1 = vendoraddr3.Line1, City = vendoraddr3.City, State = vendoraddr3.State, Zip = vendoraddr3.Zip, CountryCode = vendoraddr3.CountryCode};
            workgroup2.AddVendor(wv3);

            var wv4 = new WorkgroupVendor() { Name = "Legitimate Paper Mill", Line1 = "1 Fake Street.", City = "Davis", State = "CA", Zip = "95616", CountryCode = "US" };
            workgroup1.AddVendor(wv4);
            var wv5 = new WorkgroupVendor() { Name = "Office Supplies", Line1 = "2 Fake Street.", City = "Davis", State = "CA", Zip = "95616", CountryCode = "US" };
            workgroup1.AddVendor(wv5);
            var wv6 = new WorkgroupVendor() { Name = "Loads O Lab Equipment", Line1 = "5 Fake Street.", City = "Davis", State = "CA", Zip = "95616", CountryCode = "US" };
            workgroup2.AddVendor(wv6);

            var addr1 = new WorkgroupAddress() { Name = "128 Fake Hall", Address = "Fake Hall Road", City = "Davis", State = "CA", Zip = "95616" };
            workgroup1.AddAddress(addr1);
            var addr2 = new WorkgroupAddress() { Name = "10 Fake Hall", Address = "Fake Hall Road", City = "Davis", State = "CA", Zip = "95616"};
            workgroup1.AddAddress(addr2);
            var addr3 = new WorkgroupAddress() { Name = "526 Fake Hall", Address = "Fake Hall Road", City = "Davis", State = "CA", Zip = "95616" };
            workgroup2.AddAddress(addr3);

            //setup workgroup permissions
            var permission1 = new WorkgroupPermission() { User = user1, Workgroup = workgroup1, Role = session.Load<Role>("RQ") };
            var permission2 = new WorkgroupPermission() { User = user2, Workgroup = workgroup1, Role = session.Load<Role>("AP") };
            var permission3 = new WorkgroupPermission() { User = user3, Workgroup = workgroup1, Role = session.Load<Role>("AM") };
            var permission4 = new WorkgroupPermission() { User = user4, Workgroup = workgroup1, Role = session.Load<Role>("PR") };
            var permission5 = new WorkgroupPermission() { User = user5, Workgroup = workgroup1, Role = session.Load<Role>("AP") };  // conditional approver
            var permission6 = new WorkgroupPermission() { User = user6, Workgroup = workgroup2, Role = session.Load<Role>("RQ") };
            var permission7 = new WorkgroupPermission() { User = user7, Workgroup = workgroup2, Role = session.Load<Role>("AP") };
            var permission8 = new WorkgroupPermission() { User = user8, Workgroup = workgroup2, Role = session.Load<Role>("AM") };
            var permission9 = new WorkgroupPermission() { User = user9, Workgroup = workgroup2, Role = session.Load<Role>("PR") };

            // create some conditional approvals
            var ca1 = new ConditionalApproval() {Workgroup = workgroup1, PrimaryApprover = user5, Question = "Is this an IT purchaser?"};

            // standard order, no splits, no conditional approvals, at approval
            var order1 = new Order()
            {
                Justification = "I want to place this order because i need some stuff.",
                OrderType = session.Load<OrderType>("OR"),
                Vendor = wv1,
                Address = addr1,
                Workgroup = workgroup1,
                Organization = workgroup1.PrimaryOrganization,
                ShippingType = session.Load<ShippingType>("ST"),

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user1,
                StatusCode = session.Load<OrderStatusCode>("AP")
            };
            var line1 = new LineItem() { Quantity = 1, UnitPrice = 2.99m, Unit = "each", Description = "pencils" };
            var line2 = new LineItem() { Quantity = 3, UnitPrice = 17.99m, Unit = "dozen", Description = "pen", Url = "http://fake.com/product1", Notes = "I want the good pens." };
            order1.AddLineItem(line1);
            order1.AddLineItem(line2);
            order1.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("RQ"), Approved = true, User = user1});
            order1.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP") });
            order1.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AM") });
            order1.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("PR") });
            order1.AddTracking(new OrderTracking() { User = user1, DateCreated = DateTime.Now, Description = "Order submitted by " + user1.FullName, StatusCode = session.Load<OrderStatusCode>("RQ")});

            // order with conditional approval
            var order2 = new Order()
            {
                Justification = "I want to place this order because i need some stuff.",
                OrderType = session.Load<OrderType>("OR"),
                Vendor = wv1,
                Address = addr1,
                Workgroup = workgroup1,
                Organization = workgroup1.PrimaryOrganization,
                ShippingType = session.Load<ShippingType>("ST"),

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user1,
                StatusCode = session.Load<OrderStatusCode>("AP")
            };
            var line2_1 = new LineItem() { Quantity = 1, UnitPrice = 2978.99m, Unit = "each", Description = "Dell Laptop Computer" };
            order2.AddLineItem(line2_1);
            order2.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("RQ"), Approved = true, User = user1});
            order2.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP") });
            order2.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), User = user5 });    // conditional approval
            order2.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AM") });
            order2.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("PR") });
            order2.AddTracking(new OrderTracking() { User = user1, DateCreated = DateTime.Now, Description = "Order submitted by " + user1.FullName, StatusCode = session.Load<OrderStatusCode>("RQ") });

            // order with conditional, and one approved in AP
            var order3 = new Order()
            {
                Justification = "I want to place this order because i need some stuff.",
                OrderType = session.Load<OrderType>("OR"),
                Vendor = wv1,
                Address = addr1,
                Workgroup = workgroup1,
                Organization = workgroup1.PrimaryOrganization,
                ShippingType = session.Load<ShippingType>("ST"),

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user1,
                StatusCode = session.Load<OrderStatusCode>("AP")
            };
            var line3_1 = new LineItem() { Quantity = 1, UnitPrice = 1899.00m, Unit = "each", Description = "Dell Laptop Computer" };
            order3.AddLineItem(line3_1);
            order3.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("RQ"), Approved = true, User = user1 });
            order3.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), Approved = true });
            order3.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), User = user5 });    // conditional approval
            order3.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AM") });
            order3.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("PR") });
            order3.AddTracking(new OrderTracking() { User = user1, DateCreated = DateTime.Now, Description = "Order submitted by " + user1.FullName, StatusCode = session.Load<OrderStatusCode>("RQ") });
            order3.AddTracking(new OrderTracking() { User = user2, DateCreated = DateTime.Now, Description = "Order approved by " + user2.FullName, StatusCode = session.Load<OrderStatusCode>("AP") });

            // order with conditional, approved through AM
            var order4 = new Order()
            {
                Justification = "I want to place this order because i need some stuff.",
                OrderType = session.Load<OrderType>("OR"),
                Vendor = wv1,
                Address = addr1,
                Workgroup = workgroup1,
                Organization = workgroup1.PrimaryOrganization,
                ShippingType = session.Load<ShippingType>("ST"),

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user1,
                StatusCode = session.Load<OrderStatusCode>("AM")
            };
            var line4_1 = new LineItem() { Quantity = 1, UnitPrice = 1899.00m, Unit = "each", Description = "Dell Laptop Computer" };
            order4.AddLineItem(line4_1);
            order4.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("RQ"), Approved = true, User = user1 });
            order4.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), Approved = true });
            order4.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), Approved = true, User = user5 });    // conditional approval
            order4.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AM") });
            order4.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("PR") });
            order4.AddTracking(new OrderTracking() { User = user1, DateCreated = DateTime.Now, Description = "Order submitted by " + user1.FullName, StatusCode = session.Load<OrderStatusCode>("RQ") });
            order4.AddTracking(new OrderTracking() { User = user2, DateCreated = DateTime.Now, Description = "Order approved by " + user2.FullName, StatusCode = session.Load<OrderStatusCode>("AP") });
            order4.AddTracking(new OrderTracking() { User = user5, DateCreated = DateTime.Now, Description = "Order approved by " + user5.FullName, StatusCode = session.Load<OrderStatusCode>("AP") });

            // order with conditional up to account manager
            var order5 = new Order()
            {
                Justification = "I want to place this order because i need some stuff.",
                OrderType = session.Load<OrderType>("OR"),
                Vendor = wv1,
                Address = addr1,
                Workgroup = workgroup1,
                Organization = workgroup1.PrimaryOrganization,
                ShippingType = session.Load<ShippingType>("ST"),

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user1,
                StatusCode = session.Load<OrderStatusCode>("PR")
            };
            var line5_1 = new LineItem() { Quantity = 1, UnitPrice = 1899.00m, Unit = "each", Description = "Dell Laptop Computer" };
            order5.AddLineItem(line5_1);
            order5.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("RQ"), Approved = true, User = user1 });
            order5.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), Approved = true });
            order5.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), Approved = true, User = user5 });    // conditional approval
            order5.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AM"), Approved = true});
            order5.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("PR") });
            order5.AddTracking(new OrderTracking() { User = user1, DateCreated = DateTime.Now, Description = "Order submitted by " + user1.FullName, StatusCode = session.Load<OrderStatusCode>("RQ") });
            order5.AddTracking(new OrderTracking() { User = user2, DateCreated = DateTime.Now, Description = "Order approved by " + user2.FullName, StatusCode = session.Load<OrderStatusCode>("AP") });
            order5.AddTracking(new OrderTracking() { User = user5, DateCreated = DateTime.Now, Description = "Order approved by " + user5.FullName, StatusCode = session.Load<OrderStatusCode>("AP") });
            order5.AddTracking(new OrderTracking() { User = user4, DateCreated = DateTime.Now, Description = "Order approved by " + user4.FullName, StatusCode = session.Load<OrderStatusCode>("AM") });

            // no conditional approal, at account manager
            var order6 = new Order()
            {
                Justification = "I want to place this order because i need some stuff.",
                OrderType = session.Load<OrderType>("OR"),
                Vendor = wv1,
                Address = addr1,
                Workgroup = workgroup1,
                Organization = workgroup1.PrimaryOrganization,
                ShippingType = session.Load<ShippingType>("ST"),

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user1,
                StatusCode = session.Load<OrderStatusCode>("AM")
            };
            var line6_1 = new LineItem() { Quantity = 1, UnitPrice = 1899.00m, Unit = "each", Description = "Dell Laptop Computer" };
            order6.AddLineItem(line6_1);
            order6.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("RQ"), Approved = true, User = user1 });
            order6.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), Approved = true });
            order6.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AM"), User = user3 });
            order6.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("PR") });
            order6.AddTracking(new OrderTracking() { User = user1, DateCreated = DateTime.Now, Description = "Order submitted by " + user1.FullName, StatusCode = session.Load<OrderStatusCode>("RQ") });
            order6.AddTracking(new OrderTracking() { User = user2, DateCreated = DateTime.Now, Description = "Order approved by " + user2.FullName, StatusCode = session.Load<OrderStatusCode>("AP") });

            var order7 = new Order()
            {
                Justification = "I want to place this order because i need some stuff.",
                OrderType = session.Load<OrderType>("OR"),
                Vendor = wv1,
                Address = addr1,
                Workgroup = workgroup2,
                Organization = workgroup1.PrimaryOrganization,
                ShippingType = session.Load<ShippingType>("ST"),

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user6,
                StatusCode = session.Load<OrderStatusCode>("AM")
            };
            var line7_1 = new LineItem() { Quantity = 1, UnitPrice = 999.99m, Unit = "each", Description = "Dell Laptop Computer" };
            order7.AddLineItem(line7_1);
            order7.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("RQ"), Approved = true, User = user6 });
            order7.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AP"), User = user7 });
            order7.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("AM"), User = user8 });
            order7.AddApproval(new Approval() { StatusCode = session.Load<OrderStatusCode>("PR") });
            order7.AddTracking(new OrderTracking() { User = user6, DateCreated = DateTime.Now, Description = "Order submitted by " + user6.FullName, StatusCode = session.Load<OrderStatusCode>("RQ") });

            // save all the objects
            session.Save(admin);
            session.Save(user1);
            session.Save(user2);
            session.Save(user3);
            session.Save(user4);
            session.Save(user5);
            session.Save(user6);
            session.Save(user7);
            session.Save(user8);
            session.Save(user9);

            session.Save(workgroup1);
            session.Save(workgroup2);

            session.Save(ca1);

            session.Save(order1);
            session.Save(order2);
            session.Save(order3);
            session.Save(order4);
            session.Save(order5);
            session.Save(order6);
            session.Save(order7);
        }
    }
}
