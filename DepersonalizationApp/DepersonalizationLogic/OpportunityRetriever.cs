﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class OpportunityLink
    {
        public Guid Id { get; set; }

        // Организация (account)
        public Guid? CustomerId { get; set; }
        public Guid? CmdsoftProjectAgency { get; set; }
        public Guid? McdsoftRefAccount { get; set; }
        public Guid? CmdsoftGeneralContractor { get; set; }

        // Контакт (contact)
        public Guid? CmdsoftManagerProject { get; set; }
        public Guid? CmdsoftDealer { get; set; }
        public Guid? CmdsoftContactProjectAgency { get; set; }
        public Guid? McdsoftRefContact { get; set; }
    }

    public class OpportunityRetriever : Base<OpportunityLink>
    {
        public OpportunityRetriever(SqlConnection sqlConnection) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select opp.OpportunityId, opp.customerid, opp.cmdsoft_project_agency, opp.mcdsoft_ref_account, opp.cmdsoft_generalcontractor,");
            sb.AppendLine(" opp.cmdsoft_Managerproject, opp.cmdsoft_Dealer, opp.cmdsoft_contact_project_agency, opp.mcdsoft_ref_contact");
            sb.AppendLine(" from Opportunity as opp");
            sb.AppendLine(" where opp.OpportunityId in (select oppIn.OpportunityId");
            sb.AppendLine("  from dbo.Opportunity as oppIn");
            sb.AppendLine("  order by oppIn.CreatedOn desc");
            sb.AppendLine("  offset 0 rows");
            sb.AppendLine("  fetch next 500 rows only)");
            _retrieveSqlQuery = sb.ToString();
        }

        public IEnumerable<OpportunityLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override OpportunityLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var opportunityLink = new OpportunityLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                CustomerId = sqlReader.GetValue(1) as Guid?,
                CmdsoftProjectAgency = sqlReader.GetValue(2) as Guid?,
                McdsoftRefAccount = sqlReader.GetValue(3) as Guid?,
                CmdsoftGeneralContractor = sqlReader.GetValue(4) as Guid?,
                CmdsoftManagerProject = sqlReader.GetValue(5) as Guid?,
                CmdsoftDealer = sqlReader.GetValue(6) as Guid?,
                CmdsoftContactProjectAgency = sqlReader.GetValue(7) as Guid?,
                McdsoftRefContact = sqlReader.GetValue(8) as Guid?
            };
            return opportunityLink;
        }
    }
}