using UpdaterApp.Log;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using DepersonalizationApp.DepersonalizationLogic;

namespace UpdaterApp.DepersonalizationLogic
{
    public abstract class BaseUpdater : Base
    {
        /// <summary>
        /// Каждый subclass пусть сам определяет, что ему сделать с пачкой записей
        /// </summary>
        protected abstract void ChangeByRules(IEnumerable<Entity> records);

        public BaseUpdater(IOrganizationService organizationService, QueryExpression mainQuery, int maxAmountOfRecords)
            : base(organizationService, mainQuery, maxAmountOfRecords)
        {
        }

        public void Process()
        {
            RetrieveAll((entities) =>
            {
                ChangeByRules(entities);
                AllUpdate(entities);
            });
        }

        /// <summary>
        /// Обновить измененные записи
        /// </summary>
        private void AllUpdate(IEnumerable<Entity> records)
        {
            var amountOfSuccessful = 0;
            var entityName = _mainQuery.EntityName;
            foreach (var record in records)
            {
                try
                {
                    _organizationService.Update(record);
                    amountOfSuccessful++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{record.Id}' is not updated", ex);
                }
            }
            _logger.Info($"Successful updated '{amountOfSuccessful}' and '{records.Count() - amountOfSuccessful}' are failed records '{entityName}'");
        }
    }
}