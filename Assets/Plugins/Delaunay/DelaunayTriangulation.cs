using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class DelaunayTriangulation {

		public readonly List<Vector2> Vertices;
		public readonly List<int> Triangles;

		internal DelaunayTriangulation() {
			Vertices = new List<Vector2>();
			Triangles = new List<int>();
		}

		internal void Clear() {
			Vertices.Clear();
			Triangles.Clear();
		}

		public bool Verify() {
			for (int i = 0; i < Triangles.Count; i+=3) {
				var c0 = Vertices[Triangles[i]];
				var c1 = Vertices[Triangles[i+1]];
				var c2 = Vertices[Triangles[i+2]];

				for (int j = 0; j < Vertices.Count; j++) {
					var p = Vertices[j];
					if (Geom.InsideCircumcircle(p, c0, c1, c2)) {
						return false;
					}
				}
			}

			return true;
		}
	}
}
