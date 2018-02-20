using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class VoronoiClipper {

		List<Vector2> pointsIn = new List<Vector2>();
		List<Vector2> pointsOut = new List<Vector2>();

		/// <summary>
		/// Create a new Voronoi clipper
		/// </summary>
		public VoronoiClipper() { }

		
		/// <summary>
		/// Clip site of voronoi diagram using polygon (must be convex),
		/// returning the clipped vertices in clipped list. Modifies neither
		/// polygon nor diagram, so can be run in parallel for several sites at
		/// once. 
		/// </summary>
		public void ClipSite(VoronoiDiagram diag, IList<Vector2> polygon, int site, ref List<Vector2> clipped) {
			pointsIn.Clear();

			pointsIn.AddRange(polygon);

			int firstEdge, lastEdge;
			
			if (site == diag.Sites.Count - 1) {
				firstEdge = diag.FirstEdgeBySite[site];
				lastEdge = diag.Edges.Count - 1;
			} else {
				firstEdge = diag.FirstEdgeBySite[site];
				lastEdge = diag.FirstEdgeBySite[site + 1] - 1;
			}

			for (int ei = firstEdge; ei <= lastEdge; ei++) {
				pointsOut.Clear();

				var edge = diag.Edges[ei];

				Vector2 lp, ld;

				if (edge.Type == VoronoiDiagram.EdgeType.RayCCW || edge.Type == VoronoiDiagram.EdgeType.RayCW) {
					lp = diag.Vertices[edge.Vert0];
					ld = edge.Direction;

					if (edge.Type == VoronoiDiagram.EdgeType.RayCW) {
						ld *= -1;
					}
				} else if (edge.Type == VoronoiDiagram.EdgeType.Segment) {
					var lp0 = diag.Vertices[edge.Vert0];
					var lp1 = diag.Vertices[edge.Vert1];

					lp = lp0;
					ld = lp1 - lp0;
				} else if (edge.Type == VoronoiDiagram.EdgeType.Line) {
					throw new NotSupportedException("Haven't implemented voronoi halfplanes yet");
				} else {
					Debug.Assert(false);
					return;
				}

				for (int pi0 = 0; pi0 < pointsIn.Count; pi0++) {
					var pi1 = pi0 == pointsIn.Count - 1 ? 0 : pi0 + 1;

					var p0 = pointsIn[pi0];
					var p1 = pointsIn[pi1];

					var p0Inside = Geom.ToTheLeft(p0, lp, lp + ld);
					var p1Inside = Geom.ToTheLeft(p1, lp, lp + ld);

					if (p0Inside && p1Inside) {
						pointsOut.Add(p1);
					} else if (!p0Inside && !p1Inside) {
						// Do nothing, both are outside
					} else {
						var intersection = Geom.LineLineIntersection(lp, ld.normalized, p0, (p1 - p0).normalized);

						if (p0Inside) {
							pointsOut.Add(intersection);
						} else if (p1Inside) {
							pointsOut.Add(intersection);
							pointsOut.Add(p1);
						} else {
							Debug.Assert(false);
						}
					}
				}

				var tmp = pointsIn;
				pointsIn = pointsOut;
				pointsOut = tmp;
			}

			if (clipped == null) {
				clipped = new List<Vector2>();
			} else {
				clipped.Clear();
			}

			clipped.AddRange(pointsIn);
		}
	}
}
