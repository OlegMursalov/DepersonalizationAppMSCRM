using DepersonalizationApp.Helpers;
using System.Linq;
using Xunit;

namespace DepersonalizationApp.Tests
{
    public class RandomRangeHelperTest
    {
        [Fact]
        public void Is_GeneratingRange_Equals_Several_Sums()
        {
            var range1 = RandomRangeHelper.Get(1, 100);
            Assert.Single(range1);
            Assert.Equal(100, range1[0]);

            var range2 = RandomRangeHelper.Get(15, 178);
            Assert.Equal(15, range2.Length);
            Assert.Equal(178, range2.Sum());

            var range3 = RandomRangeHelper.Get(100, 45);
            Assert.Equal(100, range3.Length);
            Assert.Equal(45, range3.Sum());
        }
    }
}