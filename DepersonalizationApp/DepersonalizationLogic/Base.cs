using CRMEntities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdaterApp.Log;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class Base<T> where T : Entity
    {
        protected IQueryable<T> _mainQuery;
        protected OrganizationServiceCtx _serviceContext;
        protected ILogger _logger = new FileLogger();

        public Base(OrganizationServiceCtx serviceContext)
        {
            _serviceContext = serviceContext;
        }

        /// <summary>
        /// Метод извлекает из CRM записи и для каждой пачки выполняет действие
        /// </summary>
        protected void RetrieveAll(Action<IEnumerable<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentException("RetrieveAll is failed. Action is null");
            }

            var amountRecordsOnPage = 500;
            var maxAmountOfRecords = 1000;
            for (int i = 0; i * amountRecordsOnPage < maxAmountOfRecords; i++)
            {
                var records = _mainQuery.Skip(i * amountRecordsOnPage).Take(amountRecordsOnPage).AsEnumerable();
                action(records);
            }
        }
    }
}