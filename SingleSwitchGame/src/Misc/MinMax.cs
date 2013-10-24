using System;

namespace SingleSwitchGame
{
    class MinMax
    {
        public int Max;
        public int Min;

        public MinMax(int min, int max)
        {
            Set(min, max);
        }

        public void Set(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public void Set(int val)
        {
            Min = val;
            Max = val;
        }

        public int Random() { return Utils.RandomInt(Min, Max); }
    }
}
