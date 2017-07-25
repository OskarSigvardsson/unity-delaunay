using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GK {
	public class DebugDelaunay : MonoBehaviour {

		public UnityEngine.UI.Text Text;

		IEnumerator Start() {
			yield return new WaitForSeconds(3.0f);
			//var p = new Vector2(8.9f, 3.5f);
			//var c0 = new Vector2(-118.5f, -14.2f);
			//var c1 = new Vector2(123.9f, -14.2f);
			//var c2 = new Vector2(3.2f, 125.7f);

			//Debug.Log(Geom.Contains(p, c0, c1, c2));
			//return;

			var del = new Delaunay();
			var points = new List<Vector2>();

			while (true) {
				points.Clear();
				var count = 1000;

				for (int i = 0; i < count; i++) {
					points.Add(new Vector2(Random.value, Random.value));
				}

				var t = System.DateTime.UtcNow;
				del.SetPoints(points);
				del.Calculate();
				Text.text = "" + (System.DateTime.UtcNow - t).TotalMilliseconds;

				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}
