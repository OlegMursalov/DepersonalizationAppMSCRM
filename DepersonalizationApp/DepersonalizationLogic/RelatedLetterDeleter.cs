using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedLetterDeleter : BaseDeleter<Letter>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedLetterDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            Letter[] letters = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    letters = (from letter in _serviceContext.LetterSet
                               where letter.RegardingObjectId != null && letter.RegardingObjectId.Id == regObjId
                               select letter).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedLetterDeleter.Process query is failed", ex);
                }
                if (letters != null && letters.Length > 0)
                {
                    AllDelete(letters);
                }
            }
        }
    }
}