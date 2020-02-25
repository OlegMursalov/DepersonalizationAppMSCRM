using Microsoft.Xrm.Sdk;
using System;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных activities
    /// </summary>
    public class RelatedActivityDeleter
    {
        private IOrganizationService _orgService;
        private SqlConnection _sqlConnection;
        private Guid[] _regardingObjectIds;

        public RelatedActivityDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds)
        {
            _orgService = orgService;
            _sqlConnection = sqlConnection;
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            // Удаление задач
            var relatedTaskDeleter = new RelatedTaskDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedTaskDeleter.Process();

            // Удаление факсов
            var relatedFaxDeleter = new RelatedFaxDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedFaxDeleter.Process();

            // Удаление звонков
            var relatedPhoneCallDeleter = new RelatedPhoneCallDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedPhoneCallDeleter.Process();

            // Удаление эмеилов
            var relatedEmailDeleter = new RelatedEmailDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedEmailDeleter.Process();

            // Удаление писем
            var relatedLetterDeleter = new RelatedLetterDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedLetterDeleter.Process();

            // Удаление встреч
            var relatedAppointmentDeleter = new RelatedAppointmentDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedAppointmentDeleter.Process();

            // Удаление действий сервиса
            var relatedServiceAppointmentDeleter = new RelatedServiceAppointmentDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedServiceAppointmentDeleter.Process();

            // Удаление откликов от кампании
            var relatedCampaignResponseDeleter = new RelatedCampaignResponseDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedCampaignResponseDeleter.Process();

            // Удаление повторяющихся встреч
            var relatedRecurringAppointmentMasterDeleter = new RelatedRecurringAppointmentMasterDeleter(_orgService, _sqlConnection, _regardingObjectIds);
            relatedRecurringAppointmentMasterDeleter.Process();
        }
    }
}