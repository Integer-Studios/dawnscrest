using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketHandler {

		private static List<PacketReceipt> packetReceipts = new List<PacketReceipt>();

		public static void update() {
			
			PacketReceipt[] receipts;
			lock (packetReceipts) {
				receipts = packetReceipts.ToArray ();
				packetReceipts.Clear ();
			}
			foreach (PacketReceipt r in receipts) {
				handlePacket (r.packet, r.sender);
			}

		}

		public static void sendPacket(Packet p, PolyNetPlayer[] r) {
			
			if (PolyServer.isActive) {
				foreach (PolyNetPlayer pl in r) {
					deliverPacket (p, pl);
				}
			} else
				deliverPacket (p, null);
		}

		private static void deliverPacket(Packet packet, PolyNetPlayer recipient) {
			//Routing
			MemoryStream s = new MemoryStream (new byte[1024]);
			BinaryWriter writer = new BinaryWriter(s);
			writer.Write (packet.id);

			//Packet Data
			packet.write (ref writer);

			//Socket Send
			if (PolyClient.isActive) {
				PolyClient.sendMessage (s.ToArray ());
			} else if (PolyServer.isActive) {
				PolyServer.sendMessage (s.ToArray (), recipient);
			}
		}

		public static void receivePacket(byte[] buffer, PolyNetPlayer player) {
			lock (packetReceipts) {
				packetReceipts.Add (new PacketReceipt (buffer, player));
			}
		}

		private static void handlePacket(byte[] buffer, PolyNetPlayer player) {
			MemoryStream stream = new MemoryStream (buffer);
			BinaryReader reader = new BinaryReader (stream);
			int id = reader.ReadInt32 ();
			Packet p = Packet.getPacket (id);
			if (p == null)
				Debug.Log ("Unknown packet id: " + id);
			else
				p.read (ref reader, player);
		}

	}

	public struct PacketReceipt {
		public byte[] packet;
		public PolyNetPlayer sender;
		public PacketReceipt(byte[] p, PolyNetPlayer s) {
			packet = p;
			sender = s;
		}
	}

}