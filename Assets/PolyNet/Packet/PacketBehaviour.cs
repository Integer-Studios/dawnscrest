using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketBehaviour : Packet {

		public int instanceId;
		public int scriptId;

		public PacketBehaviour() {
			id = -1;
		}

		public PacketBehaviour(PolyNetBehaviour b) {
			id = -1;
			instanceId = b.getInstanceId();
			scriptId = b.getScriptId();
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			instanceId = reader.ReadInt32 ();
			scriptId = reader.ReadInt32 ();
			routeToBehaviour ();
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (instanceId);
			writer.Write (scriptId);
		}

		public void routeToBehaviour() {
			PolyNetIdentity i = PolyNetWorld.getObject (instanceId);
			if (i != null) {
				i.routeBehaviourPacket (this);
			}
		}
	}

}