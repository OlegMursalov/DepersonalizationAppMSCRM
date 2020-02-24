using DepersonalizationApp.Helpers;
using Xunit;

namespace DepersonalizationApp.Tests
{
    public class RandomTelephoneByMaskHelperTest
    {
        [Fact]
        public void Is_Generating_By_TelephoneMask1_IsMatch()
        {
            var randTelephoneHelper = new RandomTelephoneByMaskHelper(CommonObjsHelper.TelephoneMask1);
            var telephone = randTelephoneHelper.Get();
            Assert.Matches("\\+(\\d{2}) (\\d{3}) (\\d{3})-(\\d{2})-(\\d{2})", telephone);
        }
    }
}