using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketHandler {

		private static Queue<Packet> packetQueue = new Queue<Packet>();
		private static int blastMax = 20;

		public static void queuePacket(Packet p, PolyNetPlayer[] r) {
			if (PolyServer.isActive) {
				foreach (PolyNetPlayer pl in r) {
					pl.packetQueue.Enqueue (p);
				}
			} else
				packetQueue.Enqueue (p);
		}

		public static void update() {
			if (PolyServer.isActive) {
				foreach (PolyNetPlayer p in PolyServer.players.Values) {
					if (!p.isAccepting)
						continue;
				
					int i = 0;
					while (i < blastMax) {
						if (p.packetQueue.Count == 0)
							break;
						deliverPacket (p.packetQueue.Dequeue (), p);
						i++;
					}
					if (i == blastMax) {
						p.isAccepting = false;
						deliverPacket (new PacketBlast (), p);
					}
				}
			} else {
				for (int j = 0; j < packetQueue.Count; j++) {
					deliverPacket (packetQueue.Dequeue(), null);
				}
			}
		}

		public static void handlePacket(byte[] buffer, PolyNetPlayer player) {
			MemoryStream stream = new MemoryStream (buffer);
			BinaryReader reader = new BinaryReader (stream);
			int id = reader.ReadInt32 ();
			Packet p = Packet.getPacket (id);
			if (p == null)
				Debug.Log ("Unknown packet id: " + id);
			else
				p.read (ref reader, player);
		}

		private static void deliverPacket(Packet packet, PolyNetPlayer recipient) {
			//Routing
			MemoryStream s = new MemoryStream (new byte[1024]);
			BinaryWriter writer = new BinaryWriter(s);
			writer.Write (packet.id);

			//Packet Data
			packet.write (ref writer);

			//Socket Send
			if (PolyClient.isActive)
				PolyClient.sendMessage (s.ToArray());
			else if (PolyServer.isActive)
				PolyServer.sendMessage (s.ToArray(), recipient);
		}

	}

	public struct PacketEntry {
		public Packet packet;
		public PolyNetPlayer[] recipients;
		public PacketEntry(Packet p, PolyNetPlayer[] r) {
			packet = p;
			recipients = r;
		}
	}

}