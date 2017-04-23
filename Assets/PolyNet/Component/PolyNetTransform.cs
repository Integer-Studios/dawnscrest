using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyNet {

	public class PolyNetTransform : PolyNetBehaviour {

		public bool localPlayerAuthority = false;
		public float sendRate = 9f;

		public void Start() {
			if (localPlayerAuthority && identity.isLocalPlayer)
				StartCoroutine (networkTransform ());
			else if (!localPlayerAuthority && PolyServer.isActive)
				StartCoroutine (networkTransform ());
		}

		public IEnumerator networkTransform() {
			yield return new WaitForSeconds (1f);
			while (true) {
				identity.sendBehaviourPacket (new PacketTransform(this));
				yield return new WaitForSeconds (1f/sendRate);
			}
		}

		public void rpc_networkTransform(PacketTransform t) {
			if (!localPlayerAuthority || !identity.isLocalPlayer) {
				transform.position = t.position;
				transform.eulerAngles = t.euler;
				transform.localScale = t.scale;
			}
		}

		public void cmd_networkTransform(PacketTransform t) {
			transform.position = t.position;
			transform.eulerAngles = t.euler;
			transform.localScale = t.scale;
			identity.sendBehaviourPacket (t);
		}

		public override void handleBehaviourPacket (PacketBehaviour p) {
			base.handleBehaviourPacket (p);
			if (p.id == 2) {
				PacketTransform t = (PacketTransform)p;
				if (PolyClient.isActive)
					rpc_networkTransform (t);
				else if (PolyServer.isActive)
					cmd_networkTransform (t);
			}
		}

	}


}