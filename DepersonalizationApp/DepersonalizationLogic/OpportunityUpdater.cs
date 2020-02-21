using CRMEntities;
using DepersonalizationApp.DepersonalizationLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdaterApp.DepersonalizationLogic
{
    public class OpportunityUpdater : BaseUpdater<Opportunity>
    {
        public OpportunityUpdater(OrganizationServiceCtx serviceContext) : base(serviceContext)
        {
        }

        protected override void ChangeByRules(IEnumerable<Opportunity> opportunities)
        {
            var random = new Random();
            var shuffleReasonsForTheLoss = new ShuffleFieldValues<string>("mcdsoft_reason_for_the_loss");

            foreach (var opportunity in opportunities)
            {
                try
                {
                    // А. Если значение поля «Ручной ввод скидки»(mcdsoft_discount) = «Да» [1], то 
                    // заполнить поля «Основная скидка СМ»(cmdsoft_standartdiscount), «% Основная скидка Чиллера»(mcdsoft_standartdiscount_chiller %), 
                    // «Гарантия, %»(cmdsoft_warranty) = Random(Тип - число в плавающей точкой, точность - 2, 0 - 100, 00)
                    if (opportunity.mcdsoft_discount != null && (bool)opportunity.mcdsoft_discount)
                    {
                        decimal i = random.Next(0, 100);
                        decimal c = i + (decimal)random.NextDouble();
                        opportunity.cmdsoft_standartdiscount = c;
                        opportunity.mcdsoft_standartdiscount_chiller = c;
                        opportunity.cmdsoft_warranty = c;
                    }

                    // B. В тех проектах, где значение поля «Результат»(cmdsoft_result) = «Проигран» [289 540 002], 
                    // копировать в отдельную таблицу значения полей «Причина проигрыша» (mcdsoft_reason_for_the_loss), 
                    // потом из этой таблицы случайным образом вставить(переписать) значения в другой проект(то есть перетасовать в проигранных проектах «Причины проигрыша»)
                    if (opportunity.cmdsoft_Result != null && opportunity.cmdsoft_Result.Value == 289540002)
                    {
                        shuffleReasonsForTheLoss.AddEntity(opportunity);
                        shuffleReasonsForTheLoss.AddValue(opportunity.mcdsoft_reason_for_the_loss);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record with Id = {opportunity.Id} is not changed by logic A or B", ex);
                }
            }

            // B. Дополнение
            shuffleReasonsForTheLoss.Process();

            // C. Все что есть в примечаниях(Notes) и действиях(actions) связанных с проектами удалить (сообщения, эл. почта, прикрепленный файлы)
            var opportunityGuids = opportunities.Select(e => e.Id);

            var activityDeleter = new ActivityDeleter(_serviceContext);
            activityDeleter.Process();

            var annotationDeleter = new AnnotationDeleter(_serviceContext);
            annotationDeleter.Process();
        }
    }
}