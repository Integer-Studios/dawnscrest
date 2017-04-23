using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PolyNet {

	public class PolyNetChunk {

		private List<PolyNetIdentity> objects;
		private List<PolyNetPlayer> players;
		private ChunkIndex index;

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
			PacketHandler.queuePacket (new PacketObjectDespawn (i), rec);
			rec = newChunk.players.Except (players).ToArray ();
			PacketHandler.queuePacket (new PacketObjectSpawn (i), rec);
		}

		public void addPlayer(PolyNetPlayer i) {
			players.Add (i);
			PolyNetPlayer[] rec = new PolyNetPlayer[]{ i };
			foreach(PolyNetIdentity o in objects) {
				PacketHandler.queuePacket (new PacketObjectSpawn (o), rec);
			}
		}

		public void removePlayer(PolyNetPlayer i) {
			players.Remove (i);
			PolyNetPlayer[] rec = new PolyNetPlayer[]{ i };
			foreach(PolyNetIdentity o in objects) {
				PacketHandler.queuePacket (new PacketObjectDespawn (o), rec);
			}
		}

		public void sendPacket(Packet p) {
			PacketHandler.queuePacket (p, players.ToArray());
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