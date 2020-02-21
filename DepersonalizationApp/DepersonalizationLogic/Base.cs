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

        private const int AmountRecordsOnPage = 500;
        private const int MaxAmountOfRecords = 1000;

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
                _logger.Error("RetrieveAll is failed. Action is null");
                return;
            }

            for (int i = 0; i * AmountRecordsOnPage < MaxAmountOfRecords; i++)
            {
                T[] records = null;
                try
                {
                    records = _mainQuery.Skip(i * AmountRecordsOnPage).Take(AmountRecordsOnPage).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Base<{nameof(T)}>.RetrieveAll query is failed", ex);
                }
                if (records != null)
                {
                    action(records);
                }
            }
        }
    }
}