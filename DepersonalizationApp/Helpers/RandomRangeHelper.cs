using System;

namespace DepersonalizationApp.Helpers
{
    public static class RandomRangeHelper
    {
        public static int[] Get(int amount, int maxSum)
        {
            var maxRandVal = maxSum;
            var rand = new Random();
            var array = new int[amount];
            for (int i = 0; i < array.Length; i++)
            {
                var newElem = rand.Next(0, maxRandVal);
                array[i] = newElem;
                maxRandVal = maxRandVal - newElem;
            }
            array[array.Length - 1] += maxRandVal;
            return array;
        }
    }
}