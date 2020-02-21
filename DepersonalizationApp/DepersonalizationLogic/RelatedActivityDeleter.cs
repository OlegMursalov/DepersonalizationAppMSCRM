using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Данный класс общий (удаляет все CRM действия)
    /// </summary>
    public class RelatedActivityDeleter : BaseDeleter<ActivityPointer>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedActivityDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            foreach (var regObjId in _regardingObjectIds)
            {
                // Удаление задач
                var taskRelatedAnnotationDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedTaskDeleter = new RelatedTaskDeleter(_serviceContext, _regardingObjectIds);
                    relatedTaskDeleter.Process();
                });
                taskRelatedAnnotationDeleter.Start();

                // Удаление факсов
                var taskRelatedFaxDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedFaxDeleter = new RelatedFaxDeleter(_serviceContext, _regardingObjectIds);
                    relatedFaxDeleter.Process();
                });
                taskRelatedFaxDeleter.Start();

                // Удаление звонков
                var taskCallRelatedPhoneDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedPhoneCallDeleter = new RelatedPhoneCallDeleter(_serviceContext, _regardingObjectIds);
                    relatedPhoneCallDeleter.Process();
                });
                taskCallRelatedPhoneDeleter.Start();

                // Удаление эмеилов
                var taskRelatedEmailDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedEmailDeleter = new RelatedEmailDeleter(_serviceContext, _regardingObjectIds);
                    relatedEmailDeleter.Process();
                });
                taskRelatedEmailDeleter.Start();

                // Удаление писем
                var taskRelatedLetterDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedLetterDeleter = new RelatedLetterDeleter(_serviceContext, _regardingObjectIds);
                    relatedLetterDeleter.Process();
                });
                taskRelatedLetterDeleter.Start();

                // Удаление встреч
                var taskRelatedAppointmentDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedAppointmentDeleter = new RelatedAppointmentDeleter(_serviceContext, _regardingObjectIds);
                    relatedAppointmentDeleter.Process();
                });
                taskRelatedAppointmentDeleter.Start();

                // Удаление действий сервиса
                var taskRelatedServiceAppointmentDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedServiceAppointmentDeleter = new RelatedServiceAppointmentDeleter(_serviceContext, _regardingObjectIds);
                    relatedServiceAppointmentDeleter.Process();
                });
                taskRelatedServiceAppointmentDeleter.Start();

                // Удаление откликов от кампании
                var taskRelatedCampaignResponseDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedCampaignResponseDeleter = new RelatedCampaignResponseDeleter(_serviceContext, _regardingObjectIds);
                    relatedCampaignResponseDeleter.Process();
                });
                taskRelatedCampaignResponseDeleter.Start();

                // Удаление повторяющихся встреч
                var taskRelatedRecurringAppointmentMasterDeleter = new System.Threading.Tasks.Task(() =>
                {
                    var relatedRecurringAppointmentMasterDeleter = new RelatedRecurringAppointmentMasterDeleter(_serviceContext, _regardingObjectIds);
                    relatedRecurringAppointmentMasterDeleter.Process();
                });
                taskRelatedRecurringAppointmentMasterDeleter.Start();

                System.Threading.Tasks.Task.WaitAll(taskRelatedEmailDeleter, taskRelatedEmailDeleter, taskCallRelatedPhoneDeleter, 
                    taskRelatedEmailDeleter, taskRelatedLetterDeleter, taskRelatedAppointmentDeleter, taskRelatedServiceAppointmentDeleter, 
                    taskRelatedCampaignResponseDeleter, taskRelatedRecurringAppointmentMasterDeleter);
            }
        }
    }
}