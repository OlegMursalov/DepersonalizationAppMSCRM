using DepersonalizationApp.Helpers;
using Xunit;

namespace DepersonalizationApp.Tests
{
    public class RandomEmailByMaskHelperTest
    {
        [Fact]
        public void Is_Generating_By_EmailMask1_IsMatch()
        {
            var emailMask1 = CommonObjsHelper.EmailMask1;
            var randEmailHelper = new RandomEmailByMaskHelper(emailMask1);
            var email = randEmailHelper.Get();
            Assert.Matches("(\\b[a-z]{5}\\b)@(\\b[a-z]{5}\\b).(\\b[a-z]{2}\\b)", email);
        }
    }
}