using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketObjectDespawn : Packet {

		public int instanceId;

		public PacketObjectDespawn() {
			id = 1;
		}

		public PacketObjectDespawn(PolyNetIdentity i) {
			id = 1;
			instanceId = i.getInstanceId();
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			instanceId = reader.ReadInt32 ();
			despawnObject (instanceId);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (instanceId);
		}

		public void despawnObject(int instanceId) {
			PolyNetIdentity obj = PolyNetWorld.getObject(instanceId);
			if (obj != null) {
				PolyNetWorld.despawnObject (obj);
				GameObject.Destroy (obj.gameObject);
			} else {
				Debug.Log ("Object despawn error: instance not found for id: " + instanceId + ", ignoring despawn.");
			}
		}
	}

}
