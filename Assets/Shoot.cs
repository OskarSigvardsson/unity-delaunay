using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace GK {
	public class Shoot : MonoBehaviour {

		public GameObject Projectile; 
		public float MinDelay = 0.25f;
		public float InitialSpeed = 10.0f;
		public Transform SpawnLocation;

		float lastShot = -1000.0f;

		void Update() {
			var shooting = CrossPlatformInputManager.GetButton("Fire1");

			if (shooting) {
				if (Time.time - lastShot >= MinDelay) {
					lastShot = Time.time;

					var go = Instantiate(Projectile, SpawnLocation.position, SpawnLocation.rotation);

					go.GetComponent<Rigidbody>().velocity = InitialSpeed * Camera.main.transform.forward;
				}
			} 
		}
	}
}
