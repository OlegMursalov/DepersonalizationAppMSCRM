using CRMEntities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftOrderlineNavUpdater : BaseUpdater<cmdsoft_orderlinenav>
    {
        public CmdsoftOrderlineNavUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] opprotunityIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select orLnNav.cmdsoft_orderlinenavId, orLnNav.mcdsoft_price_discount_with_VAT, orLnNav.mcdsoft_price_discount_without_VAT,");
            sb.AppendLine(" orLnNav.mcdsoft_price_without_vat, orLnNav.cmdsoft_amountsalesvat, orLnNav.cmdsoft_amountsale");
            sb.AppendLine(" from dbo.cmdsoft_orderlinenav as orLnNav");
            sb.AppendLine(" where orLnNav.cmdsoft_ref_opportunity in (");
            for (int i = 0; i < opprotunityIds.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append($"'{opprotunityIds[i]}'");
                }
                else
                {
                    sb.Append($", '{opprotunityIds[i]}'");
                }
            }
            sb.Append(")");
            _retrieveSqlQuery = sb.ToString();
        }

        protected override cmdsoft_orderlinenav ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var cmdsoft_orderlinenavId = (Guid)sqlReader.GetValue(0);
            var mcdsoft_price_discount_with_VAT = sqlReader.GetValue(1) as decimal?;
            var mcdsoft_price_discount_without_VAT = sqlReader.GetValue(2) as decimal?;
            var mcdsoft_price_without_vat = sqlReader.GetValue(3) as decimal?;
            var cmdsoft_amountsalesvat = sqlReader.GetValue(4) as decimal?;
            var cmdsoft_amountsale = sqlReader.GetValue(5) as decimal?;
            var cmdsoft_orderlinenav = new cmdsoft_orderlinenav
            {
                Id = cmdsoft_orderlinenavId,
                mcdsoft_price_discount_with_VAT = mcdsoft_price_discount_with_VAT,
                mcdsoft_price_discount_without_VAT = mcdsoft_price_discount_without_VAT,
                mcdsoft_price_without_vat = mcdsoft_price_without_vat,
                cmdsoft_amountsalesvat = cmdsoft_amountsalesvat,
                cmdsoft_amountsale = cmdsoft_amountsale
            };
            return cmdsoft_orderlinenav;
        }

        protected override void ChangeByRules(IEnumerable<cmdsoft_orderlinenav> cmdsoftOrderineNavs)
        {
            var randN = new Random().Next(1, 10);
            foreach (var orderineNav in cmdsoftOrderineNavs)
            {
                if (orderineNav.mcdsoft_price_discount_with_VAT != null)
                {
                    orderineNav.mcdsoft_price_discount_with_VAT /= randN;
                }
                if (orderineNav.mcdsoft_price_discount_without_VAT != null)
                {
                    orderineNav.mcdsoft_price_discount_without_VAT /= randN;
                }
                if (orderineNav.mcdsoft_price_without_vat != null)
                {
                    orderineNav.mcdsoft_price_without_vat /= randN;
                }
                if (orderineNav.cmdsoft_amountsalesvat != null)
                {
                    orderineNav.cmdsoft_amountsalesvat /= randN;
                }
                if (orderineNav.cmdsoft_amountsale != null)
                {
                    orderineNav.cmdsoft_amountsale /= randN;
                }
            }
        }
    }
}