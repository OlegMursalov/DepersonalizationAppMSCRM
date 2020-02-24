using System;

namespace DepersonalizationApp.Helpers
{
    public class RandomByMaskHelper
    {
        protected char[] _formatChars;
        private Random _random;
        private int _minRandUnicodeValue;
        private int _maxRandUnicodeValue;

        public RandomByMaskHelper(string mask, int minRandUnicodeValue, int maxRandUnicodeValue)
        {
            _formatChars = mask.ToCharArray();
            _random = new Random();
            _minRandUnicodeValue = minRandUnicodeValue;
            _maxRandUnicodeValue = maxRandUnicodeValue;
        }

        public string Get()
        {
            var newChars = new char[_formatChars.Length];
            for (int i = 0; i < _formatChars.Length; i++)
            {
                if (_formatChars[i] == 'X')
                {
                    int x = _random.Next(_minRandUnicodeValue, _maxRandUnicodeValue);
                    newChars[i] = (char)x;
                }
                else
                {
                    newChars[i] = _formatChars[i];
                }
            }
            return new string(newChars);
        }
    }
}