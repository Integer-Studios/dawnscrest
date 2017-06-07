using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Dawnscrest.World {

	public class WorldTime : MonoBehaviour {

		[Require] private TimeComponent.Writer timeWriter;

		public float interpolationRate = 9f;
		private float time = 0f;

		private void Update() {
			time += Time.deltaTime;
			StartCoroutine (UpdateTime());
		}

		private IEnumerator UpdateTime() {
			while (true) {
				timeWriter.Send (new TimeComponent.Update ()
					.SetTime(time)
				);
				yield return new WaitForSeconds (1f / interpolationRate);
			}
		}

	}

}