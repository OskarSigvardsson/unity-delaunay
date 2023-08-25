/**
 * Copyright 2019 Oskar Sigvardsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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

		public override string ToString()
		{
			string rString = "";

			rString += "#Vertices\n";
			foreach (Vector2 vert in Vertices)
			{
				rString += $"X:{vert.x.ToString()}, Y:{vert.y.ToString()} \n";
			}

			rString += "#Edges of Triangles\n";
			for (int i = 0; i < this.Triangles.Count; i++)
			{
				rString += $"{i.ToString()}: {this.Triangles[i].ToString()}\n";
	
			}

			return rString;
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
