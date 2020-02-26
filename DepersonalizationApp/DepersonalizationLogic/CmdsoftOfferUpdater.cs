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
    /// Обновление коммерческих предложений
    /// </summary>
    public class CmdsoftOfferUpdater : BaseUpdater<cmdsoft_offer>
    {
        public CmdsoftOfferUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] specificationIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select offer.cmdsoft_offerId, offer.mcdsoft_other_conditions");
            sb.AppendLine(" from dbo.cmdsoft_offer as offer");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("offer.mcdsoft_offer2", specificationIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override IEnumerable<cmdsoft_offer> ChangeByRules(IEnumerable<cmdsoft_offer> offers)
        {
            foreach (var offer in offers)
            {
                offer.mcdsoft_other_conditions = null;
                yield return offer;
            }
        }

        protected override cmdsoft_offer ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new cmdsoft_offer
            {
                Id = (Guid)sqlReader.GetValue(0),
                mcdsoft_other_conditions = sqlReader.GetValue(1) as string
            };
        }

        protected override Entity GetEntityForUpdate(cmdsoft_offer offer)
        {
            var entityForUpdate = new Entity(offer.LogicalName, offer.Id);
            entityForUpdate[_commonDepersonalizationNameField] = true;
            entityForUpdate["mcdsoft_other_conditions"] = offer.mcdsoft_other_conditions;
            return entityForUpdate;
        }
    }
}