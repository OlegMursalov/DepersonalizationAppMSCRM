using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftSpecificationLink
    {
        public Guid Id { get; set; }

        // Прайс-лист NAV (yolva_salesprice)
        public Guid? YolvaSalesPrice { get; set; }
    }

    public class CmdsoftSpecificationRetriever : Base<CmdsoftSpecificationLink>
    {
        public CmdsoftSpecificationRetriever(SqlConnection sqlConnection, IEnumerable<Guid> opprotunityIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select sp.cmdsoft_specificationId, sp.yolva_salespricenav");
            sb.AppendLine(" from dbo.cmdsoft_specification as sp");
            sb.AppendLine(" where sp.cmdsoft_specificationId in (select spIn.cmdsoft_specificationId");
            sb.AppendLine("  from dbo.cmdsoft_specification as spIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("spIn.cmdsoft_spprojectnumber", opprotunityIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("spIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery= sb.ToString();
        }

        public IEnumerable<CmdsoftSpecificationLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override CmdsoftSpecificationLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var specificationLink = new CmdsoftSpecificationLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                YolvaSalesPrice = sqlReader.GetValue(1) as Guid?
            };
            return specificationLink;
        }
    }
}