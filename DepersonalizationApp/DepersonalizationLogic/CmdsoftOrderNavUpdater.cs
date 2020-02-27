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
    /// Обновление продаж
    /// </summary>
    public class CmdsoftOrderNavUpdater : BaseUpdater<cmdsoft_ordernav>
    {
        private static int _globalCounterBySessionApp = 1;

        public CmdsoftOrderNavUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select ordNav.cmdsoft_ordernavId, ordNav.cmdsoft_namecustomotgr, ordNav.cmdsoft_namecustomsales, ordNav.cmdsoft_namecustomorder,");
            sb.AppendLine($" ordNav.cmdsoft_totalamount, ordNav.cmdsoft_totamontvat, ordNav.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.cmdsoft_ordernav as ordNav");
            sb.AppendLine(" where ordNav.cmdsoft_ordernavId in (select ordNavIn.cmdsoft_ordernavId");
            sb.AppendLine("  from dbo.cmdsoft_ordernav as ordNavIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ordNavIn.cmdsoft_ordernavId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("ordNavIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        protected override cmdsoft_ordernav ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var cmdsoft_ordernav = new cmdsoft_ordernav
            {
                Id = (Guid)sqlReader.GetValue(0),
                cmdsoft_namecustomotgr = sqlReader.GetValue(1) as string,
                cmdsoft_namecustomsales = sqlReader.GetValue(2) as string,
                cmdsoft_namecustomorder = sqlReader.GetValue(3) as string,
                cmdsoft_totalamount = sqlReader.GetValue(4) as decimal?,
                cmdsoft_totamontvat = sqlReader.GetValue(5) as decimal?,
                yolva_is_depersonalized = sqlReader.GetValue(6) as bool?
            };
            return cmdsoft_ordernav;
        }

        protected override IEnumerable<cmdsoft_ordernav> ChangeByRules(IEnumerable<cmdsoft_ordernav> cmdsoftOrdernavs)
        {
            var random = new Random();
            foreach (var orderNav in cmdsoftOrdernavs)
            {
                var saleRandN = random.Next(10000, 20000);
                orderNav.cmdsoft_namecustomotgr = $"КлиентОтг №{_globalCounterBySessionApp}";
                orderNav.cmdsoft_namecustomsales = $"КлиентПрод №{_globalCounterBySessionApp}";
                orderNav.cmdsoft_namecustomorder = $"КлиентВыстСчет №{_globalCounterBySessionApp}";
                orderNav.cmdsoft_totalamount = orderNav.cmdsoft_totalamount + saleRandN;
                orderNav.cmdsoft_totamontvat = orderNav.cmdsoft_totamontvat + saleRandN;
                _globalCounterBySessionApp++;
                yield return orderNav;
            }
        }

        protected override Entity GetEntityForUpdate(cmdsoft_ordernav cmdsoftOrdernav)
        {
            var entityForUpdate = new Entity(cmdsoftOrdernav.LogicalName, cmdsoftOrdernav.Id);
            entityForUpdate["cmdsoft_namecustomotgr"] = cmdsoftOrdernav.cmdsoft_namecustomotgr;
            entityForUpdate["cmdsoft_namecustomsales"] = cmdsoftOrdernav.cmdsoft_namecustomsales;
            entityForUpdate["cmdsoft_namecustomorder"] = cmdsoftOrdernav.cmdsoft_namecustomorder;
            entityForUpdate["cmdsoft_totalamount"] = cmdsoftOrdernav.cmdsoft_totalamount;
            entityForUpdate["cmdsoft_totamontvat"] = cmdsoftOrdernav.cmdsoft_totamontvat;
            return entityForUpdate;
        }
    }
}