using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PolyNetBehaviour : MonoBehaviour {

		private int scriptId;
		protected PolyNetIdentity identity;

		public virtual int getInstanceId() {
			return identity.getInstanceId();
		}

		public int getScriptId() {
			return scriptId;
		}

		public void setScriptId(int sid) {
			scriptId = sid;
		}

		public void setIdentity(PolyNetIdentity i) {
			identity = i;
		}

		public virtual void handleBehaviourPacket(PacketBehaviour p) {

		}

		public virtual void writeBehaviourSpawnData(ref BinaryWriter writer) {
			
		}

		public virtual void readBehaviourSpawnData(ref BinaryReader reader) {
			
		}

	}


}