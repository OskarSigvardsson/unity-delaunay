using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class DelaunayTriangulation {

		/// <summary>
		/// List of vertices that make up the triangulation
		/// </summary>
		public readonly List<Vector2> Vertices;

		/// <summary>
		/// List of triangles that make up the triangulation. The elements index
		/// the Vertices array. 
		/// </summary>
		public readonly List<int> Triangles;

		internal DelaunayTriangulation() {
			Vertices = new List<Vector2>();
			Triangles = new List<int>();
		}

		internal void Clear() {
			Vertices.Clear();
			Triangles.Clear();
		}

		/// <summary>
		/// Verify that this is an actual Delaunay triangulation
		/// </summary>
		public bool Verify() {
			try {
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
			} catch {
				return false;
			}
		}
	}
}
