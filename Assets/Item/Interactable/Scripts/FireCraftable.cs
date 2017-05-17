using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyWorldOld;

namespace PolyItem {

	public class FireCraftable : FueledCraftable {

		public float maxLight = 2f;
		public float fuelAtMaxLight = 100f;
		public Light lightSource;
		public ParticleSystem flames;

		protected override void setFuled(bool f) {
			base.setFuled(f);

			HeatSource hs = GetComponent<HeatSource> ();
			if (hs != null)
				hs.setHeating (true);
			
			if (isFueled)
				flames.Play ();
			else {
				flames.Stop ();
				lightSource.intensity = 0f;
			}

		}

		protected override void Update() {
			base.Update ();
			if (isFueled) {
				lightSource.intensity = Mathf.Min (1f, (fuel / fuelAtMaxLight)) * maxLight;
			}
		}

	}

}