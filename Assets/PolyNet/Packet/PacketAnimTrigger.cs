using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketAnimTrigger : PacketBehaviour {

		public int triggerId;

		public PacketAnimTrigger() {
			id = 6;
		}

		public PacketAnimTrigger(PolyNetBehaviour b, int tid) : base(b){
			id = 6;
			triggerId = tid;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			triggerId = reader.ReadInt32 ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (triggerId);
			base.write (ref writer);
		}

	}

}