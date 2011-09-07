﻿using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Order : DomainObject
    {
        public Order()
        {
            LineItems = new List<LineItem>();
            Approvals = new List<Approval>();
        }

        public virtual OrderType OrderType { get; set; }
        public virtual int VendorId { get; set; }//TODO: Replace with actual vendor
        public virtual int AddressId { get; set; }//TODO: Replace
        public virtual ShippingType ShippingType { get; set; }
        public virtual DateTime DateNeeded { get; set; }
        public virtual bool AllowBackorder { get; set; }
        public virtual decimal EstimatedTax { get; set; }
        public virtual Workgroup Workgroup { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual string PoNumber { get; set; }
        public virtual Approval LastCompletedApproval { get; set; }
        public virtual decimal ShippingAmount { get; set; }
        public virtual OrderStatusCode StatusCode { get; set; }

        public virtual IList<LineItem> LineItems { get; set; }
        public virtual IList<Approval> Approvals { get; set; }

        public virtual void AddLineItem(LineItem lineItem)
        {
            lineItem.Order = this;
            LineItems.Add(lineItem);
        }

        public virtual void AddApproval(Approval approval)
        {
            approval.Order = this;
            Approvals.Add(approval);
        }
    }

    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id);

            Map(x => x.VendorId);
            Map(x => x.AddressId); //TODO: Replace these with actual lookups

            Map(x => x.DateNeeded);
            Map(x => x.AllowBackorder);
            Map(x => x.EstimatedTax);
            Map(x => x.PoNumber);
            Map(x => x.ShippingAmount);

            References(x => x.OrderType);
            References(x => x.ShippingType);
            References(x => x.Workgroup);
            References(x => x.Organization);
            References(x => x.LastCompletedApproval).Column("LastCompletedApprovalId");
            References(x => x.StatusCode);

            HasMany(x => x.LineItems).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Approvals).ExtraLazyLoad().Cascade.AllDeleteOrphan().Inverse(); //TODO: check out this mapping when used with splits
        }
    }
}