using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class Packet {
		
		public int id;
		public PolyNetPlayer[] recipients;

		public Packet() {
			id = -1;
		}
		public virtual void read(ref BinaryReader reader, PolyNetPlayer sender) {
		}

		public virtual void write(ref BinaryWriter writer) {
		}

		public static Packet getPacket(int id) {
			switch (id) {
			case 0:
				return new PacketObjectSpawn ();
			case 1:
				return new PacketObjectDespawn ();
			case 2:
				return new PacketTransform ();
			case 3:
				return new PacketLogin ();
			case 4:
				return new PacketPlayerTransform ();
			case 5:
				return new PacketPlayerTransformDenied ();
			case 6:
				return new PacketAnimTrigger ();
			case 7:
				return new PacketAnimBool ();
			case 8:
				return new PacketAnim2HandedTrigger ();
			case 9:
				return new PacketAnim2HandedBool ();
			case 10:
				return new PacketPlayerHit ();
			case 11:
				return new PacketMetadata ();
			case 12:
				return new PacketPlaceItem ();
			default:
				return null;
			}
		}

		public static int packageObject(GameObject g) {
			int i = -1;
			PolyNetIdentity identity = g.GetComponent<PolyNetIdentity> ();
			if (identity != null)
				i = identity.getInstanceId ();
			else
				Debug.Log ("SERIOUS FUCK UP: you can't send objects without a network identity over the network dipshit. Well, I sent a -1 as the id in its place - have fun.");
			return i;
		}

		public static GameObject unpackageObject(int i) {
			PolyNetIdentity identity = PolyNetWorld.getObject(i);
			if (identity != null)
				return identity.gameObject;
			else
				return null;
		}

	}

}