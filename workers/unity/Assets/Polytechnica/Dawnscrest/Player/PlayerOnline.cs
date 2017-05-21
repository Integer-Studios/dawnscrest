using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Dawnscrest.Core;
using Improbable;

namespace Polytechnica.Dawnscrest.Player {

	/*
	 * This class is enabled when a character is currently being controller by a remote client
	 * It is onlu enabled server side, and works as a server-side counterpart to PlayerController
	 * Any code that the server must execute authoritatively for remote client characters
	 * is in here.
	 */
	public class PlayerOnline : MonoBehaviour {

		// Isnt used, just wire-tapped so the heartbeat can make sure the player is still online
		[Require] private WorldTransform.Reader WorldTransformReader;

		// This isnt used - its just to make this class disable on clients
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

		/*
		 * Updates every transform packet from client, resets timeout counter
		 */
		private void Heatbeat(WorldTransform.Update update) {
			Timeout = 0f;
		}

		/*
		 * Called when timeout is reached ie the client stopped responing
		 */
		private void Logout() {
			Debug.LogWarning ("Timeout - Kicking Player");
			Character.setToNPC ();
		}
	}

}