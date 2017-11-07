using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GK {
	//public class DebugDelaunay : MonoBehaviour {

	//	public GameObject Ball;
	//	public MeshFilter Visualization;
	//		

	//	void TestBalls() {
	//		var c = new Vector2(10.0f * Random.value, 10.0f * Random.value);
	//		var r = 5.0f * Random.value;

	//		var sb = Instantiate(Ball).transform;

	//		sb.position = c;
	//		sb.localScale = Vector3.one * r * 2.0f;

	//		var a0 = Random.Range(-Mathf.PI, Mathf.PI);
	//		var a1 = Random.Range(-Mathf.PI, Mathf.PI);
	//		var a2 = Random.Range(-Mathf.PI, Mathf.PI);

	//		var c0 = c + new Vector2(r * Mathf.Cos(a0), r * Mathf.Sin(a0));
	//		var c1 = c + new Vector2(r * Mathf.Cos(a1), r * Mathf.Sin(a1));
	//		var c2 = c + new Vector2(r * Mathf.Cos(a2), r * Mathf.Sin(a2));

	//		for (int i = 0; i < 1000; i++) {
	//			var ball = Instantiate(Ball).GetComponent<MeshRenderer>();

	//			var p = new Vector2(10.0f * Random.value, 10.0f * Random.value);

	//			ball.transform.position = p;

	//			if (Geom.InsideCircumcircle(p, c0, c1, c2)) {
	//				ball.material.color = Color.green;
	//			} else {
	//				ball.material.color = Color.red;
	//			}
	//		}
	//	}

	//	IEnumerator Start() {
	//		//TestBalls();
	//		//TestCircumcircle();
	//		//yield return new WaitForSeconds(3.0f);
	//		//var p = new Vector2(8.9f, 3.5f);
	//		//var c0 = new Vector2(-118.5f, -14.2f);
	//		//var c1 = new Vector2(123.9f, -14.2f);
	//		//var c2 = new Vector2(3.2f, 125.7f);

	//		//Debug.Log(Geom.Contains(p, c0, c1, c2));
	//		//return;

	//		var del = new Delaunay();
	//		var points = new List<Vector2>();

	//		Mesh mesh = new Mesh();
	//		mesh.MarkDynamic();
	//		Visualization.sharedMesh = mesh;

	//		List<Vector3> verts = new List<Vector3>();
	//		List<int> tris = new List<int>();

	//		while (true) {
	//			points.Clear();
	//			var count = 100;

	//			for (int i = 0; i < count; i++) {
	//				points.Add(new Vector2(1.0f * Random.value, 1.0f * Random.value));
	//			}

	//			del.SetPoints(points);
	//			del.Calculate();

	//			del.GetTriangulation(verts, tris);

	//			mesh.SetVertices(verts);
	//			mesh.SetTriangles(tris, 0);

	//			for (int i = 0; i < tris.Count; i+=3) {
	//				var p0 = Visualization.transform.TransformPoint(verts[tris[i]]);
	//				var p1 = Visualization.transform.TransformPoint(verts[tris[i+1]]);
	//				var p2 = Visualization.transform.TransformPoint(verts[tris[i+2]]);

	//				Debug.DrawLine(p0, p1, Color.green, 10.0f);
	//				Debug.DrawLine(p1, p2, Color.green, 10.0f);
	//				Debug.DrawLine(p2, p0, Color.green, 10.0f);
	//			}

	//			Debug.Log("Valid: " + Delaunay.IsDelaunayTriangulation(verts, tris));
	//			yield return new WaitForSeconds(10.0f);;
	//		}
	//	}

	//	//void TestCircumcircle() {
	//	//	for (int i = 0; i < 10; i++) {
	//	//		var c0 = new Vector2(10.0f * (Random.value - 0.5f), 10.0f * (Random.value - 0.5f));
	//	//		var c1 = new Vector2(10.0f * (Random.value - 0.5f), 10.0f * (Random.value - 0.5f));
	//	//		var c2 = new Vector2(10.0f * (Random.value - 0.5f), 10.0f * (Random.value - 0.5f));

	//	//		if (!Geom.ToTheLeft(c2, c0, c1)) {
	//	//			var tmp = c1;
	//	//			c1 = c2;
	//	//			c2 = tmp;
	//	//		}
	//	//		var mp0 = 0.5f * (c0 + c1);
	//	//		var mp1 = 0.5f * (c1 + c2);
	//	//		var mp2 = 0.5f * (c2 + c0);

	//	//		var v0 = Rotate(c0 - c1);
	//	//		var v1 = Rotate(c1 - c2);
	//	//		var v2 = Rotate(c2 - c0);

	//	//		float m00, m01;
	//	//		float m10, m11;
	//	//		float m20, m21;

	//	//		Geom.LineLineIntersection(mp0, v0, mp1, v1, out m00, out m01);
	//	//		Geom.LineLineIntersection(mp1, v1, mp2, v2, out m10, out m11);
	//	//		Geom.LineLineIntersection(mp2, v2, mp0, v0, out m20, out m21);

	//	//		var r0 = ((mp0 + m00 * v0) - c0).magnitude;
	//	//		var r1 = ((mp1 + m10 * v1) - c1).magnitude;
	//	//		var r2 = ((mp2 + m20 * v2) - c2).magnitude;

	//	//		Debug.Assert(Mathf.Abs(r0 - r1) < 0.001f);
	//	//		Debug.Assert(Mathf.Abs(r1 - r2) < 0.001f);
	//	//		Debug.Assert(Mathf.Abs(r2 - r0) < 0.001f);

	//	//		var c = (mp0 + m00 * v0);

	//	//		for (int j = 0; j < 100; j++) {
	//	//			var p = new Vector2(100.0f * (Random.value - 0.5f), 100.0f * (Random.value - 0.5f));

	//	//			var inside = Geom.InsideCircumcircle(p, c0, c1, c2);

	//	//			var d = (c - p).magnitude;

	//	//			if ((d <= r0) != inside) {
	//	//				Debug.Assert((d <= r0) == inside);
	//	//				Debug.Log(d);
	//	//				Debug.Log(r0);
	//	//				Debug.Log(inside);

	//	//				var b0 = Instantiate(Ball).transform;
	//	//				var b1 = Instantiate(Ball).transform;
	//	//				var b2 = Instantiate(Ball).transform;

	//	//				var cb = Instantiate(Ball).transform;
	//	//				var pb = Instantiate(Ball).transform;

	//	//				b0.position = c0;
	//	//				b1.position = c1;
	//	//				b2.position = c2;

	//	//				cb.position = c;
	//	//				cb.localScale = Vector3.one * r0 * 2;

	//	//				pb.position = p;

	//	//				pb.GetComponent<MeshRenderer>().material.color = Color.blue;
	//	//				return;
	//	//			}
	//	//		}
	//	//	}
	//	//}
	//}
}
