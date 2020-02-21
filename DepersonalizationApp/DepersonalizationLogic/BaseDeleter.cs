using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdaterApp.Log;

namespace DepersonalizationApp.LogicOfDeleting
{
    public class BaseDeleter
    {
        private IOrganizationService _organizationService;
        private QueryExpression _mainQuery;

        private const int AmountOnPage = 500;

        private readonly ILogger _logger = new FileLogger();

        public BaseDeleter(IOrganizationService organizationService, QueryExpression mainQuery)
        {
            _organizationService = organizationService;
            _mainQuery = mainQuery;
        }

        public void Process()
        {
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
                    AllDelete(entities);
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

        private void AllDelete(IEnumerable<Entity> records)
        {
            var amountOfSuccessful = 0;
            var minCreatedOn = DateTime.MaxValue;
            var maxCreatedOn = DateTime.MinValue;
            var entityName = _mainQuery.EntityName;
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
                    _organizationService.Delete(entityName, record.Id);
                    amountOfSuccessful++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{record.Id}' is not deleted", ex);
                }
            }
            _logger.Info($"Successful deleted '{amountOfSuccessful}' and '{records.Count() - amountOfSuccessful}' are failed records '{entityName}' from '{minCreatedOn}' to '{maxCreatedOn}'");
        }
    }
}