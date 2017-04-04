using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEffects {

	public class EffectListener : MonoBehaviour {

		private AudioSource source;

		private void Start() {
			source = GetComponent<AudioSource> ();
		}

		public void playEffect(Effect e, Vector3 pos, Vector3 norm, float intensity) {
			//play a random one of the sounds
			if (e.clips.GetLength (0) > 0) {
				source.clip = e.clips [Random.Range (0, e.clips.GetLength (0))];
				source.Play ();
			}
			//spawn the particle system facing normals at position
			if (e.particles != null) {
				GameObject g = Instantiate (e.particles.gameObject);
				g.transform.position = pos;
				g.transform.forward = norm;
				g.transform.Translate (new Vector3 (0f, 0f, 0.3f));
//				float r = g.GetComponent<ParticleSystem> ().emission.rateOverTimeMultiplier;
//				r = intensity;
			}
		}

	}

}
