using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PolyNetIdentity : PolyNetBehaviour {

		public int prefabId;
		public bool isStatic = true;
		public bool isLocalPlayer = false;
		public bool isSaveable = true;

		private PolyNetChunk chunk;
		private PolyNetPlayer owner;
		private int instanceId = -1;
		private Dictionary<int, PolyNetBehaviour> behaviours;

		public void initialize (int i) {
			instanceId = i;
		}

		public void Awake() {
			behaviours = new Dictionary<int,PolyNetBehaviour> ();
			int nextId = 0;
			foreach (PolyNetBehaviour b in GetComponents<PolyNetBehaviour>()) {
				b.setScriptId(nextId);
				b.setIdentity(this);
				behaviours.Add (nextId, b);
				nextId++;
			}
		}

		private void Start() {
			if (!isStatic)
				StartCoroutine (updateChunk());
		}

		public void routeBehaviourPacket(PacketBehaviour p) {
			PolyNetBehaviour b;
			if (behaviours.TryGetValue (p.scriptId, out b))
				b.handleBehaviourPacket (p);
			else
				Debug.Log ("Invalid script id on behaviour packet: " + p.scriptId + ". Ignoring packet.");
		}

		public void sendBehaviourPacket(PacketBehaviour p) {
			if (PolyServer.isActive)
				chunk.sendPacket (p);
			else
				PacketHandler.sendPacket (p, null);
		}

		public int getSpawnSize() {
			PolyNetBehaviour b;
			int i = 0;
			int size = 0;
			while (behaviours.TryGetValue (i, out b)) {
				size += b.getBehaviourSpawnSize ();
				i++;
			}
			//give it some room to write the transform
			return Mathf.Max(512,size);
		}

		public void writeSpawnData(ref BinaryWriter writer) {
			PolyNetBehaviour b;
			int i = 0;
			while (behaviours.TryGetValue (i, out b)) {
				b.writeBehaviourSpawnData (ref writer);
				i++;
			}
		}

		public void readSpawnData(ref BinaryReader reader) {
			PolyNetBehaviour b;
			int i = 0;
			while (behaviours.TryGetValue (i, out b)) {
				b.readBehaviourSpawnData (ref reader);
				i++;
			}
		}

		public JSONObject writeSaveData() {
			JSONObject obj = new JSONObject (JSONObject.Type.OBJECT);
			obj.AddField ("id", instanceId);
			obj.AddField ("prefab", prefabId);
			JSONHelper.wrap (obj, transform.position, "position");
			JSONHelper.wrap (obj, transform.eulerAngles, "rotation");
			JSONHelper.wrap (obj, transform.localScale, "scale");
			JSONObject scripts = new JSONObject (JSONObject.Type.ARRAY);
			foreach (PolyNetBehaviour b in behaviours.Values) {
				scripts.Add(b.writeBehaviourSaveData ());
			}
			obj.AddField ("scripts", scripts);
			return obj;
		}

		public void readSaveData(JSONObject data) {
			transform.eulerAngles = JSONHelper.unwrap (data, "rotation");
			transform.localScale = JSONHelper.unwrap (data, "scale");
			JSONObject scripts = data.GetField ("scripts");
			foreach(JSONObject s in scripts.list) {
				int scriptId = (int)s.GetField ("id").n;
				behaviours [scriptId].readBehaviourSaveData ( s);
			}
		}

		public int getOwnerId() {
			if (owner != null)
				return owner.playerId;
			else
				return -1;
		}

		public void setOwner(PolyNetPlayer o) {
			owner = o;
		}

		public override int getInstanceId() {
			return instanceId;
		}
			
		public void setChunk(PolyNetChunk c) {
			chunk = c;
		}

		private IEnumerator updateChunk() {
			yield return new WaitForSeconds (10f);
			while (PolyServer.isActive) {
				if (owner != null)
					owner.position = transform.position;

				if (!chunk.inChunk (transform.position)) {
					chunk.migrateChunk (this);
					if (owner != null)
						owner.refreshLoadedChunks ();
				}
				yield return new WaitForSeconds (1f);
			}
		}

	}


}