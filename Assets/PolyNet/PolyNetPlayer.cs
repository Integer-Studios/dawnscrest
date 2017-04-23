using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PolyNet {

	public class PolyNetPlayer {

		public int connectionId;
		public int playerId;
		public Vector3 position;
		public List<PolyNetChunk> loadedChunks;
		public PolyNetIdentity identity;

		public PolyNetPlayer(int i) {
			loadedChunks = new List<PolyNetChunk> ();
			connectionId = i;
		}

		public void setData(Vector3 p) {
			position = p;
		}

		public void refreshLoadedChunks() {
			List<PolyNetChunk> newChunks = PolyNetWorld.getLoadedChunks (position);
			List<PolyNetChunk> final = new List<PolyNetChunk>();
			foreach (PolyNetChunk c in loadedChunks) {
				if (!newChunks.Exists (j => j == c))
					c.removePlayer (this);
				else
					final.Add (c);
			}
			foreach (PolyNetChunk c in newChunks) {
				if (!loadedChunks.Exists (j => j == c)) {
					c.addPlayer (this);
					final.Add (c);
				}
			}
			loadedChunks = final;
		}

		public void unloadChunks() {
			foreach (PolyNetChunk c in loadedChunks) {
				c.removePlayer (this);
			}
		}
	}

}