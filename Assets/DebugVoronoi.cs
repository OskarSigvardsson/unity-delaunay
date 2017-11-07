using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GK {
	public class DebugVoronoi : MonoBehaviour {

		public Text Text;
		public VoronoiDiagram Diagram;
		public DelaunayTriangulation Triangulation;
		public List<List<Vector2>> Clipped;
		public Vector2[] poly = new Vector2[] {
			new Vector2(0,0),
			new Vector2(10, 0),
			new Vector2(10, 10),
			new Vector2(0, 10),
		};

		IEnumerator Start() {
			var ps = new List<Vector2>();
			var vor = new VoronoiCalculator();


			var clip = false;

			while(true) {
				ps.Clear();
				//for (int i = 0; i < 4; i++) {
				//	ps.Add(new Vector2(10.0f * Random.value, 10.0f * Random.value));
				//}
				
				ps.Add(new Vector2(0,5));
				ps.Add(new Vector2(1,0));
				ps.Add(new Vector2(6,3));
				ps.Add(new Vector2(5,0));
				ps.Add(new Vector2(5,1));


				var start = System.DateTime.UtcNow;
				vor.CalculateDiagram(ps, ref Diagram);

				var clipper = new VoronoiClipper();

				Clipped = new List<List<Vector2>>();

				for (int i = 0; i < Diagram.Sites.Count; i++) {
					var clipped = new List<Vector2>();

					clipper.ClipSite(Diagram, poly, i, ref clipped);

					Clipped.Add(clipped);
				}

				clip = !clip;

				var end = System.DateTime.UtcNow;


				if (Text != null) {
					Text.text = "Milliseconds: " + (end - start).TotalMilliseconds;
				} 

				//for (int i = 0; i < Diagram.Vertices.Count; i++) {
				//	Debug.Log(string.Format("Vert {0}: {1}", i, Diagram.Vertices[i]));
				//}

				//for (int i = 0; i < Diagram.Edges.Count; i++) {
				//	Debug.Log(string.Format("Edge {0}: {1}", i, Diagram.Edges[i]));
				//}

				yield return new WaitForSeconds(1.0f);
			}

		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(DebugVoronoi))]
	public class DrawVoronoi : Editor {
		void OnSceneGUI() {

			var diag = ((DebugVoronoi)target).Diagram;

			if (diag == null) return;

			var trig = diag.Triangulation;

			for (int ti = 0; ti < trig.Triangles.Count; ti+=3) {
				var p0 = trig.Vertices[trig.Triangles[ti]];
				var p1 = trig.Vertices[trig.Triangles[ti+1]];
				var p2 = trig.Vertices[trig.Triangles[ti+2]];

				Handles.color = Color.blue;
				Handles.DrawLine(p0, p1);
				Handles.DrawLine(p1, p2);
				Handles.DrawLine(p2, p0);
			}

			for (int i = 0; i < diag.Sites.Count; i++) {
				var p = diag.Sites[i];
				var norm = Vector3.back;
				var radius = 0.05f;

				Handles.color = Color.blue;
				Handles.DrawSolidDisc(p, norm, radius);
			}

			for (int vi = 0; vi < diag.Vertices.Count; vi++) {
				var p = diag.Vertices[vi];
				var norm = Vector3.back;
				var radius = 0.05f;

				Handles.color = Color.red;
				Handles.DrawSolidDisc(p, norm, radius);
			}

			for (int ei = 0; ei < diag.Edges.Count; ei++) {
				var e = diag.Edges[ei];

				Handles.color = Color.red;

				if (e.Type == VoronoiDiagram.EdgeType.Segment) {
					Handles.DrawLine(diag.Vertices[e.Vert0], diag.Vertices[e.Vert1]);
				} else if (e.Type == VoronoiDiagram.EdgeType.Ray) {
					Handles.DrawLine(diag.Vertices[e.Vert0], diag.Vertices[e.Vert0] + 10.0f * e.Direction);
				}
			}

			Handles.color = Color.green;

			var poly = ((DebugVoronoi)target).poly;
	
			for (int i0 = 0; i0 < poly.Length; i0++) {
				var i1 = i0 == poly.Length - 1 ? 0 : i0 + 1;

				Handles.DrawLine(poly[i0], poly[i1]);
			}

			Handles.color = Color.yellow;

			foreach (var region in ((DebugVoronoi)target).Clipped) {
				for (int i0 = 0; i0 < region.Count; i0++) {
					var i1 = i0 == region.Count - 1 ? 0 : i0 + 1;

					Handles.DrawLine(region[i0], region[i1]);
				}
				break;
			}
		}
	}

#endif
}
