using UpdaterApp.Log;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdaterApp.LogicOfUpdater
{
    public abstract class BaseUpdater
    {
        private IOrganizationService _organizationService;
        private QueryExpression _mainQuery;

        private const int AmountOnPage = 500;

        private readonly ILogger _logger = new FileLogger();

        /// <summary>
        /// Каждый subclass пусть сам определяет, что ему сделать с пачкой записей
        /// </summary>
        protected abstract void ChangeByRules(IEnumerable<Entity> records);

        public BaseUpdater(IOrganizationService organizationService, QueryExpression mainQuery)
        {
            _organizationService = organizationService;
            _mainQuery = mainQuery;
        }

        public void Process()
        {
            /*var queryExpr = new QueryExpression
            {
                EntityName = _entityName,
                ColumnSet = _columnSet,
                Orders =
                {
                    new OrderExpression
                    {
                        AttributeName = "createdon",
                        OrderType = OrderType.Descending
                    }
                }
            };*/

            _mainQuery.PageInfo = new PagingInfo();
            _mainQuery.PageInfo.Count = AmountOnPage;
            _mainQuery.PageInfo.PageNumber = 1;
            _mainQuery.PageInfo.PagingCookie = null;

            while (true)
            {
                var collection = _organizationService.RetrieveMultiple(_mainQuery);
                if (collection.Entities != null)
                {
                    var entities = collection.Entities;
                    ChangeByRules(entities);
                    AllUpdate(entities);
                }
                if (collection.MoreRecords)
                {
                    _mainQuery.PageInfo.PageNumber++;
                    _mainQuery.PageInfo.PagingCookie = collection.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Обновить измененные записи
        /// </summary>
        private void AllUpdate(IEnumerable<Entity> records)
        {
            var amountOfSuccessful = 0;
            var minCreatedOn = DateTime.MaxValue;
            var maxCreatedOn = DateTime.MinValue;
            foreach (var record in records)
            {
                try
                {
                    var createdOn = (DateTime)record["createdon"];
                    if (createdOn < minCreatedOn)
                    {
                        minCreatedOn = createdOn;
                    }
                    if (createdOn > maxCreatedOn)
                    {
                        maxCreatedOn = createdOn;
                    }
                    _organizationService.Update(record);
                    amountOfSuccessful++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{_entityName}' with Id = '{record.Id}' is not updated", ex);
                }
            }
            _logger.Info($"Successful updated '{amountOfSuccessful}' and '{records.Count() - amountOfSuccessful}' are failed records '{_entityName}' from '{minCreatedOn}' to '{maxCreatedOn}'");
        }
    }
}