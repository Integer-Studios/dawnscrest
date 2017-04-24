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
			default:
				return null;
			}
		}

	}

}