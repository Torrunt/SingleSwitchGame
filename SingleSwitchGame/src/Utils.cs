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

        /// <param name="angle">In Degrees</param>
        public static Vector2f GetPointInDirection(dynamic point, float angle, float distance)
		{
            angle = (float)Utils.ToRadians(angle);
			return new Vector2f(point.X + ((float)Math.Cos(angle) * distance), point.Y + ((float)Math.Sin(angle) * distance));
		}

        /// <summary> The distance between two objects (that have X and Y variables).</summary>
        public static float Distance(dynamic obj1, dynamic obj2)
		{
			float l = Math.Abs(obj1.X - obj2.X);
			float h = Math.Abs(obj1.Y - obj2.Y);

            float answer = (float)Math.Sqrt(Math.Pow(l, 2) + Math.Pow(h, 2));
			
			return answer;
		}
    }
}
