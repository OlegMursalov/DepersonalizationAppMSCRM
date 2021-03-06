﻿using CRMEntities;
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
        public CmdsoftOfferUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select offer.cmdsoft_offerId, offer.mcdsoft_other_conditions, offer.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.cmdsoft_offer as offer");
            sb.AppendLine(" where offer.cmdsoft_offerId in (select offerIn.cmdsoft_offerId");
            sb.AppendLine("  from dbo.cmdsoft_offer as offerIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("offerIn.cmdsoft_offerId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("offerIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
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
                mcdsoft_other_conditions = sqlReader.GetValue(1) as string,
                yolva_is_depersonalized = sqlReader.GetValue(2) as bool?
            };
        }

        protected override Entity GetEntityForUpdate(cmdsoft_offer offer)
        {
            var entityForUpdate = new Entity(offer.LogicalName, offer.Id);
            entityForUpdate["mcdsoft_other_conditions"] = offer.mcdsoft_other_conditions;
            return entityForUpdate;
        }
    }
}