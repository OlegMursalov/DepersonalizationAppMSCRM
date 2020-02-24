namespace DepersonalizationApp.Helpers
{
    public class RandomEmailByMaskHelper : RandomByMaskHelper
    {
        public RandomEmailByMaskHelper(string mask) : base(mask, 97, 123)
        {
        }
    }
}