﻿using System;
using System.Collections.Generic;
using NHibernate.Transform;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Purchasing.Core.Domain;
using NHibernate;

namespace Purchasing.Core.Repositories
{
    /// <summary>
    /// Repository for full text search queries
    /// </summary>
    public interface ISearchRepository
    {
        ISession Session { get; }
        IList<SearchResults.OrderResult> SearchOrders(string searchTerm);
        IList<SearchResults.LineResult> SearchLineItems(string searchTerm);
        IList<CustomFieldAnswer> SearchCustomFieldAnswers(string searchTerm);
        IList<OrderComment> SearchComments(string searchTerm);

        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        IList<Commodity> SearchCommodities(string searchTerm);
    }

    public class SearchResults
    {
        public class OrderResult
        {
            public int Id { get; set; }
            public DateTime DateCreated { get; set; }
            public string DeliverTo { get; set; }
            public string DeliverToEmail { get; set; }
            public string Justification { get; set; }
            public string CreatedBy { get; set; }
            public string RequestNumber { get; set; }
        }

        public class LineResult
        {
            public int OrderId { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public string RequestNumber { get; set; }
            public string CatalogNumber { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string Notes { get; set; }
            public string CommodityId { get; set; }
        }
    }

    public class DevelopmentSearchRepository : ISearchRepository
    {
        public ISession Session { get { return NHibernateSessionManager.Instance.GetSession(); } }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm)
        {
            var query = Session.CreateSQLQuery(
                @"SELECT TOP 1000 [Id],[DateCreated],[DeliverTo],[DeliverToEmail],[Justification],[CreatedBy],[RequestNumber] 
                    FROM [PrePurchasing].[dbo].[Orders]
                    WHERE Justification LIKE '%' + :searchTerm + '%'")
                .SetString("searchTerm", searchTerm)
                .SetResultTransformer(Transformers.AliasToBean<SearchResults.OrderResult>());

            return query.List<SearchResults.OrderResult>();
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm)
        {
            var query = Session.CreateSQLQuery(
                @"SELECT TOP 1000 o.Id as OrderId, o.RequestNumber,[Quantity],[Unit],[CatalogNumber],[Description],[Url],[Notes],[CommodityId]
                  FROM [PrePurchasing].[dbo].[LineItems] li INNER JOIN
                  [PrePurchasing].[dbo].[Orders] o on o.Id = li.OrderId
                  WHERE Description LIKE '%' + :searchTerm + '%'")
                .SetString("searchTerm", searchTerm)
                .SetResultTransformer(Transformers.AliasToBean<SearchResults.LineResult>());

            return query.List<SearchResults.LineResult>();
        }

        public IList<CustomFieldAnswer> SearchCustomFieldAnswers(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<OrderComment> SearchComments(string searchTerm)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        public IList<Commodity> SearchCommodities(string searchTerm)
        {
            //TODO: we can remove groupCode/SubGroupCode if they aren't needed.  Here we just leave them out of the query 
            //TODO: make into FTS once DB indexes are setup
            var query = Session.CreateSQLQuery(
                @"SELECT [Id],[Name]
                ,'' as [GroupCode],'' as [SubGroupCode]
                FROM [vCommodities]
                WHERE Id like '%' + :searchTerm + '%'
                    OR Name like '%' + :searchTerm + '%'")
                .AddEntity(typeof(Commodity))
                .SetString("searchTerm", searchTerm);

            return query.List<Commodity>();
        }
    }

    public class SearchRepository : ISearchRepository
    {
        public ISession Session { get { return NHibernateSessionManager.Instance.GetSession(); } }

        public IList<SearchResults.OrderResult> SearchOrders(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<SearchResults.LineResult> SearchLineItems(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<CustomFieldAnswer> SearchCustomFieldAnswers(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<OrderComment> SearchComments(string searchTerm)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches commodities via FTS
        /// </summary>
        public IList<Commodity> SearchCommodities(string searchTerm)
        {
            //TODO: we can remove groupCode/SubGroupCode if they aren't needed.  Here we just leave them out of the query 
            //TODO: make into FTS once DB indexes are setup
            var query = Session.CreateSQLQuery(
                @"SELECT [Id],[Name]
                ,'' as [GroupCode],'' as [SubGroupCode]
                FROM [vCommodities]
                WHERE Id like '%' + :searchTerm + '%'
                    OR Name like '%' + :searchTerm + '%'")
                .AddEntity(typeof(Commodity))
                .SetString("searchTerm", searchTerm);

            return query.List<Commodity>();
        }
    }
}
