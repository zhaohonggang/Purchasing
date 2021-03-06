using System;

namespace Purchasing.Core.Queries
{
    public class SearchResults
    {
        public class OrderResult
        {
            public static readonly string[] SearchableFields = { "requestnumber", "justification", "businesspurpose","shipto", "shiptoemail", "ponumber", "tag","referencenumber", "workgroupid" };

            public int Id { get; set; }
            public DateTime DateCreated { get; set; }
            public string DeliverTo { get; set; }
            public string DeliverToEmail { get; set; }
            public string Justification { get; set; }
            public string BusinessPurpose { get; set; } //New
            public string CreatedBy { get; set; }
            public string RequestNumber { get; set; }
            public string PoNumber { get; set; }
            public string Tag { get; set; } //New
            public string ReferenceNumber { get; set; }
            public string Approver { get; set; }
            public string AccountManager { get; set; }
            public string Purchaser { get; set; }
        }

        public class LineResult
        {
            public static readonly string[] SearchableFields = {
                                                                   "description", "url", "notes", "catalognumber",
                                                                   "commodityid", "receivednotes", "paidnotes"
                                                               };

            public int Id { get; set; }
            public int OrderId { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public string RequestNumber { get; set; }
            public string CatalogNumber { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string Notes { get; set; }
            public string CommodityId { get; set; }
            public string ReceivedNotes { get; set; }
            public string PaidNotes { get; set; }
        }

        public class CommentResult
        {
            public static readonly string[] SearchableFields = {"text"};

            public int Id { get; set; }
            public int OrderId { get; set; }
            public string RequestNumber { get; set; }
            public DateTime DateCreated { get; set; }
            public string Text { get; set; }
            public string CreatedBy { get; set; }
        }

        public class CustomFieldResult
        {
            public static readonly string[] SearchableFields = {"answer"};

            public int Id { get; set; }
            public int OrderId { get; set; }
            public string RequestNumber { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
        }
    }
}