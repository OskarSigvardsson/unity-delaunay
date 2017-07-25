using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class Delaunay {

		List<Vector2> points;
		List<Triangle> triangles;

		public Delaunay() {
			points = new List<Vector2>();
			triangles = new List<Triangle>();
		}

		public void SetPoints(List<Vector2> points) {
			if (points.Count < 1) {
				throw new System.ArgumentException("Needs at least three points");
			}

			Rect bounds = new Rect {
				Min = points[0],
				Max = points[0]
			};

			this.points.Clear();
			this.triangles.Clear();

			this.points.Add(Vector2.zero);
			this.points.Add(Vector2.zero);
			this.points.Add(Vector2.zero);

			for (int i = 0; i < points.Count; i++) {
				bounds.Encapsulate(points[i]);
				this.points.Add(points[i]);
			}

			//for (int i = 2; i < this.points.Count - 1; i++) {
			//	var j = Random.Range(i, this.points.Count);

			//	var tmp = this.points[i];
			//	this.points[i] = this.points[j];
			//	this.points[j] = tmp;
			//}

			//var dims = bounds.Size;
			//var size = Mathf.Max(dims.x, dims.y);
			//var center = bounds.Center;
			//var min = center - 0.5f * size * Vector2.one;
			//var max = center + 0.5f * size * Vector2.one;

			//var diag = Mathf.Sqrt(2) * size;
			//var dist = 2.0f * diag;

			//this.points[0] = new Vector2(min.x - dist, min.y);
			//this.points[1] = new Vector2(max.x + dist, min.y);
			//this.points[2] = new Vector2(center.x, max.y + dist);
			
			var center = bounds.Center;
			var size = bounds.Size;

			var c0 = center - new Vector2(size.x, 0.5f*size.y);
			var c1 = center + new Vector2(size.x, -0.5f*size.y);
			var c2 = center + new Vector2(0.0f, 1.5f*size.y);
			var centroid = (c0 + c1 + c2) / 3.0f;

			this.points[0] = centroid + 2.0f * (c0 - centroid);
			this.points[1] = centroid + 2.0f * (c1 - centroid);
			this.points[2] = centroid + 2.0f * (c2 - centroid);

			triangles.Add(new Triangle(0, 1, 2));
		}

		public void Calculate() {
			//for (int i = 0; i < points.Count; i++) {
				//Debug.Log(string.Format("{0}: {1}", i, points[i]));
			//}

			for (int i = 3; i < points.Count; i++) {
				var p = points[i];
				var ti = FindTriangle(p);

				var p0 = triangles[ti].P0;
				var p1 = triangles[ti].P1;
				var p2 = triangles[ti].P2;

				var nti0 = triangles.Count; 
				var nti1 = nti0 + 1;
				var nti2 = nti0 + 2;

				var nt0 = new Triangle(i, p0, p1);
				var nt1 = new Triangle(i, p1, p2);
				var nt2 = new Triangle(i, p2, p0);

				nt0.A0 = triangles[ti].A2;
				nt1.A0 = triangles[ti].A0;
				nt2.A0 = triangles[ti].A1;

				nt0.A1 = nti1;
				nt1.A1 = nti2;
				nt2.A1 = nti0;

				nt0.A2 = nti2;
				nt1.A2 = nti0;
				nt2.A2 = nti1;

				var nt = triangles[ti];

				nt.C0 = nti0;
				nt.C1 = nti1;
				nt.C2 = nti2;

				triangles[ti] = nt;

				triangles.Add(nt0);
				triangles.Add(nt1);
				triangles.Add(nt2);

				LegalizeEdge(nti0, nt0.A0, i, p0, p1);
				LegalizeEdge(nti1, nt1.A0, i, p1, p2);
				LegalizeEdge(nti2, nt2.A0, i, p2, p0);
			}

			//for (int i = 0; i < triangles.Count; i++) {
				//Debug.Log(string.Format("{0}: {1}", i, triangles[i].ToString()));
			//}

			//SanityCheck();
		}

		void SanityCheck() {
			for (int i = 0; i < triangles.Count; i++) {
				var t = triangles[i];

				if (!t.IsLeaf) continue;

				for (int j = 0; j < points.Count; j++) {
					if (t.P0 != j && t.P1 != j && t.P2 != j) {
						var p = points[j];
						var c0 = points[t.P0];
						var c1 = points[t.P1];
						var c2 = points[t.P2];

						var inside = Geom.InsideCircumcircle(p, c0, c1, c2);

						if (inside) {
							Debug.Assert(!inside);

							Debug.Log(j);
							Debug.Log(t);
						}
					}
				}
			}
		}

		int LeafWithEdge(int ti, int e0, int e1) {

			while (!triangles[ti].IsLeaf) {
				var t = triangles[ti];

				if (t.C0 != -1 && triangles[t.C0].HasEdge(e0, e1)) {
					ti = t.C0;
				} else if (t.C1 != -1 && triangles[t.C1].HasEdge(e0, e1)) {
					ti = t.C1;
				} else if (t.C2 != -1 && triangles[t.C2].HasEdge(e0, e1)) {
					ti = t.C2;
				} else {
					throw new System.Exception("This shouldn't happen");
				}
			}

			return ti;
		}

		void LegalizeEdge(int ti0, int ti1, int pi, int li0, int li1) {
			if (ti1 == -1) {
				return;
			}

			ti1 = LeafWithEdge(ti1, li0, li1);

			var t0 = triangles[ti0];
			var t1 = triangles[ti1];

			Debug.Assert(t0.IsLeaf);
			Debug.Assert(t1.IsLeaf);

			var qi = t1.OtherPoint(li0, li1);

			var p = points[pi];
			var q = points[qi];
			var l0 = points[li0];
			var l1 = points[li1];

			if (Geom.InsideCircumcircle(q, p, l0, l1)) {
				var ti2 = triangles.Count;
				var ti3 = ti2 + 1;

				var t2 = new Triangle(pi, li0, qi);
				var t3 = new Triangle(pi, qi, li1);

				t2.A0 = t1.Opposite(li1);
				t2.A1 = ti3;
				t2.A2 = t0.Opposite(li1);

				t3.A0 = t1.Opposite(li0);
				t3.A1 = t0.Opposite(li0);
				t3.A2 = ti2;

				triangles.Add(t2);
				triangles.Add(t3);

				var nt0 = triangles[ti0];
				var nt1 = triangles[ti1];

				nt0.C0 = ti2;
				nt0.C1 = ti3;

				nt1.C0 = ti2;
				nt1.C1 = ti3;

				triangles[ti0] = nt0;
				triangles[ti1] = nt1;

				LegalizeEdge(ti2, t2.A0, pi, li0, qi);
				LegalizeEdge(ti3, t3.A0, pi, qi, li1);
			} 
		}

		int FindTriangle(Vector2 point) {
			var curr = 0;

			while (!triangles[curr].IsLeaf) {
				var t = triangles[curr];

				if (t.C0 >= 0 && PointInTriangle(point, t.C0)) {
					curr = t.C0;
				} else if (t.C1 >= 0 && PointInTriangle(point, t.C1)) {
					curr = t.C1;
				} else {
					//Debug.Assert(t.C2 >= 0 && PointInTriangle(point, t.C2));

					curr = t.C2;
				}
			}

			return curr;
		}

		bool PointInTriangle(Vector2 p, int t) {
			var tr = triangles[t];
			return Geom.Contains(p, points[tr.P0], points[tr.P1], points[tr.P2]);
		}

		struct Rect {
			public Vector2 Min;
			public Vector2 Max;

			public Vector2 Size {
				get {
					return Max - Min;
				}
			}

			public Vector2 Center {
				get {
					return 0.5f * (Min + Max);
				}
			}

			public void Encapsulate(Vector2 point) {
				if (point.x <= Min.x) {
					Min.x = point.x;
				} 
				if (point.x >= Max.x) {
					Max.x = point.x;
				} 
				if (point.y <= Min.y) {
					Min.y = point.y;
				} 
			   	if (point.y >= Max.y) {
					Max.y = point.y;
				}
			}
		}

		struct Triangle {
			public int P0;
			public int P1;
			public int P2;

			public int C0;
			public int C1;
			public int C2;

			public int A0;
			public int A1;
			public int A2;

			public bool IsLeaf {
				get {
					return C0 < 0 && C1 < 0 && C2 < 0;
				}
			}

			public Triangle(int P0, int P1, int P2) {
				this.P0 = P0;
				this.P1 = P1;
				this.P2 = P2;

				this.C0 = -1;
				this.C1 = -1;
				this.C2 = -1;

				this.A0 = -1;
				this.A1 = -1;
				this.A2 = -1;
			}

			public bool HasEdge(int e0, int e1) {
				if (e0 == P0) {
					return e1 == P1 || e1 == P2;
				} else if (e0 == P1) {
					return e1 == P0 || e1 == P2;
				} else if (e0 == P2) {
					return e1 == P0 || e1 == P1;
				}

				return false;
			}

			public int OtherPoint(int p0, int p1) {
				if (p0 == P0) {
					if (p1 == P1) return P2;
					if (p1 == P2) return P1;
					throw new System.ArgumentException("p0 and p1 not on triangle");
				}
				if (p0 == P1) {
					if (p1 == P0) return P2;
					if (p1 == P2) return P0;
					throw new System.ArgumentException("p0 and p1 not on triangle");
				}
				if (p0 == P2) {
					if (p1 == P0) return P1;
					if (p1 == P1) return P0;
					throw new System.ArgumentException("p0 and p1 not on triangle");
				}

				throw new System.ArgumentException("p0 and p1 not on triangle");
			}

			public int Opposite(int p) {
				if (p == P0) return A0;
				if (p == P1) return A1;
				if (p == P2) return A2;
				throw new System.ArgumentException("p not in triangle");
			}

			public override string ToString() {
				if (IsLeaf) {
					return string.Format("Triangle({0}, {1}, {2})", P0, P1, P2);
				} else {
					return string.Format("Triangle({0}, {1}, {2}, {3}, {4}, {5})", P0, P1, P2, C0, C1, C2);
				}
			}
		}
	}
}
