using System;
using SFML.Window;

namespace SingleSwitchGame
{
    class Utils
    {

        private static readonly Random rnd = new Random();
        public static int RandomInt(int min = 0, int max = 1) { return rnd.Next(min, max); }

        public static float StepTo(float no, float to, float by)
        {
            if (no != to)
            {
		        if (no > to)
			        no -= by;
		        else if (no < to)
			        no += by;
				
		        if (no > to - by && no < to + by)
			        no = to;
	        }
	        return no;
        }

        public static double GetAngle(Vector2f p1, Vector2f p2, bool inDegrees = true)
		{
            return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * (inDegrees ? (180.0 / Math.PI) : 1);
		}
        public static double ToDegrees(double angle) { return angle * (180 / Math.PI); }
        public static double ToRadians(double angle) { return angle * (Math.PI / 180); }
    }
}
