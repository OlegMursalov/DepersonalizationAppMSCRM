using CRMEntities;
using DepersonalizationApp.DepersonalizationLogic;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace UpdaterApp.DepersonalizationLogic
{
    public class OpportunityUpdater : BaseUpdater<Opportunity>
    {
        public OpportunityUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
            _retrieveSqlQuery =
                "top 1000" +
                " select opp.OpportunityId, opp.mcdsoft_discount, opp.cmdsoft_standartdiscount, opp.mcdsoft_standartdiscount_chiller," +
                " opp.cmdsoft_warranty, opp.cmdsoft_Result, opp.mcdsoft_reason_for_the_loss" +
                " from dbo.Opportunity as opp" +
                " order by opp.CreatedOn desc";
        }

        protected override Opportunity ConvertSqlDataReaderToEntity(SqlDataReader sqlReader)
        {
            var opportunityId = (Guid)sqlReader.GetValue(0);
            var mcdsoft_discount = sqlReader.GetValue(1) as bool?;
            var cmdsoft_standartdiscount = sqlReader.GetValue(2) as decimal?;
            var mcdsoft_standartdiscount_chiller = sqlReader.GetValue(3) as decimal?;
            var cmdsoft_warranty = sqlReader.GetValue(4) as decimal?;
            var cmdsoft_Result = sqlReader.GetValue(5) as OptionSetValue;
            var mcdsoft_reason_for_the_loss = sqlReader.GetValue(6) as string;
            var opportunity = new Opportunity
            {
                Id = opportunityId,
                mcdsoft_discount = mcdsoft_discount,
                cmdsoft_standartdiscount = cmdsoft_standartdiscount,
                mcdsoft_standartdiscount_chiller = mcdsoft_standartdiscount_chiller,
                cmdsoft_warranty = cmdsoft_warranty,
                cmdsoft_Result = cmdsoft_Result,
                mcdsoft_reason_for_the_loss = mcdsoft_reason_for_the_loss
            };
            return opportunity;
        }

        protected override void ChangeByRules(IEnumerable<Opportunity> opportunities)
        {
            var random = new Random();
            var shuffleReasonsForTheLoss = new ShuffleFieldValues<string>("mcdsoft_reason_for_the_loss");

            foreach (var opportunity in opportunities)
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

            // B. Дополнение (см. выше)
            shuffleReasonsForTheLoss.Process();

            // C. Все что есть в примечаниях (Notes) и действиях (actions), связанных с проектами, удалить (сообщения, эл. почта, прикрепленный файлы)
            var opportunityGuids = opportunities.Select(e => e.Id).Distinct();

            var taskActivityDeleter = new System.Threading.Tasks.Task(() =>
            {
                var activityDeleter = new RelatedActivityDeleter(_serviceContext, opportunityGuids);
                activityDeleter.Process();
            });
            taskActivityDeleter.Start();

            var taskAnnotationDeleter = new System.Threading.Tasks.Task(() =>
            {
                var annotationDeleter = new RelatedAnnotationDeleter(_serviceContext, opportunityGuids);
                annotationDeleter.Process();
            });
            taskAnnotationDeleter.Start();

            // D. 2. Меняем связанные с изменяемыми проектами записи сущности «Доля ответственного» (cmdsoft_part_of_owner), 
            // связь по полю cmdsoft_part_of_owner.cmdsoft_ref_opportunity:
            // В изменяемых записях меняем значения поля «Доля %»(cmdsoft_part) = Random(Тип - integer, 0 - 100), 
            // таким образом, чтобы по каждому проекту сумма Полей «Доля» СУММА(cmdsoft_part_of_owner.cmdsoft_part по каждому проекту) = 100.
            var taskCmdsoftPartOfOwnerUpdater = new System.Threading.Tasks.Task(() =>
            {
                var partOfOwnerUpdater = new CmdsoftPartOfOwnerUpdater(_serviceContext, opportunityGuids);
                partOfOwnerUpdater.Process();
            });
            taskCmdsoftPartOfOwnerUpdater.Start();

            // D. 3. Меняем связанные с изменяемыми проектами записи сущности Составы продаж (cmdsoft_orderlinenav), меняем поля:
            // С каждой записью «Составы продаж», взять Var_Rand_n = Random(Тип – Целое число, 0 - 9) и поделить все 
            // изменяемые поля на это число(важно, чтобы случайное число у каждой отдельной записи «Состава продаж» было одно)...
            var taskCmdsoftOrderlineNavUpdater = new System.Threading.Tasks.Task(() =>
            {
                var orderlineNavUpdater = new CmdsoftOrderlineNavUpdater(_serviceContext, opportunityGuids);
                orderlineNavUpdater.Process();
            });
            taskCmdsoftOrderlineNavUpdater.Start();

            // 4.	Меняем связанные с изменяемыми проектами записи сущности «Организация»(account), связи по полям «Заказчик»(customerid) и 
            // «Проектная Организация»(cmdsoft_project_agency), «Эксплуатирующая организация»(mcdsoft_ref_account), «Ген. подрядчик»(cmdsoft_generalcontractor)...
            var taskAccountUpdater = new System.Threading.Tasks.Task(() =>
            {
                var accountUpdater = new AccountUpdater(_serviceContext, opportunities);
                accountUpdater.Process();
            });
            taskAccountUpdater.Start();

            System.Threading.Tasks.Task.WaitAll(taskActivityDeleter, taskAnnotationDeleter, taskCmdsoftPartOfOwnerUpdater, taskCmdsoftOrderlineNavUpdater, taskAccountUpdater);
        }
    }
}