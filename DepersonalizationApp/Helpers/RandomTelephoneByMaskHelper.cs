namespace DepersonalizationApp.Helpers
{
    public class RandomTelephoneByMaskHelper : RandomByMaskHelper
    {
        public RandomTelephoneByMaskHelper(string mask) : base(mask, 48, 58)
        {
        }
    }
}