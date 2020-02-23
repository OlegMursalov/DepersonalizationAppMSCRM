﻿using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftPartOfOwnerUpdater : BaseUpdater<cmdsoft_part_of_owner>
    {
        public CmdsoftPartOfOwnerUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] opprotunityIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select partOwn.cmdsoft_part_of_ownerId, partOwn.cmdsoft_part");
            sb.AppendLine(" from dbo.cmdsoft_part_of_owner as partOwn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("partOwn.cmdsoft_ref_opportunity", opprotunityIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override cmdsoft_part_of_owner ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var cmdsoft_part_of_owner = new cmdsoft_part_of_owner
            {
                Id = (Guid)sqlReader.GetValue(0),
                cmdsoft_part = sqlReader.GetValue(1) as decimal?
            };
            return cmdsoft_part_of_owner;
        }

        protected override void ChangeByRules(IEnumerable<cmdsoft_part_of_owner> cmdsoftPartOfOwners)
        {
            var amountOfPartOwners = cmdsoftPartOfOwners.Count();
            if (amountOfPartOwners > 0)
            {
                int i = 0;
                var array = RandomRangeHelper.Get(amountOfPartOwners, 100);
                foreach (var partOfOwner in cmdsoftPartOfOwners)
                {
                    partOfOwner.cmdsoft_part = array[i];
                    i++;
                }
            }
        }
    }
}