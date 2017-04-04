using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEffects {

	public class FXMaterial : MonoBehaviour {

		public MaterialEffects effects;

	}

	[System.Serializable]
	public struct MaterialEffects {
		public Effect hitEffect;
		public Effect stepEffect;
	}

	[System.Serializable]
	public class Effect {
		public AudioClip[] clips;
		public ParticleSystem particles;
	}

}
