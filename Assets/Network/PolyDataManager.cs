using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.Networking;
using SocketIO;

public class PolyDataManager {

	//Objects / Saving
	public static Dictionary<int, string> prefabs = new Dictionary<int, string>();
	public static Dictionary<int, Saveable> objects = new Dictionary<int, Saveable> ();

	private static SocketIOComponent socket;

	//Clients
	private static Dictionary<int, int> activeConnections = new Dictionary<int, int> ();
	private static Dictionary<int, short> queuedConnections = new Dictionary<int, short> ();
	private static Dictionary<int, PolyClient> activePlayers = new Dictionary<int, PolyClient> ();
	private static Dictionary<int, int> activeGameObjects = new Dictionary<int, int> ();

	private static Dictionary<int, PolyClient> queuedPlayers = new Dictionary<int, PolyClient> ();
	//	public static Dictionary<int, PolyClient> inactiveClients = new Dictionary<int, PolyClient> ();

	private static bool ripping = false;
	private static JSONObject rippedObjects;

	public static void initialize(SocketIOComponent socket) {
		PolyDataManager.socket = socket;
	}

	/*
	 * 
	 *  Public Interface
	 * 
	 */

	public static void connectData() {
		Debug.Log ("Connecting to Node Server...");
		socket.Connect ();

		PolyNetworkManager.getManager().StartCoroutine (Connect ());

	}

	public static void checkPlayerQueue() {
		int[] keys = new int[queuedConnections.Keys.Count];
		queuedConnections.Keys.CopyTo(keys, 0);
		foreach (int connectionID in keys) {
			if (queuedPlayers.ContainsKey (connectionID)) {
				PolyClient player = queuedPlayers [connectionID];
				player.connection = NetworkServer.connections [connectionID];
				player.controllerID = queuedConnections [connectionID];
				player = PolyNetworkManager.FinishPlayerLogin (player);
				activeGameObjects.Add (player.gameObject.GetInstanceID (), player.loginID);
				PlayerSaveable s = player.gameObject.GetComponent<PlayerSaveable> ();
				IPolyPlayer playerObj = player.gameObject.GetComponent<IPolyPlayer> ();
				player.playerObject = playerObj;

				queuedConnections.Remove (player.connectionID);
				queuedPlayers.Remove (player.connectionID);
				activePlayers.Add (player.loginID, player);
				activeConnections.Add (player.connectionID, player.loginID);

				s.id = player.loginID;
				s.read (player.data);

			}
		}
	}

	public static int getPlayerIDForConnection(int connectionID) {
		return activeConnections [connectionID];
	}

	public static int getPlayerIDForObject(int instanceID) {
		return activeGameObjects [instanceID];
	}

	public static PolyClient getPlayerForConnection(int connectionID) {
		return activePlayers [activeConnections [connectionID]];
	}

	public static PolyClient getPlayer(int playerID) {
		return activePlayers [playerID];
	}

	public static void connectionQue(int connectionID, short controllerID) {
		queuedConnections.Add (connectionID, controllerID);
	}

	public static void playerLogin(int playerID, int connectionID) {

		JSONObject playerJSON = new JSONObject (JSONObject.Type.OBJECT);
		playerJSON.AddField ("id", playerID);
		playerJSON.AddField ("connection", connectionID);
		socket.Emit ("playerLogin", playerJSON);

	}

	public static void playerLoginOffline(PolyClient player) {

		player.identifier = "TESTER_" + player.connectionID;
		queuedPlayers.Add (player.connectionID, player);

	}


	public static void onPlayerDisconnect(int connectionID) {

		int playerID = activeConnections [connectionID];
		savePlayer (playerID);
		activeGameObjects.Remove (activePlayers[playerID].gameObject.GetInstanceID());
		activePlayers.Remove (playerID);
		activeConnections.Remove (connectionID);
		JSONObject obj = new JSONObject (JSONObject.Type.OBJECT);
		obj.AddField ("id", playerID);
		socket.Emit("playerDisconnect", obj);
	}

	public static void DeleteObject(int id) {
		Saveable s = objects [id];
		DeleteObject (s);
	}

	public static void DeleteObject(GameObject obj) {
		Saveable s = obj.GetComponent<Saveable> ();
		if (s != null && PolyNetworkManager.getManager ().live) {
			DeleteObject (s);
		} else {
			NetworkServer.Destroy (obj);
		}
	}

	public static void DeleteObject(Saveable s) {
		objects.Remove (s.id);
		NetworkServer.Destroy (s.gameObject);
	}

	public static IEnumerator Connect() {
		yield return new WaitForSeconds (3f);
		if (!socket.IsConnected) {
			Debug.Log ("FAILED TO CONNECT"); 
		} else {
			socket.On ("playerLogin", onPlayerLogin);
			socket.On ("playerSave", onPlayerSave);

			socket.On ("setPrefabs", onSetPrefabs);
			socket.On ("setObjects", onSetObjects);

			socket.On ("loadPrefabs", onLoadPrefabs);
			socket.On ("loadObjects", onLoadObjects);

			PolyNetworkManager.FinishStart ();
			if (PolyNetworkManager.getManager ().shouldRip) {
				rip ();
				setPrefabs ();
			} else {
				if (PolyNetworkManager.getManager ().live) {
					reverseRip ();
					socket.Emit ("loadPrefabs");
				}
			}
			GameObject spawnObject = PolyNetworkManager.getManager ().spawn;
			JSONObject spawn = new JSONObject (JSONObject.Type.OBJECT);
			spawn.AddField ("x", spawnObject.transform.position.x);
			spawn.AddField ("y", spawnObject.transform.position.y);
			spawn.AddField ("z", spawnObject.transform.position.z);

			socket.Emit ("spawn", spawn);
		}
	}

	public static List<PolyClient> getActiveList() {
		List<PolyClient> items = new List<PolyClient>();
		items.AddRange(activePlayers.Values);
		return items;
	}

	public static void save() {
		Debug.Log ("Saving...");
	
		int[] keys = new int[objects.Keys.Count];
		objects.Keys.CopyTo(keys, 0);
		JSONObject savedObjects = new JSONObject (JSONObject.Type.ARRAY);
		foreach (int id in keys) {

			savedObjects.Add(objects[id].write());

		}

		setObjects (savedObjects);

	}

	public static void overwriteSave(int playerID) {
		PolyClient p = activePlayers [playerID];
		p.gameObject.transform.position = PolyNetworkManager.getManager ().spawn.transform.position;
		savePlayer (playerID);
	}

	public static void savePlayer(int playerID) {
		Debug.Log ("Saving Player...");

		PolyClient p = activePlayers [playerID];
		PlayerSaveable s = p.gameObject.GetComponent<PlayerSaveable> ();
		JSONObject data = s.write ();

		socket.Emit ("playerSave", data);
	}

	public static void savePlayers() {
		Debug.Log ("Saving Players...");
		int[] keys = new int[activePlayers.Keys.Count];
		activePlayers.Keys.CopyTo(keys, 0);
		foreach (int id in keys) {
			PolyClient p = activePlayers [id];
			PlayerSaveable s = p.gameObject.GetComponent<PlayerSaveable> ();
			JSONObject data = s.write ();
			socket.Emit ("playerSave", data);
		}
	}

	/* 
	 * 
	 * Private
	 * 
	 */

	private static void setPrefabs() {
		JSONObject prefabsJSON = new JSONObject (JSONObject.Type.ARRAY);

		foreach (int id in PolyDataManager.prefabs.Keys) {
			JSONObject prefabJSON = new JSONObject (JSONObject.Type.OBJECT);
			prefabJSON.AddField ("id", id);
			prefabJSON.AddField ("path", PolyDataManager.prefabs[id]);
			prefabsJSON.Add (prefabJSON);

		}
		socket.Emit ("setPrefabs", prefabsJSON);
	}

	private static void setObjects(JSONObject objects) {
		socket.Emit ("setObjects", objects);
	}

	private static void reverseRip() {

		foreach (GameObject g in PolyNetworkManager.FindObjectsOfType<GameObject>()) {
			Saveable saveable = g.GetComponent<Saveable> ();

			if (saveable == null)
				continue;

			PolyNetworkManager.DestroyImmediate (g);
		}

	}

	private static void rip() {
//		ripping = true;
//		rippedObjects = new JSONObject (JSONObject.Type.ARRAY);
//		prefabs = new Dictionary<int, string>();
//		Dictionary<string, int> prefabsInverse = new Dictionary<string, int>();
//		int persistentID = 0;
//		int prefabID = 0;
//		foreach (GameObject g in PolyNetworkManager.FindObjectsOfType<GameObject>()) {
//			Saveable saveable = g.GetComponent<Saveable> ();
//
//			if (saveable == null)
//				continue;
//
//			// get prefab
//			GameObject pre = PrefabUtility.GetPrefabParent (g) as GameObject;
//			string path = AssetDatabase.GetAssetPath(pre);
//
//			// get correct prefab ID, add new if necessary
//			int gPreindex = prefabID;
//			if (!prefabsInverse.TryGetValue (path, out gPreindex)) {
//
//				prefabs.Add (prefabID, path);
//				prefabsInverse.Add (path, prefabID);
//				gPreindex = prefabID;
//				prefabID++;
//			}
//			//on rip only because it will already have it on save
//			saveable.setID (persistentID);
//			saveable.setPrefab (gPreindex);
//			// saves object to saves
//			rippedObjects.Add(saveable.write());
//			//schedule gameobject save
//
//			// increment and destroy object
//			persistentID++;
//			PolyNetworkManager.DestroyImmediate (g);
//		}
	}

	private static JSONObject serializeGameobject(int persistentID, int prefabID, GameObject obj) {

		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
		json.AddField ("id", persistentID);
		json.AddField ("prefab", prefabID);
		json.AddField ("position-x", obj.transform.position.x);
		json.AddField ("position-y", obj.transform.position.y);
		json.AddField ("position-z", obj.transform.position.z);
		json.AddField ("rotation-x", obj.transform.eulerAngles.x);
		json.AddField ("rotation-y", obj.transform.eulerAngles.y);
		json.AddField ("rotation-z", obj.transform.eulerAngles.z);
		json.AddField ("scale-x", obj.transform.localScale.x);
		json.AddField ("scale-y", obj.transform.localScale.y);
		json.AddField ("scale-z", obj.transform.localScale.z);
		return json;
	}

	private static GameObject spawnSerializedGameobject(JSONObject json) {
		int persistentID = (int)json.GetField("id").n;
		int prefabID = (int)json.GetField("prefab").n;

		GameObject g = PolyNetworkManager.Instantiate(Resources.Load(convertPath(PolyDataManager.prefabs[prefabID]), typeof(GameObject))) as GameObject;
		g.GetComponent<Saveable> ().setID (persistentID);
		g.GetComponent<Saveable> ().setPrefab (prefabID);

		return g;
	}

	private static void load (JSONObject savedObjects) {

		foreach (JSONObject obj in savedObjects.list) {

			Saveable s = spawnSerializedGameobject (obj).GetComponent<Saveable> ();

			s.read (obj);

			objects.Add (s.id, s);

		}
	}

	private static string convertPath(string path) {
		int i1 = path.IndexOf ("Resources/");
		i1 += 10;
		int i2 = path.IndexOf (".");
		int length = i2 - i1;
		return path.Substring (i1, length);
	}

	/* 
	* 
	* Node Events
	* 
	*/

	private static void onSetPrefabs(SocketIOEvent e) {
		prefabs.Clear ();
		setObjects (rippedObjects);
	}

	private static void onSetObjects(SocketIOEvent e) {
		if (ripping)
			socket.Emit ("loadPrefabs");
		else { 

		}
	}

	private static void onLoadPrefabs(SocketIOEvent e) {
		foreach (string key in e.data.keys){
			JSONObject field = e.data.GetField (key);
			string path = field.GetField ("path").str;
			int id = (int)field.GetField ("id").n;
			prefabs.Add (id, path);
		}
		socket.Emit ("loadObjects");
	}

	private static void onLoadObjects(SocketIOEvent e) {
		load (e.data);
	}

	private static void onPlayerLogin(SocketIOEvent e) {
		int loginID = (int)e.data.GetField ("id").n;
		int connectionID = (int)e.data.GetField ("connection").n;

		PolyClient player = new PolyClient (loginID, connectionID, e.data);
		player.identifier = e.data.GetField ("username").str;
	
		queuedPlayers.Add (connectionID, player);
	}

	private static void onPlayerSave(SocketIOEvent e) {

	}

}
