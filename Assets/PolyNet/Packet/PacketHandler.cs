using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketHandler {

		private static List<PacketEntry> packetQueue = new List<PacketEntry>();

		public static void queuePacket(Packet p, PolyNetPlayer[] r) {
			packetQueue.Add (new PacketEntry(p, r));
		}

		public static void update() {
			PacketEntry[] packets = packetQueue.ToArray ();
			packetQueue.Clear ();

			foreach (PacketEntry entry in packets) {
				deliverPacketEntry (entry);
			}
		}

		public static void serverSendPacket(byte[] buffer, PolyNetPlayer[] players) {
			foreach (PolyNetPlayer player in players) {
				PolyServer.sendMessage (buffer, player);
			}
		}

		public static void clientSendPacket(byte[] buffer) {
			PolyClient.sendMessage (buffer);
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

		private static void deliverPacketEntry(PacketEntry entry) {
			//Routing
			MemoryStream s = new MemoryStream (new byte[1024]);
			BinaryWriter writer = new BinaryWriter(s);
			writer.Write (entry.packet.id);

			//Packet Data
			entry.packet.write (ref writer);

			//Socket Send
			if (PolyClient.isActive)
				clientSendPacket (s.ToArray());
			else if (PolyServer.isActive)
				serverSendPacket (s.ToArray(), entry.recipients);
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