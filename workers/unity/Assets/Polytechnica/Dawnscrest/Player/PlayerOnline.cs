using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Dawnscrest.Core;
using Improbable;

namespace Polytechnica.Dawnscrest.Player {

	// Server authoritative player functionality
	public class PlayerOnline : MonoBehaviour {

		[Require] private WorldTransform.Reader WorldTransformReader;

		// Just to make this never turn on if it isnt a
		[Require] private EntityAcl.Writer AclWriter;

		public float MaxTimeout = 1f;

		private float Timeout;
		private CharacterController Character;

		void OnEnable () {
			WorldTransformReader.ComponentUpdated += Heatbeat;
			Character = GetComponent<CharacterController> ();
			Timeout = 0f;
		}

		void OnDisable () {
			WorldTransformReader.ComponentUpdated -= Heatbeat;
		}

		private void Update() {

			// Assume Logout if no word from client
			Timeout += Time.deltaTime;
			if (Timeout > MaxTimeout)
				Logout ();
		}

		private void Heatbeat(WorldTransform.Update update) {
			Timeout = 0f;
		}

		private void Logout() {
			Debug.LogWarning ("Timeout - Kicking Player");
			Character.setToNPC ();
		}
	}

}