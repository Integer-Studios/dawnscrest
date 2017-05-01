using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PolyNet {

	public class PolyNetChunk {

		private List<PolyNetIdentity> objects;
		private List<PolyNetPlayer> players;
		public ChunkIndex index;

		public PolyNetChunk(ChunkIndex i) {
			index = i;
			objects = new List<PolyNetIdentity> ();
			players = new List<PolyNetPlayer> ();
		}

		public void spawnObject(PolyNetIdentity i) {
			addObject (i);
			sendPacket (new PacketObjectSpawn (i));
		}

		public void despawnObject(PolyNetIdentity i) {
			removeObject (i);
			sendPacket (new PacketObjectDespawn (i));
		}

		public void migrateChunk(PolyNetIdentity i) {
			PolyNetChunk newChunk = PolyNetWorld.getChunk(i.transform.position);
			removeObject (i);
			newChunk.addObject (i);
			PolyNetPlayer[] rec = players.Except (newChunk.players).ToArray ();
			PacketHandler.sendPacket (new PacketObjectDespawn (i), rec);
			rec = newChunk.players.Except (players).ToArray ();
			PacketHandler.sendPacket (new PacketObjectSpawn (i), rec);
		}

		public void addPlayer(PolyNetPlayer i) {
			players.Add (i);
			PolyNetPlayer[] rec = new PolyNetPlayer[]{ i };
			foreach(PolyNetIdentity o in objects) {
				PacketHandler.sendPacket (new PacketObjectSpawn (o), rec);
			}
		}

		public void removePlayer(PolyNetPlayer i) {
			players.Remove (i);
			PolyNetPlayer[] rec = new PolyNetPlayer[]{ i };
			foreach(PolyNetIdentity o in objects) {
				PacketHandler.sendPacket (new PacketObjectDespawn (o), rec);
			}
		}

		public void sendPacket(Packet p) {
			PacketHandler.sendPacket (p, players.ToArray());
		}

		public bool inChunk(Vector3 position) {
			return (PolyNetWorld.getChunkIndex (position).x == index.x && PolyNetWorld.getChunkIndex (position).z == index.z);
		}


		private void addObject(PolyNetIdentity i) {
			objects.Add (i);
			i.setChunk(this);
		}

		private void removeObject(PolyNetIdentity i) {
			objects.Remove (i);
			i.setChunk(null);
		}

	}

}