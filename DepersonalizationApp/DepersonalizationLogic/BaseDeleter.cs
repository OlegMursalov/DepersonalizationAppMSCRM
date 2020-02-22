using CRMEntities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаляет записи по QueryExpression
    /// </summary>
    public class BaseDeleter<T> : Base<T> where T : Entity
    {
        public BaseDeleter(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
        }

        protected void AllDelete(IEnumerable<T> entities)
        {
            int successfulAmount = 0;
            var entityName = typeof(T).Name;
            foreach (var entity in entities)
            {
                try
                {
                    _orgService.Delete(entity.LogicalName, entity.Id);
                    _logger.Info($"Record '{entityName}' with Id = '{entity.Id}' is deleted");
                    successfulAmount++;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Record '{entityName}' with Id = '{entity.Id}' is not deleted", ex);
                }
            }
            _logger.Info($"{successfulAmount} records '{entityName}' are deleted, {entities.Count() - successfulAmount} are failed");
        }

        protected override T ConvertSqlDataReaderToEntity(SqlDataReader sqlReader)
        {
            
        }
    }
}