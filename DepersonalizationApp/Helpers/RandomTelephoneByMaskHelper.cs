using System;

namespace DepersonalizationApp.Helpers
{
    public class RandomTelephoneByMaskHelper
    {
        private char[] _formatChars;
        private Random _random;

        public RandomTelephoneByMaskHelper(string mask)
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
                    _formatChars[i] = Convert.ToChar(_random.Next(0, 10));
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
