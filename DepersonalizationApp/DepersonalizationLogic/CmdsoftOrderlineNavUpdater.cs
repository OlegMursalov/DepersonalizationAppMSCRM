using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Обновление составов продаж
    /// </summary>
    public class CmdsoftOrderlineNavUpdater : BaseUpdater<cmdsoft_orderlinenav>
    {
        public CmdsoftOrderlineNavUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select orLnNav.cmdsoft_orderlinenavId, orLnNav.mcdsoft_price_discount_with_VAT, orLnNav.mcdsoft_price_discount_without_VAT,");
            sb.AppendLine($" orLnNav.mcdsoft_price_without_vat, orLnNav.cmdsoft_amountsalesvat, orLnNav.cmdsoft_amountsale, orLnNav.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.cmdsoft_orderlinenav as orLnNav");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("orLnNav.cmdsoft_orderlinenavId", ids);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override cmdsoft_orderlinenav ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var cmdsoft_orderlinenav = new cmdsoft_orderlinenav
            {
                Id = (Guid)sqlReader.GetValue(0),
                mcdsoft_price_discount_with_VAT = sqlReader.GetValue(1) as decimal?,
                mcdsoft_price_discount_without_VAT = sqlReader.GetValue(2) as decimal?,
                mcdsoft_price_without_vat = sqlReader.GetValue(3) as decimal?,
                cmdsoft_amountsalesvat = sqlReader.GetValue(4) as decimal?,
                cmdsoft_amountsale = sqlReader.GetValue(5) as decimal?,
                yolva_is_depersonalized = sqlReader.GetValue(6) as bool?
            };
            return cmdsoft_orderlinenav;
        }

        protected override IEnumerable<cmdsoft_orderlinenav> ChangeByRules(IEnumerable<cmdsoft_orderlinenav> cmdsoftOrderineNavs)
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
                yield return orderineNav;
            }
        }

        protected override Entity GetEntityForUpdate(cmdsoft_orderlinenav cmdsoftOrderineNav)
        {
            var entityForUpdate = new Entity(cmdsoftOrderineNav.LogicalName, cmdsoftOrderineNav.Id);
            entityForUpdate["mcdsoft_price_discount_with_VAT"] = cmdsoftOrderineNav.mcdsoft_price_discount_with_VAT;
            entityForUpdate["mcdsoft_price_discount_without_VAT"] = cmdsoftOrderineNav.mcdsoft_price_discount_without_VAT;
            entityForUpdate["mcdsoft_price_without_vat"] = cmdsoftOrderineNav.mcdsoft_price_without_vat;
            entityForUpdate["cmdsoft_amountsalesvat"] = cmdsoftOrderineNav.cmdsoft_amountsalesvat;
            entityForUpdate["cmdsoft_amountsale"] = cmdsoftOrderineNav.cmdsoft_amountsale;
            return entityForUpdate;
        }
    }
}