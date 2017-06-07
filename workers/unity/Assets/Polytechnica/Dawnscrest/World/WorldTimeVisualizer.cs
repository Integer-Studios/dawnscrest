using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Borodar.FarlandSkies.LowPoly;

namespace Polytechnica.Dawnscrest.World {

	public class WorldTimeVisualizer : MonoBehaviour {

		[Require] private TimeComponent.Reader timeReader;

		public float ipAllowance = 1f;
		public float dayLength = 60f;
		private float time = 0f;
		private float targetTime = 0f;

		private void OnEnable() {
			timeReader.ComponentUpdated += OnTimeUpdated;
		}

		private void Update() {
			time += Time.deltaTime;

			if (Mathf.Abs (time - targetTime) > ipAllowance)
				time = Mathf.Lerp (time, targetTime, 0.5f);

			SkyboxDayNightCycle.Instance.TimeOfDay = GetDayPercent()*100f;
		}

		private void OnTimeUpdated(TimeComponent.Update update) {
			targetTime = update.time.Value;
		}

		public float GetTime() {
			return time;
		}

		public float GetDayTime() {
			return time%dayLength;
		}

		public float GetDayPercent() {
			return GetDayTime () / dayLength;
		}
		
	}

}