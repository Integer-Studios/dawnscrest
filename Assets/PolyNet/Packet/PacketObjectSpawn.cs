using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketObjectSpawn : Packet {

		public int prefabId;
		public int instanceId;
		public int ownerPlayerId;
		public Vector3 position;
		public Vector3 scale;
		public Vector3 euler;
		public PolyNetIdentity identity;

		public PacketObjectSpawn() {
			id = 0;
		}

		public PacketObjectSpawn(PolyNetIdentity i) {
			id = 0;
			identity = i;
			prefabId = i.prefabId;
			instanceId = i.getInstanceId();
			ownerPlayerId = i.getOwnerId ();
			position = i.transform.position;
			scale = i.transform.localScale;
			euler = i.transform.eulerAngles;
		}

		public override int getSize() {
			return identity.getSpawnSize ();
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			prefabId = reader.ReadInt32 ();
			instanceId = reader.ReadInt32 ();
			ownerPlayerId = reader.ReadInt32 ();

			position = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			scale = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			euler = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());

			createObject (ref reader);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (prefabId);
			writer.Write (instanceId);
			writer.Write (ownerPlayerId);

			writer.Write ((decimal)position.x);
			writer.Write ((decimal)position.y);
			writer.Write ((decimal)position.z);

			writer.Write ((decimal)scale.x);
			writer.Write ((decimal)scale.y);
			writer.Write ((decimal)scale.z);

			writer.Write ((decimal)euler.x);
			writer.Write ((decimal)euler.y);
			writer.Write ((decimal)euler.z);

			identity.writeSpawnData(ref writer);

		}
			
		public void createObject(ref BinaryReader reader) {
			GameObject prefab = PolyNetWorld.getPrefab(prefabId);
			if (prefab != null) {
				GameObject instance = GameObject.Instantiate (prefab);
				instance.transform.position = position;
				instance.transform.localScale = scale;
				instance.transform.eulerAngles = euler;
				identity = instance.GetComponent<PolyNetIdentity> ();
				if (ownerPlayerId == GameObject.FindObjectOfType<PolyNetManager>().playerId)
					identity.isLocalPlayer = true;

				identity.readSpawnData(ref reader);
				PolyNetWorld.spawnObject (identity, instanceId);
			} else {
				Debug.Log ("Object spawn error: prefab not found for id: " + prefabId + ", ignoring spawn.");
			}
		}

	}

}