using DepersonalizationApp.Helpers;
using Xunit;

namespace DepersonalizationApp.Tests
{
    public class RandomHelperTest
    {
        [Fact]
        public void Is_Generating_Decimal_In_Diapason()
        {
            var randomHelper = new RandomHelper();
            for (int i = 0; i < 1000; i++)
            {
                var x = randomHelper.GetDecimal(0, 100);
                Assert.True(x >= 0 && x < 100);
            }
        }
    }
}