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
