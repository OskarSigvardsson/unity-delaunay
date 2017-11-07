using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class Geom : MonoBehaviour {

		public static bool ToTheLeft(Vector2 p, Vector2 l0, Vector2 l1) {
			return ((l1.x-l0.x)*(p.y-l0.y) - (l1.y-l0.y)*(p.x-l0.x)) >= 0;
		}

		public static bool ToTheRight(Vector2 p, Vector2 l0, Vector2 l1) {
			return !ToTheLeft(p, l0, l1);
		}

		public static bool PointInTriangle(Vector2 p, Vector2 c0, Vector2 c1, Vector2 c2) {
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

		public static Vector2 RotateRightAngle(Vector2 v) {
			var x = v.x;
			v.x = -v.y;
			v.y = x;

			return v;
		}

		public static bool LineLineIntersection(Vector2 p0, Vector2 v0, Vector2 p1, Vector2 v1, out float m0, out float m1) {
			var det = (v0.x * v1.y - v0.y * v1.x);

			if (Mathf.Abs(det) < 0.001f) {
				m0 = float.NaN;
				m1 = float.NaN;

				return false;
			} else {
				m0 = ((p0.y - p1.y) * v1.x - (p0.x - p1.x) * v1.y) / det;
				
				if (Mathf.Abs(v1.x) >= 0.001f) {
					m1 = (p0.x + m0*v0.x - p1.x) / v1.x;
				} else {
					m1 = (p0.y + m0*v0.y - p1.y) / v1.y;
				}

				return true;
			}
		}

		public static Vector2 LineLineIntersection(Vector2 p0, Vector2 v0, Vector2 p1, Vector2 v1) {
			float m0, m1;

			if (LineLineIntersection(p0, v0, p1, v1, out m0, out m1)) {
				return p0 + m0 * v0;
			} else {
				return new Vector2(float.NaN, float.NaN);
			}
		}

		public static Vector2 CircumcircleCenter(Vector2 c0, Vector2 c1, Vector2 c2) {
			var mp0 = 0.5f * (c0 + c1);
			var mp1 = 0.5f * (c1 + c2);

			var v0 = RotateRightAngle(c0 - c1);
			var v1 = RotateRightAngle(c1 - c2);

			float m0, m1;

			Geom.LineLineIntersection(mp0, v0, mp1, v1, out m0, out m1);

			return mp0 + m0 * v0;
		}

		public static Vector2 TriangleCentroid(Vector2 c0, Vector2 c1, Vector2 c2) {
			var val = (1.0f/3.0f) * (c0 + c1 + c2) ;
			return val;
		}
	}
}
