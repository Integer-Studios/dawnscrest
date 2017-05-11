using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class Packet {
		
		public int id;
		public int size;

		public Packet() {
			id = -1;
			size = 512;
		}

		public virtual int getSize() {
			return size;
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
			case 13:
				return new PacketSyncFloat ();
			case 14:
				return new PacketSlotUpdate ();
			case 15:
				return new PacketPlayerSetSlot ();
			case 16:
				return new PacketSyncInt ();
			case 17:
				return new PacketHotbarSwitch ();
			case 18:
				return new PacketOpenInventory ();
			case 19:
				return new PacketItemStackArray ();
			case 20:
				return new PacketRecipe ();
			case 21:
				return new PacketSetCraftableInput ();
			case 22:
				return new PacketSetCraftableRecipe ();
			case 23:
				return new PacketItemHeld ();
			default:
				return null;
			}
		}

	}

}