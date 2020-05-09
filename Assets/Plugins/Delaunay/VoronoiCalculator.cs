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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace GK {
	public class VoronoiCalculator {

		DelaunayCalculator delCalc;
		PTComparer cmp;
		List<PointTriangle> pts;

		/// <summary>
		/// Create new Voronoi calculator.
		/// </summary>
		public VoronoiCalculator() {
			pts = new List<PointTriangle>();
			delCalc = new DelaunayCalculator();
			cmp = new PTComparer();
		}

		/// <summary>
		/// Calculate a voronoi diagram and return it. 
		/// </summary>
		public VoronoiDiagram CalculateDiagram(IList<Vector2> inputVertices) {
			VoronoiDiagram result = null;
			CalculateDiagram(inputVertices, ref result);
			return result;
		}

		/// <summary>
		/// Non-allocating version of CalculateDiagram.
		///
		/// I guess it's not strictly true that it generates NO garbage, because
		/// it might if it has to resize internal buffers, but all buffers are
		/// reused from invocation to invocation. 
		/// </summary>
		public void CalculateDiagram(IList<Vector2> inputVertices, ref VoronoiDiagram result) {
			// TODO: special case for 1 points
			// TODO: special case for 2 points
			// TODO: special case for 3 points
			// TODO: special case for collinear points
			if (inputVertices.Count < 3) {
				throw new NotImplementedException("Not implemented for < 3 vertices");
			}

			if (result == null) {
				result = new VoronoiDiagram();
			}

			var trig = result.Triangulation;

			result.Clear();

			Profiler.BeginSample("Delaunay triangulation");
			delCalc.CalculateTriangulation(inputVertices, ref trig);
			Profiler.EndSample();

			pts.Clear();

			var verts = trig.Vertices;
			var tris = trig.Triangles;
			var centers = result.Vertices;
			var edges = result.Edges;


			if (tris.Count > pts.Capacity)   { pts.Capacity = tris.Count; }
			if (tris.Count > edges.Capacity) { edges.Capacity = tris.Count; }


			for (int ti = 0; ti < tris.Count; ti+=3) {
				var p0 = verts[tris[ti]];
				var p1 = verts[tris[ti+1]];
				var p2 = verts[tris[ti+2]];

				// Triangle is in CCW order
				Debug.Assert(Geom.ToTheLeft(p2, p0, p1));

				centers.Add(Geom.CircumcircleCenter(p0, p1, p2));
			}


			for (int ti = 0; ti < tris.Count; ti+=3) {
				pts.Add(new PointTriangle(tris[ti],   ti));
				pts.Add(new PointTriangle(tris[ti+1], ti));
				pts.Add(new PointTriangle(tris[ti+2], ti));
			}

			cmp.tris = tris;
			cmp.verts = verts;

			Profiler.BeginSample("Sorting");
			pts.Sort(cmp);
			Profiler.EndSample();

			// The comparer lives on between runs of the algorithm, so clear the
			// reference to the arrays so that the reference is lost. It may be
			// the case that the calculator lives on much longer than the
			// results, and not clearing these would keep the results alive,
			// leaking memory.
			cmp.tris = null;
			cmp.verts = null;

			for (int i = 0; i < pts.Count; i++) {
				result.FirstEdgeBySite.Add(edges.Count);

				var start = i;
				var end = -1;

				for (int j = i+1; j < pts.Count; j++) {
					if (pts[i].Point != pts[j].Point) {
						end = j - 1;
						break;
					}
				}

				if (end == -1) {
					end = pts.Count - 1;
				}

				i = end;

				var count = end - start;

				Debug.Assert(count >= 0);

				for (int ptiCurr = start; ptiCurr <= end; ptiCurr++) {
					bool isEdge;

					var ptiNext = ptiCurr + 1;

					if (ptiNext > end) ptiNext = start;

					var ptCurr = pts[ptiCurr];
					var ptNext = pts[ptiNext];

					var tiCurr = ptCurr.Triangle;
					var tiNext = ptNext.Triangle;

					var p0 = verts[ptCurr.Point];

					var v2nan = new Vector2(float.NaN, float.NaN);

					if (count == 0) {
						isEdge = true;
					} else if (count == 1) {

						var cCurr = Geom.TriangleCentroid(verts[tris[tiCurr]], verts[tris[tiCurr+1]], verts[tris[tiCurr+2]]);
						var cNext = Geom.TriangleCentroid(verts[tris[tiNext]], verts[tris[tiNext+1]], verts[tris[tiNext+2]]);

						isEdge = Geom.ToTheLeft(cCurr, p0, cNext);
					} else {
						isEdge = !SharesEdge(tris, tiCurr, tiNext);
					}

					if (isEdge) {
						Vector2 v0, v1;

						if (ptCurr.Point == tris[tiCurr]) {
							v0 = verts[tris[tiCurr+2]] - verts[tris[tiCurr+0]];
						} else if (ptCurr.Point == tris[tiCurr+1]) {
							v0 = verts[tris[tiCurr+0]] - verts[tris[tiCurr+1]];
						} else {
							Debug.Assert(ptCurr.Point == tris[tiCurr+2]);
							v0 = verts[tris[tiCurr+1]] - verts[tris[tiCurr+2]];
						}

						if (ptNext.Point == tris[tiNext]) {
							v1 = verts[tris[tiNext+0]] - verts[tris[tiNext+1]];
						} else if (ptNext.Point == tris[tiNext+1]) {
							v1 = verts[tris[tiNext+1]] - verts[tris[tiNext+2]];
						} else {
							Debug.Assert(ptNext.Point == tris[tiNext+2]);
							v1 = verts[tris[tiNext+2]] - verts[tris[tiNext+0]];
						}

						edges.Add(new VoronoiDiagram.Edge(
							VoronoiDiagram.EdgeType.RayCCW,
							ptCurr.Point,
							tiCurr / 3,
							-1,
							Geom.RotateRightAngle(v0)
						));

						edges.Add(new VoronoiDiagram.Edge(
							VoronoiDiagram.EdgeType.RayCW,
							ptCurr.Point,
							tiNext / 3,
							-1,
							Geom.RotateRightAngle(v1)
						));
					} else {
						if (!Geom.AreCoincident(centers[tiCurr/3], centers[tiNext/3])) {
							edges.Add(new VoronoiDiagram.Edge(
								VoronoiDiagram.EdgeType.Segment,
								ptCurr.Point,
								tiCurr / 3,
								tiNext / 3,
								v2nan
							));
						}
					}
				}
			}
		}

		/// <summary>
		/// Assuming ti0 and ti1 shares an edge, which point of ti0 is not on
		/// ti1?
		/// <summary>
		static int NonSharedPoint(List<int> tris, int ti0, int ti1) {
			Debug.Assert(SharesEdge(tris, ti0, ti1));

			var x0 = tris[ti0];
			var x1 = tris[ti0+1];
			var x2 = tris[ti0+2];

			var y0 = tris[ti1];
			var y1 = tris[ti1+1];
			var y2 = tris[ti1+2];

			if (x0 != y0 && x0 != y1 && x0 != y2) return x0;
			if (x1 != y0 && x1 != y1 && x1 != y2) return x1;
			if (x2 != y0 && x2 != y1 && x2 != y2) return x2;

			Debug.Assert(false);
			return -1;
		}

		static bool SharesEdge(List<int> tris, int ti0, int ti1) {
			var x0 = tris[ti0];
			var x1 = tris[ti0+1];
			var x2 = tris[ti0+2];

			var y0 = tris[ti1];
			var y1 = tris[ti1+1];
			var y2 = tris[ti1+2];

			var n = 0;

			if (x0 == y0 || x0 == y1 || x0 == y2) n++;
			if (x1 == y0 || x1 == y1 || x1 == y2) n++;
			if (x2 == y0 || x2 == y1 || x2 == y2) n++;

			Debug.Assert(n != 3);

			return n >= 2;
		}


		struct PointTriangle {
			public readonly int Point;
			public readonly int Triangle;

			public PointTriangle(int point, int triangle) {
				this.Point = point;
				this.Triangle = triangle;
			}

			public override string ToString() {
				return string.Format("PointTriangle({0}, {1})", Point, Triangle);
			}
		}


		class PTComparer : IComparer<PointTriangle> {
			public List<Vector2> verts;
			public List<int> tris;

			public int Compare(PointTriangle pt0, PointTriangle pt1) {
				if (pt0.Point < pt1.Point) {
					return -1;
				} else if (pt0.Point > pt1.Point) {
					return 1;
				} else if (pt0.Triangle == pt1.Triangle) {
					Debug.Assert(pt0.Point == pt1.Point);
					return 0;
				} else {
					return CompareAngles(pt0, pt1);
				}
			}

			int CompareAngles(PointTriangle pt0, PointTriangle pt1) {
				Debug.Assert(pt0.Point == pt1.Point);

				// "reference" point
				var rp = verts[pt0.Point];

				// triangle centroids in "reference point space"
				var p0 = Centroid(pt0) - rp;
				var p1 = Centroid(pt1) - rp;

				// quadrants. false for 1,2, true for 3,4.
				var q0 = ((p0.y < 0) || ((p0.y == 0) && (p0.x < 0)));
				var q1 = ((p1.y < 0) || ((p1.y == 0) && (p1.y < 0)));

				if (q0 == q1) {
					// p0 and p1 are within 180 degrees of each other, so just
					// use cross product to find out if pt1 is to the left of
					// p0.
					var cp = p0.x*p1.y - p0.y*p1.x;

					if (cp > 0) {
						return -1;
					} else if (cp < 0) {
						return 1;
					} else {
						return 0;
					}
				} else {

					// if q0 != q1, q1 is true, then p0 is in quadrants 1 or 2,
					// and p1 is in quadrants 3 or 4. Hence, pt0 < pt1. If q1
					// is not true, vice versa.
					return q1 ? -1 : 1;
				}
			}

			Vector2 Centroid(PointTriangle pt) {
				var ti = pt.Triangle;
				return Geom.TriangleCentroid(verts[tris[ti]], verts[tris[ti+1]], verts[tris[ti+2]]);
			}
		}
	}
}
