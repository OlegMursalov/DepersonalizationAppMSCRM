using System;

namespace DepersonalizationApp.Helpers
{
    public class RandomHelper
    {
        private Random _random;

        public RandomHelper()
        {
            _random = new Random();
        }

        /// <summary>
        /// Возвращает random decimal value. Маловероятно, что будет minValue.
        /// MaxValue не вернет никогда.
        /// </summary>
        public decimal GetDecimal(int minValue, int maxValue)
        {
            decimal i = _random.Next(0, 100);
            decimal c = i + (decimal)_random.NextDouble();
            return c;
        }
    }
}