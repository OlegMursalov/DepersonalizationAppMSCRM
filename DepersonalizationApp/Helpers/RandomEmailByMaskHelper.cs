using System;

namespace DepersonalizationApp.Helpers
{
    public class RandomEmailByMaskHelper
    {
        private char[] _formatChars;
        private Random _random;

        public RandomEmailByMaskHelper(string mask)
        {
            _formatChars = mask.ToCharArray();
            _random = new Random();
        }

        public string Get()
        {
            var newChars = new char[_formatChars.Length];
            for (int i = 0; i < _formatChars.Length; i++)
            {
                if (_formatChars[i] == 'X')
                {
                    _formatChars[i] = Convert.ToChar(_random.Next(0, 26)); // from a to z
                }
                else
                {
                    newChars[i] = _formatChars[i];
                }
            }
            return newChars.ToString();
        }
    }
}