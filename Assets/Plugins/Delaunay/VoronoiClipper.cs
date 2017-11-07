using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class VoronoiClipper {

		struct VertRay {
			public Vector2 Vert;
			public Vector2 Direction;
			public bool Reverse;
		}

		List<VertRay> segsIn = new List<VertRay>();
		List<VertRay> segsOut = new List<VertRay>();

		public void ClipSite(VoronoiDiagram diag, IList<Vector2> polygon, int site, ref List<Vector2> clipped) {
			int firstEdge, lastEdge;

			diag.GetSiteEdgeRange(site, out firstEdge, out lastEdge);

			Debug.Assert(firstEdge >= 0 && firstEdge < diag.Edges.Count);
			Debug.Assert(lastEdge >= 0 && lastEdge < diag.Edges.Count);
			Debug.Assert(diag.Edges[firstEdge].Site == diag.Edges[lastEdge].Site);
			Debug.Assert(firstEdge == 0 || diag.Edges[firstEdge - 1].Site != diag.Edges[firstEdge].Site);
			Debug.Assert(lastEdge == diag.Edges.Count - 1 || diag.Edges[lastEdge].Site != diag.Edges[lastEdge + 1].Site);

			segsIn.Clear();
			segsOut.Clear();

			var nan = new Vector2(float.NaN, float.NaN);

			for (int i = firstEdge; i <= lastEdge; i++) {
				if (diag.Edges[i].Type == VoronoiDiagram.EdgeType.Segment) {
					Debug.Assert(diag.Edges[i].Vert1 == diag.Edges[i == lastEdge ? firstEdge : i + 1].Vert0);

					segsIn.Add(new VertRay {
						Vert = diag.Vertices[diag.Edges[i].Vert0],
						Direction = nan,
					});
				} else if (diag.Edges[i].Type == VoronoiDiagram.EdgeType.Ray) {

					var prev = i == firstEdge ? lastEdge : i - 1;
					var prevRay = diag.Edges[prev].Type == VoronoiDiagram.EdgeType.Ray;

#if UNITY_ASSERTIONS
					var next = i == lastEdge ? firstEdge : i + 1;
					var nextRay = diag.Edges[next].Type == VoronoiDiagram.EdgeType.Ray;

					Debug.Assert(prevRay != nextRay);
#endif

					segsIn.Add(new VertRay {
						Vert = diag.Vertices[diag.Edges[i].Vert0],
						Direction = diag.Edges[i].Direction,
						Reverse = prevRay,
					});
				} else if (diag.Edges[i].Type == VoronoiDiagram.EdgeType.Line) {
					throw new System.NotImplementedException();
				} else {
					Debug.Assert(false);
				}
			}

			// foreach line in polygon
			//   for each vertex/ray in site
			//     if ray:
			//       generate intersection
			//       check if point is inside
			//
			//       if point inside and intersection:
			//          if reverse:
			//            AddVertex(intersection)
			//            AddVertex(point)
			//          else:
			//            AddVertex(point)
			//            AddVertex(intersection)
			//       else if point not inside and intersection:
			//          AddRay(intersection)
			//       else if point inside and not intersection:
			//          AddRay(point)
			//       else if point not inside and not intersection:
			//          pass
			//
			//     if point
			//       p1 = point
			//       p2 = next point
			//
			//       if p1 inside and p2 inside:
			//         AddPoint(p1)
			//       else if p1 not inside and p2 not inside:
			//         pass
			//       else:
			//         mid = intersection between (p1,p2) and line
			//
			//         if p1 inside:
			//           assert(p2 outside)
			//           AddPoint(p1)
			//           AddPoint(mid)
			//         else:
			//           assert(p1 outside)
			//           assert(p2 inside)
			//           AddPoint(mid)
			//           AddPoint(p2)
			//
			//   swap segsIn and segsOut
			//


			for (int pi0 = 0; pi0 < polygon.Count; pi0++) {
				var pi1 = pi0 == polygon.Count - 1 ? 0 : pi0 + 1;

				var lp0 = polygon[pi0];
				var lp1 = polygon[pi1];
				var ld = polygon[pi1] - polygon[pi0];

				for (int si0 = 0; si0 < segsIn.Count; si0++) {
					var s = segsIn[si0];

					var isRay = s.Direction.IsReal();

					if (isRay) {
						var rp = s.Vert;
						var rd = s.Direction;

						float m0, m1;

						var pointInside = Geom.ToTheLeft(rp, lp0, lp1);
						var intersection = Geom.LineLineIntersection(lp0, ld, rp, rd, out m0, out m1);
						intersection = intersection && m1 >= 0;

						if (intersection) {
							var ip = lp0 + m0 * ld;

							if (pointInside) {
								if (s.Reverse) {
									segsOut.Add(Ray(ip, Vector2.Dot(rd, ld) * ld));
									segsOut.Add(Vert(rp));
								} else {
									segsOut.Add(Vert(rp));
									segsOut.Add(Vert(ip));
								}
							} else {
								segsOut.Add(Ray(ip, rd, s.Reverse));
							}
						} else {
							if (pointInside) {
								segsOut.Add(s);
							} else {
								// Ray fully outside, do nothing
							}
						}
					} else {
						var si1 = si0 == segsIn.Count - 1 ? 0 : si0 + 1;
						var rp0 = s.Vert;
						var rp1 = segsIn[si1].Vert;

						var rp0Inside = Geom.ToTheLeft(rp0, lp0, lp1);
						var rp1Inside = Geom.ToTheLeft(rp1, lp0, lp1);

						if (rp0Inside && rp1Inside) {
							segsOut.Add(s);
						} else if (!rp0Inside && !rp1Inside) {
							// Segment fully outside, do nothing
						} else {
							var rd = rp1 - rp0;
							float m0, m1;

							var intersection = Geom.LineLineIntersection(lp0, ld, rp0, rd, out m0, out m1);

							Debug.Assert(intersection);
							Debug.Assert(m1 >= 0.0f && m1 <= 1.0f);

							var ip = rp0 + m1 * rd;

							if (rp0Inside) {
								segsOut.Add(s);
								segsOut.Add(Vert(ip));
							} else if (rp1Inside) {
								segsOut.Add(Vert(ip));
								segsOut.Add(s);
							} else {
								Debug.Assert(false);
							}
						}
					}
				}

				var tmp = segsIn;
				segsIn = segsOut;
				segsOut = tmp;

				segsOut.Clear();
			}

			if (clipped == null) {
				clipped = new List<Vector2>();
			}

			clipped.Clear();

			for (int i = 0; i < segsIn.Count; i++) {
				Debug.Assert(!segsIn[i].Direction.IsReal());
				clipped.Add(segsIn[i].Vert);
			}
		}
		
		VertRay Vert(Vector2 p) {
			return new VertRay {
				Vert      = p,
				Direction = new Vector2(float.NaN, float.NaN),
				Reverse   = false,
			};
		}

		VertRay Ray(Vector2 p, Vector2 d, bool reverse) {
			return new VertRay {
				Vert      = p,
				Direction = d,
				Reverse   = reverse,
			};
		}

	}
}
