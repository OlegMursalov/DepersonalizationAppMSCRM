using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using UpdaterApp.Log;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class Base
    {
        // fields for retrieving records
        protected IOrganizationService _organizationService;
        protected QueryExpression _mainQuery;
        protected int _maxAmountOfRecords;

        // system fields
        protected const int AmountOnPage = 500;
        protected readonly ILogger _logger = new FileLogger();

        public Base(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
            _maxAmountOfRecords = int.MaxValue;
        }

        public Base(IOrganizationService organizationService, QueryExpression mainQuery)
        {
            _organizationService = organizationService;
            _mainQuery = mainQuery;
            _maxAmountOfRecords = int.MaxValue;
        }

        public Base(IOrganizationService organizationService, QueryExpression mainQuery, int maxAmountOfRecords)
        {
            _organizationService = organizationService;
            _mainQuery = mainQuery;
            _maxAmountOfRecords = maxAmountOfRecords;
        }

        /// <summary>
        /// Метод извлекает из CRM записи и для каждой пачки выполняет действие
        /// </summary>
        protected void RetrieveAll(Action<IEnumerable<Entity>> action)
        {
            if (action == null)
            {
                throw new ArgumentException("RetrieveAll is failed. Action is null");
            }

            int amountOfRecords = 0;

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
                    amountOfRecords += entities.Count;
                    action(entities);
                }

                if (collection.MoreRecords && amountOfRecords <= _maxAmountOfRecords)
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
    }
}