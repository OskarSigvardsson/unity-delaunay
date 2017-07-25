using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class Geom : MonoBehaviour {

		public static bool ToTheLeft(Vector2 p, Vector2 l0, Vector2 l1) {
			return ((l1.x-l0.x)*(p.y-l0.y) - (l1.y-l0.y)*(p.x-l0.x)) >= 0;
		}

		public static bool Contains(Vector2 p, Vector2 c0, Vector2 c1, Vector2 c2) {
			return ToTheLeft(p, c0, c1)
				&& ToTheLeft(p, c1, c2)
				&& ToTheLeft(p, c2, c0);
		}

		public static bool InsideCircumcircle(Vector2 p, Vector2 c0, Vector2 c1, Vector2 c2) {
			var ax = c0.x-p.x;
			var ay = c0.y-p.y;
			var bx = c1.x-p.x;
			var by = c1.y-p.y;
			var cx = c2.x-p.x;
			var cy = c2.y-p.y;

			return (
					(ax*ax + ay*ay) * (bx*cy-cx*by) -
					(bx*bx + by*by) * (ax*cy-cx*ay) +
					(cx*cx + cy*cy) * (ax*by-bx*ay)
			) > 0;
		}
	}
}
