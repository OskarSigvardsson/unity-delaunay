using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public static class VectorExtensions {

		public static bool IsReal(this float f) {
			return !float.IsInfinity(f) && !float.IsNaN(f);
		}

		public static bool IsReal(this Vector2 v2) {
			return v2.x.IsReal() && v2.y.IsReal();
		}

		public static bool IsReal(this Vector3 v3) {
			return v3.x.IsReal() && v3.y.IsReal() && v3.z.IsReal();
		}

	}
}
