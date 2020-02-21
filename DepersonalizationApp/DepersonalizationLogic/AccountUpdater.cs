using CRMEntities;
using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class AccountUpdater : BaseUpdater<Account>
    {
        private static int _globalCounterBySessionApp = 1;

        protected IEnumerable<Opportunity> _opportunities;

        public AccountUpdater(OrganizationServiceCtx serviceContext, IEnumerable<Opportunity> opportunities) : base(serviceContext)
        {
            _opportunities = opportunities;
        }

        protected override void ChangeByRules(IEnumerable<Account> accounts)
        {
            var randomTelephoneByMaskHelper = new RandomTelephoneByMaskHelper("+ХХ ХХХ ХХХ-ХХ-ХХ");
            foreach (var account in accounts)
            {
                account.Name = $"Организация №{_globalCounterBySessionApp}";
                account.Telephone1 = randomTelephoneByMaskHelper.Get();
                account.EMailAddress1 = RandomEmailHelper.Get();
                account.WebSiteURL = null;
                account.Address1_PostalCode = null;
                account.Description = null;
                account.cmdsoft_inn = null;
                account.ParentAccountId = null;
                _globalCounterBySessionApp++;

                // Все что есть в примечаниях (Notes) и действиях (actions), связанных с организациями, удалить (сообщения, эл. почта, прикрепленный файлы)
                var accountsGuids = accounts.Select(e => e.Id);

                var activityDeleter = new RelatedActivityDeleter(_serviceContext, accountsGuids);
                activityDeleter.Process();

                var annotationDeleter = new RelatedAnnotationDeleter(_serviceContext, accountsGuids);
                annotationDeleter.Process();
            }
        }

        public override void Process()
        {
            var allAccounts = new List<Account>();
            foreach (var opportunity in _opportunities)
            {
                if (opportunity.CustomerId != null)
                {
                    var account = (from acc in _serviceContext.AccountSet
                                   where acc.Id == opportunity.CustomerId.Id
                                   select acc).FirstOrDefault();
                    if (account != null)
                    {
                        allAccounts.Add(account);
                    }
                }
                if (opportunity.cmdsoft_project_agency != null)
                {
                    var account = (from acc in _serviceContext.AccountSet
                                   where acc.Id == opportunity.cmdsoft_project_agency.Id
                                   select acc).FirstOrDefault();
                    if (account != null)
                    {
                        allAccounts.Add(account);
                    }
                }
                if (opportunity.mcdsoft_ref_account != null)
                {
                    var account = (from acc in _serviceContext.AccountSet
                                   where acc.Id == opportunity.mcdsoft_ref_account.Id
                                   select acc).FirstOrDefault();
                    if (account != null)
                    {
                        allAccounts.Add(account);
                    }
                }
                if (opportunity.cmdsoft_GeneralContractor != null)
                {
                    var account = (from acc in _serviceContext.AccountSet
                                   where acc.Id == opportunity.cmdsoft_GeneralContractor.Id
                                   select acc).FirstOrDefault();
                    if (account != null)
                    {
                        allAccounts.Add(account);
                    }
                }
            }
            var allAccountsDistinct = allAccounts.Distinct(new AccountComparer());
            ChangeByRules(allAccountsDistinct);
            AllUpdate(allAccountsDistinct);
        }

        private class AccountComparer : IEqualityComparer<Account>
        {
            public bool Equals(Account account1, Account account2)
            {
                return account1.Id == account2.Id;
            }

            public int GetHashCode(Account account)
            {
                return account.Id.GetHashCode();
            }
        }
    }
}