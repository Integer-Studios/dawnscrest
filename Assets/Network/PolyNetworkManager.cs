using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using SocketIO;

public class PolyNetworkManager : NetworkManager {

	public bool live;
	public int debugUserID;
	public GameObject spawn;
	public bool shouldRip, shouldSave;

	public Dictionary<int, string> rippedPrefabs = new Dictionary<int, string>();
	public JSONObject rippedObjects;

	private static IPolyPlayer localPlayer;
	private PolyLogin login;
	private static PolyNetworkManager self; 
	private static List<PolyNetworkListener> listeners = new List<PolyNetworkListener>();
	private static List<PolyMessageHandler> handlers = new List<PolyMessageHandler> ();

	void OnApplicationQuit() {
		if (live && shouldSave) {
			PolyDataManager.savePlayers ();
			PolyDataManager.save ();
		}
	}

	void Start () {
		PolyNetworkManager.self = this;
		PolyDataManager.initialize(GetComponent<SocketIOComponent>());
		if (IsHeadless ()) {
			StartPolyServer ();
		} else {
			if (FindObjectOfType<PolyLogin> () != null) {
				login = FindObjectOfType<PolyLogin> ();
				if (login.debugHost) {
					login.playerID = debugUserID;
					this.networkAddress = "localhost";
					StartPolyHost ();
				} else if (login.debugClient) {
					login.playerID = debugUserID;
					this.networkAddress = "localhost";
					StartClient ();
				} else {
					this.networkAddress = "52.34.152.147";
					StartClient ();
				}
			} else {
				this.networkAddress = "localhost";
				StartPolyHost ();
			}
		}

		handlers.Add (new PolyChatManager ());

	}

	void Update () {

		PolyDataManager.checkPlayerQueue ();

	}

	/*
	 * 
	 * Public Interface
	 * 
	 */

	public static void FinishStart() {
		Debug.Log ("Finishing Start...");
		if (getManager ().IsHeadless ())
			self.StartServer ();
		else {
			if (self.login != null) {
				if (self.login.debugHost) {
					self.StartHost ();
				} else if (self.login.debugClient) {
					self.StartClient ();
				} else {
					self.StartClient ();
				}
			} else {
				self.StartHost ();
			}
		}
	}

	public static PolyClient FinishPlayerLogin(PolyClient player) {

		Debug.Log ("Finishing player... " + player.identifier);
		NetworkConnection conn = NetworkServer.connections[player.connectionID];
		var playerObject = (GameObject)GameObject.Instantiate(self.playerPrefab, self.spawn.transform.position, Quaternion.identity);
		NetworkServer.AddPlayerForConnection(conn, playerObject, player.controllerID);
		player.gameObject = playerObject;
		self.StartCoroutine (listeners_onPlayerLogin (player));
		return player;

	}

	public static PolyNetworkManager getManager() {
		return self;
	}

	public static void startListening(PolyNetworkListener l) {
		startListening (l, false);
	}

	public static void startListening(PolyNetworkListener l, bool fullRefreshOnLoad) {
		if (listeners == null)
			listeners = new List<PolyNetworkListener> ();
		listeners.Add (l);

		if (fullRefreshOnLoad) {
			foreach (PolyClient p in PolyDataManager.getActiveList()) {
				l.networkListener_onPlayerLogin(p);
			}
		}
	}

	public static void stopListening(PolyNetworkListener l) {
		listeners.Remove (l);
	}

	public static void sendNetMessage(NetworkConnection connection, short messageType, MessageBase msg) {
		connection.Send (messageType, msg);
	}

	public static void setLocalPlayer(IPolyPlayer p) {
		localPlayer = p;
	}

	public static IPolyPlayer getLocalPlayer() {
		return localPlayer;
	}

	/*
	 * 
	 * Public
	 * 
	 */

	public override NetworkClient StartHost() {
		
		NetworkClient client = base.StartHost ();
		return client;
	}

	public override void OnStartServer () {
		base.OnStartServer();
		NetworkServer.RegisterHandler(PlayerLogin.MSG_TYPE, OnPlayerLogin);
		foreach (PolyMessageHandler h in handlers) {
			h.registerServerHandlers ();
		}
	}

	public override void OnStopServer () {
		base.OnStopServer ();
		NetworkServer.UnregisterHandler(PlayerLogin.MSG_TYPE);
		foreach (PolyMessageHandler h in handlers) {
			h.unregisterServerHandlers ();
		}
	}

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		PolyDataManager.connectionQue (conn.connectionId, playerControllerId);
	}

	public override void OnServerDisconnect(NetworkConnection conn) 	{
		if (live)
			PolyDataManager.savePlayer (PolyDataManager.getPlayerIDForConnection(conn.connectionId));
		PolyDataManager.onPlayerDisconnect (conn.connectionId);
		NetworkServer.DestroyPlayersForConnection(conn);
	}

	public override void OnClientConnect(NetworkConnection conn) {
		base.OnClientConnect (conn);
		foreach (PolyMessageHandler h in handlers) {
			h.registerClientHandlers ();
		}

		if (conn != null) {
			Debug.Log ("Client Connected...");
			if (login != null) 
				conn.Send (PlayerLogin.MSG_TYPE, new PlayerLogin () { id = login.playerID});
			else 
				conn.Send (PlayerLogin.MSG_TYPE, new PlayerLogin () { id = debugUserID});
			
		}
	}
		
	public override void OnClientDisconnect(NetworkConnection conn) {
		foreach (PolyMessageHandler h in handlers) {
			h.unregisterClientHandlers ();
		}
	}

	/*
	 * 
	 * Private
	 * 
	 */

	private void StartPolyHost() {
		if (live) {
			Debug.Log ("Starting Poly Server...");
			PolyDataManager.connectData ();
		} else {
			StartHost ();
		}
	}

	private void StartPolyServer() {
		Debug.Log ("Starting Poly Server...");
		PolyDataManager.connectData ();
	}
		
	private void OnPlayerLogin(NetworkMessage netMsg) {
		PlayerLogin msg = netMsg.ReadMessage<PlayerLogin>();

		if (live)
			PolyDataManager.playerLogin (msg.id, netMsg.conn.connectionId);
		else {
			PolyClient c = new PolyClient (msg.id, netMsg.conn);
			c.data = generateOfflinePlayerData (msg.id);
			PolyDataManager.playerLoginOffline (c);
		}
	}

	private JSONObject generateOfflinePlayerData(int id) {
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField ("id", id);
		json.AddField ("position-x", spawn.transform.position.x);
		json.AddField ("position-y", spawn.transform.position.y);
		json.AddField ("position-z", spawn.transform.position.z);
		json.AddField ("rotation-x", 0);
		json.AddField ("rotation-y", 0);
		json.AddField ("rotation-z", 0);
		json.AddField ("health", 150f);
		json.AddField ("hunger", 150f);
		json.AddField ("thirst", 150f);

		return json;

	}

	private bool IsHeadless() {
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
	}

	private static IEnumerator listeners_onPlayerLogin(PolyClient player) {

		yield return new WaitForSeconds (0f);

		foreach (PolyNetworkListener l in listeners) {
			l.networkListener_onPlayerLogin (player);
		}
	}

	/*
	 * 
	 *  LLAPI Messages
	 * 
	 */

	public class PlayerLogin : MessageBase {
		public static short MSG_TYPE = 2000;
		public int id = 0;
	}

}
