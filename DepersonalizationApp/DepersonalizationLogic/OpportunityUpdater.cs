using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace UpdaterApp.DepersonalizationLogic
{
    /// <summary>
    /// Обновление проектов
    /// </summary>
    public class OpportunityUpdater : BaseUpdater<Opportunity>
    {
        public OpportunityUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select opp.OpportunityId, opp.mcdsoft_discount, opp.cmdsoft_standartdiscount, opp.mcdsoft_standartdiscount_chiller,");
            sb.AppendLine($" opp.cmdsoft_warranty, opp.cmdsoft_Result, opp.mcdsoft_reason_for_the_loss, opp.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from Opportunity as opp");
            sb.AppendLine(" where opp.OpportunityId in (select oppIn.OpportunityId");
            sb.AppendLine("  from dbo.Opportunity as oppIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("oppIn.OpportunityId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("oppIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        protected override Opportunity ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var opportunity = new Opportunity
            {
                Id = (Guid)sqlReader.GetValue(0),
                mcdsoft_discount = sqlReader.GetValue(1) as bool?,
                cmdsoft_standartdiscount = sqlReader.GetValue(2) as decimal?,
                mcdsoft_standartdiscount_chiller = sqlReader.GetValue(3) as decimal?,
                cmdsoft_warranty = sqlReader.GetValue(4) as decimal?,
                mcdsoft_reason_for_the_loss = sqlReader.GetValue(6) as string,
                yolva_is_depersonalized = sqlReader.GetValue(7) as bool?
            };
            var cmdsoft_ResultVal = sqlReader.GetValue(5) as int?;
            if (cmdsoft_ResultVal != null)
            {
                opportunity.cmdsoft_Result = new OptionSetValue(cmdsoft_ResultVal.Value);
            }
            return opportunity;
        }

        protected override IEnumerable<Opportunity> ChangeByRules(IEnumerable<Opportunity> opportunities)
        {
            var randomHelper = new RandomHelper();
            var shuffleFieldValues = new ShuffleFieldValuesHelper<Opportunity>();

            foreach (var opportunity in opportunities)
            {
                // А. Если значение поля «Ручной ввод скидки»(mcdsoft_discount) = «Да» [1], то 
                // заполнить поля «Основная скидка СМ»(cmdsoft_standartdiscount), «% Основная скидка Чиллера»(mcdsoft_standartdiscount_chiller %), 
                // «Гарантия, %»(cmdsoft_warranty) = Random(Тип - число в плавающей точкой, точность - 2, 0 - 100, 00)
                if (opportunity.mcdsoft_discount != null && (bool)opportunity.mcdsoft_discount)
                {
                    opportunity.cmdsoft_standartdiscount = randomHelper.GetDecimal(0, 100);
                    opportunity.mcdsoft_standartdiscount_chiller = randomHelper.GetDecimal(0, 100);
                    opportunity.cmdsoft_warranty = randomHelper.GetDecimal(0, 100);
                }

                // B. В тех проектах, где значение поля «Результат»(cmdsoft_result) = «Проигран» [289 540 002], 
                // копировать в отдельную таблицу значения полей «Причина проигрыша» (mcdsoft_reason_for_the_loss), 
                // потом из этой таблицы случайным образом вставить(переписать) значения в другой проект(то есть перетасовать в проигранных проектах «Причины проигрыша»)
                if (opportunity.cmdsoft_Result != null && opportunity.cmdsoft_Result.Value == 289540002)
                {
                    shuffleFieldValues.AddEntity(opportunity);
                    shuffleFieldValues.AddValue("mcdsoft_reason_for_the_loss", opportunity.mcdsoft_reason_for_the_loss);
                }
            }

            // B. Дополнение (см. выше)
            shuffleFieldValues.Process();

            return opportunities;
        }

        protected override Entity GetEntityForUpdate(Opportunity opportunity)
        {
            var entityForUpdate = new Entity(opportunity.LogicalName, opportunity.Id);
            entityForUpdate["cmdsoft_standartdiscount"] = opportunity.cmdsoft_standartdiscount;
            entityForUpdate["mcdsoft_standartdiscount_chiller"] = opportunity.mcdsoft_standartdiscount_chiller;
            entityForUpdate["cmdsoft_warranty"] = opportunity.cmdsoft_warranty;
            entityForUpdate["mcdsoft_reason_for_the_loss"] = opportunity.mcdsoft_reason_for_the_loss;
            return entityForUpdate;
        }
    }
}