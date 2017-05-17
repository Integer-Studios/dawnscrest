using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using Borodar.FarlandSkies.LowPoly;

namespace PolyWorld {

	public class WorldTime : PolyNetBehaviour {

		// Vars : public, protected, private, hide
		public float dayLength;
		public float startTime;
		public float tempuratureVolatility = 1f;

		private static WorldTime worldTime;
		private static float tempVolatility;

		//TODO Syncvars
		private float time = 0f;
	

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public static float getTime() {
			return worldTime.time;
		}

		public static float getDayTime() {
			return worldTime.time%worldTime.dayLength;
		}

		public static float getDayPercent() {
			return getDayTime () / worldTime.dayLength;
		}

		public static float getTempuratureMultiple() {
			return (tempVolatility * Mathf.Sin(2f*getDayPercent()*3.14f) + 0.5f);
		}

		public static bool isDay() {
			return false;
		}

		public static bool isNight() {
			return false;
		}

		public static bool isSunrise() {
			return false;
		}

		public static bool isSunset() {
			return false;
		}

		/*
		 * 
		 * Private
		 * 
		 */

		private void Start() {
			worldTime = this;
			tempVolatility = tempuratureVolatility / 2f;
			time = startTime;
		}

		private void Update() {
			SkyboxDayNightCycle.Instance.TimeOfDay = getDayPercent()*100f;

//			if (!PolyServer.isActive)
//				return;
			
			time += Time.deltaTime;

		}


	}

}
