using System;
using SFML.Window;
using SFML.Graphics;

namespace SingleSwitchGame
{
    class Utils
    {

        private static readonly Random rnd = new Random();
        public static int RandomInt(int min = 0, int max = 1) { return rnd.Next(min, max+1); }

        public static float StepTo(float no, float target, float amount)
        {
            if (no == target)
                return no;

            no += no < target ? amount : -amount;
            if (no > target - amount && no < target + amount)
                no = target;

	        return no;
        }
        /// <summary>Similar to StepTo method but for Angles.</summary>
        public static float RotateTowards(float angle, float target, float amount)
        {
            if (angle == target)
                return angle;

            // Counter-Clockwise?
            if (!Utils.RotateClockwise(angle, target))
                amount = -amount;

            // Add (without going past 180 or -180)
            if (angle + amount > 180)
                angle = angle + amount - 360;
            else if (angle + amount < -180)
                angle = angle + amount + 360;
            else
                angle += amount;


            if (angle > target - amount && angle < target + amount)
                angle = target;

            return angle;
        }

        public static double GetAngle(Vector2f p1, Vector2f p2, bool inDegrees = true)
		{
            return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) * (inDegrees ? (180.0 / Math.PI) : 1);
		}
        public static double ToDegrees(double angle) { return angle * (180 / Math.PI); }
        public static double ToRadians(double angle) { return angle * (Math.PI / 180); }

        /// <summary>Returns true if the fastest direction to rotate towards targetRotation is Clockwise.</summary>
        public static bool RotateClockwise(double currentRotation, double targetRotation)
		{
			return (currentRotation - targetRotation + 360) % 360 > 180;
		}

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

            float answer = (float)Math.Sqrt((l * l) + (h * h));
			
			return answer;
		}


        ////----------------------
        //// Collision Functions
        ////----------------------

        /// <summary>Returns interection Vector2f, otherwise null.</summary>
        public static dynamic LineIntersectLine(Vector2f A, Vector2f B, Vector2f E, Vector2f F, bool AsSegments = true)
		{
			Vector2f ip;
            float a1, a2, b1, b2, c1, c2;
		 
			a1 = B.Y-A.Y;
			b1 = A.X-B.X;
			c1 = B.X*A.Y - A.X*B.Y;
			a2 = F.Y-E.Y;
			b2 = E.X-F.X;
			c2 = F.X*E.Y - E.X*F.Y;
		 
			float denom = a1*b2 - a2*b1;
			if (denom == 0)
				return null;
			
			ip = new Vector2f();
			ip.X = (b1*c2 - b2*c1)/denom;
			ip.Y = (a2*c1 - a1*c2)/denom;
		 
			//---------------------------------------------------
			// Do checks to see if intersection to endpoints
			// distance is longer than actual Segments.
			// Return null if it is with any.
			//---------------------------------------------------
            if (AsSegments)
			{
				if ((int)((ip.X - A.X) * (ip.X - B.X)) > 0 ||
                    (int)((ip.Y - A.Y) * (ip.Y - B.Y)) > 0 ||
					(int)((ip.X - E.X) * (ip.X - F.X)) > 0 ||
                    (int)((ip.Y - E.Y) * (ip.Y - F.Y)) > 0)
					return null;
			}
			
			return ip;
		}

        public static bool InBounds(FloatRect bounds, Vector2f point) { return point.X >= bounds.Left && point.Y >= bounds.Top && point.X <= bounds.Width && point.Y <= bounds.Height; }
        /// <summary>Returns interection Vector2f, otherwise null.</summary>
        public static dynamic RaycastAgainstBounds(FloatRect bounds, Vector2f point1, Vector2f point2)
        {
            dynamic intersect_top = LineIntersectLine(point1, point2, new Vector2f(bounds.Left, bounds.Top), new Vector2f(bounds.Width, bounds.Top));
            dynamic intersect_bottom = LineIntersectLine(point1, point2, new Vector2f(bounds.Left, bounds.Height), new Vector2f(bounds.Width, bounds.Height));
            dynamic intersect_left = LineIntersectLine(point1, point2, new Vector2f(bounds.Left, bounds.Top), new Vector2f(bounds.Left, bounds.Height));
            dynamic intersect_right = LineIntersectLine(point1, point2, new Vector2f(bounds.Width, bounds.Top), new Vector2f(bounds.Width, bounds.Height));

            if (intersect_left is Vector2f)
                return intersect_left;
            else if (intersect_right is Vector2f)
                return intersect_right;
            else if (intersect_top is Vector2f)
                return intersect_top;
            else if (intersect_bottom is Vector2f)
                return intersect_bottom;

            return null;
        }

        /// <summary>Returns true if point is inside the circle.</summary>
        public static bool InCircle(Vector2f circleCenter, float radius, Vector2f point)
        {
            double square_dist = Math.Pow(point.X - circleCenter.X, 2) + Math.Pow(point.Y - circleCenter.Y, 2);
            return square_dist <= Math.Pow(radius, 2);
        }
        public static bool InCircle(CircleShape circle, Vector2f point) { return InCircle(circle.Position, circle.Radius, point); }

        public static bool CircleCircleCollision(Vector2f circle1Center, float circle1Radius, Vector2f circle2Center, float circle2Radius)
        {
            float radius = circle1Radius + circle2Radius;
            float deltaX = circle1Center.X - circle2Center.X;
            float deltaY = circle1Center.Y - circle2Center.Y;
            return (deltaX * deltaX) + (deltaY * deltaY) <= (radius * radius);
        }
        public static bool CircleCircleCollision(CircleShape circle1, CircleShape circle2) { return CircleCircleCollision(circle1.Position, circle1.Radius, circle2.Position, circle2.Radius); }


        /// <summary>Checks collision between Circle and Rotated Rectangle.</summary>
        /// <param name="rectAngle">In Degrees.</param>
        public static bool CircleRectangleCollision(Vector2f circleCenter, float circleRadius, RectangleShape rect, float rectAngle = 0, Vector2f rectOffset = default(Vector2f))
        {
            Vector2f rectPos = new Vector2f(rect.Position.X + rectOffset.X, rect.Position.Y + rectOffset.Y);
            Vector2f rectCenter = new Vector2f(rectPos.X + (rect.Size.X / 2), rectPos.Y + (rect.Size.Y / 2));

            // Rotate circle's center point back
           
            Vector2f unrotatedCircle = new Vector2f();
            unrotatedCircle.X = (float)Math.Cos(rectAngle) * (circleCenter.X - rectCenter.X) - (float)Math.Sin(rectAngle) * (circleCenter.Y - rectCenter.Y) + rectCenter.X;
            unrotatedCircle.Y = (float)Math.Sin(rectAngle) * (circleCenter.X - rectCenter.X) + (float)Math.Cos(rectAngle) * (circleCenter.Y - rectCenter.Y) + rectCenter.Y;

            // Closest point in the rectangle to the center of circle rotated backwards(unrotated)
            Vector2f closest = new Vector2f();

            // Find the unrotated closest x point from center of unrotated circle
            if (unrotatedCircle.X < rectPos.X)
                closest.X = rectPos.X;
            else if (unrotatedCircle.X > rectPos.X + rect.Size.X)
                closest.X = rectPos.X + rect.Size.X;
            else
                closest.X = unrotatedCircle.X;

            // Find the unrotated closest y point from center of unrotated circle
            if (unrotatedCircle.Y < rectPos.Y)
                closest.Y = rectPos.Y;
            else if (unrotatedCircle.Y > rectPos.Y + rect.Size.Y)
                closest.Y = rectPos.Y + rect.Size.Y;
            else
                closest.Y = unrotatedCircle.Y;

            // Determine collision
            return Distance(unrotatedCircle, closest) < circleRadius;
        }
        /// <summary>Checks collision between Circle and Rotated Rectangle.</summary>
        /// <param name="rectAngle">In Degrees.</param>
        public static bool CircleRectangleCollision(CircleShape circle, RectangleShape rect, float rectAngle = 0, Vector2f rectOffset = default(Vector2f))
        {
            return CircleRectangleCollision(circle.Position, circle.Radius, rect, rectAngle, rectOffset);
        }

    }
}
