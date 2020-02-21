using DepersonalizationApp.DepersonalizationLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdaterApp.DepersonalizationLogic
{
    public class OpportunityUpdater : BaseUpdater
    {
        /// <summary>
        /// Получаем последние 1000 записей проектов
        /// </summary>
        private static readonly QueryExpression _opportunityMainQuery = new QueryExpression
        {
            EntityName = "opportunity",
            ColumnSet = new ColumnSet("mcdsoft_discount", "cmdsoft_standartdiscount", "createdon", 
                "mcdsoft_standartdiscount_chiller", "cmdsoft_warranty", "cmdsoft_result", "mcdsoft_reason_for_the_loss"),
            /*Orders = 
            {
                new OrderExpression
                {
                    AttributeName = "createdon",
                    OrderType = OrderType.Descending
                }
            }*/
        };

        private static readonly int _opportunityMaxAmountOfRecords = 1000;

        public OpportunityUpdater(IOrganizationService organizationService)
            : base(organizationService, _opportunityMainQuery, _opportunityMaxAmountOfRecords)
        {
        }

        protected override void ChangeByRules(IEnumerable<Entity> records)
        {
            var random = new Random();
            var shuffleReasonsForTheLoss = new ShuffleFieldValues<OptionSetValue>("mcdsoft_reason_for_the_loss");

            foreach (var record in records)
            {
                try
                {
                    // А. Если значение поля «Ручной ввод скидки»(mcdsoft_discount) = «Да» [1], то 
                    // заполнить поля «Основная скидка СМ»(cmdsoft_standartdiscount), «% Основная скидка Чиллера»(mcdsoft_standartdiscount_chiller %), 
                    // «Гарантия, %»(cmdsoft_warranty) = Random(Тип - число в плавающей точкой, точность - 2, 0 - 100, 00)
                    if (record["mcdsoft_discount"] != null && (bool)record["mcdsoft_discount"])
                    {
                        float i = random.Next(0, 100);
                        var c = i + (float)random.NextDouble();
                        record["cmdsoft_standartdiscount"] = c;
                        record["mcdsoft_standartdiscount_chiller"] = c;
                        record["cmdsoft_warranty"] = c;
                    }

                    // B. В тех проектах, где значение поля «Результат»(cmdsoft_result) = «Проигран» [289 540 002], 
                    // копировать в отдельную таблицу значения полей «Причина проигрыша» (mcdsoft_reason_for_the_loss), 
                    // потом из этой таблицы случайным образом вставить(переписать) значения в другой проект(то есть перетасовать в проигранных проектах «Причины проигрыша»)
                    var cmdsoft_result = record["cmdsoft_result"] as OptionSetValue;
                    if (cmdsoft_result != null && cmdsoft_result.Value == 289540002)
                    {
                        var mcdsoft_reason_for_the_loss = record["mcdsoft_reason_for_the_loss"] as OptionSetValue;
                        shuffleReasonsForTheLoss.AddEntity(record);
                        shuffleReasonsForTheLoss.AddValue(mcdsoft_reason_for_the_loss);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record with Id = {record.Id} is not changed by logic A or B", ex);
                }
            }

            // B. Дополнение
            shuffleReasonsForTheLoss.Process();

            // C. Все что есть в примечаниях(Notes) и действиях(actions) связанных с проектами удалить (сообщения, эл. почта, прикрепленный файлы)
            var opportunityGuids = records.Select(e => e.Id);

            var activityDeleter = new ActivityDeleter(_organizationService, opportunityGuids);
            activityDeleter.Process();

            var annotationDeleter = new AnnotationDeleter(_organizationService, opportunityGuids);
            annotationDeleter.Process();
        }
    }
}