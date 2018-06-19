﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.PowerBI.Common.Api.Datasets
{
    public class Relationship
    {
        public string Name { get; set; }
        public CrossFilteringBehaviorEnum CrossFilteringBehavior { get; set; }
        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string ToTable { get; set; }
        public string ToColumn { get; set; }

        public static implicit operator Relationship(PowerBI.Api.V2.Models.Relationship relationship)
        {
            return new Relationship
            {
                Name = relationship.Name,
                CrossFilteringBehavior = ConvertCrossFilteringBehavior(relationship.CrossFilteringBehavior),
                FromTable = relationship.FromTable,
                FromColumn = relationship.FromColumn,
                ToTable = relationship.ToTable,
                ToColumn = relationship.ToColumn
            };
        }

        private static CrossFilteringBehaviorEnum ConvertCrossFilteringBehavior(PowerBI.Api.V2.Models.CrossFilteringBehaviorEnum? crossFilteringBehavior)
        {
            if(crossFilteringBehavior == null)
            {
                return CrossFilteringBehaviorEnum.NotAvailable;
            }

            return (CrossFilteringBehaviorEnum)Enum.Parse(typeof(CrossFilteringBehaviorEnum), crossFilteringBehavior.Value.ToString());
        }
    }

    public enum CrossFilteringBehaviorEnum
    {
        OneDirection,
        BothDirections,
        Automatic,
        NotAvailable
    }
}