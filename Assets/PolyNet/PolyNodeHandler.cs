using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace PolyNet {

	public class PolyNodeHandler {

		private static SocketIOComponent socket;
		private static PolyNetManager manager;

		public static void initialize(PolyNetManager m) {
			manager = m;
			socket = m.GetComponent<SocketIOComponent> ();

			switch (manager.port) {
			case 0:
				socket.url = "ws://server.integerstudios.com:4201/socket.io/?EIO=4&transport=websocket";
					break;
			case 1:
				socket.url = "ws://server.integerstudios.com:4203/socket.io/?EIO=4&transport=websocket";
				break;
			case 2:
				socket.url = "ws://server.integerstudios.com:4205/socket.io/?EIO=4&transport=websocket";
				break;
			}
			Debug.Log ("Connecting to Node Server...");
			socket.Connect ();
			manager.StartCoroutine (Connect ());
		}

		public static IEnumerator Connect() {
			yield return new WaitForSeconds (1f);
			if (!socket.IsConnected) {
				Debug.Log ("Failed to Connect to Node."); 
				Application.Quit ();
			} else {
				socket.On ("playerLogin", receivePlayerLogin);
				socket.On ("disconnect", receiveDisconnect);
			}
		}

		public static void sendPlayerLogin(PolyNetPlayer player) {
			//player.playerId is working now so you send that to node
			//this will send to node
			JSONObject playerJSON = new JSONObject (JSONObject.Type.OBJECT);
			playerJSON.AddField ("id", player.playerId);

			emit ("playerLogin", playerJSON);
		}

		public static void receivePlayerLogin(SocketIOEvent e) {
			int playerId = (int)e.data.GetField ("id").n;
			Debug.Log (playerId);
			//this is where node gets back to us with the data
			JSONObject playerObjectData = e.data.GetField("object");
			PolyNetPlayer player = PolyServer.getPlayerPId(playerId);
			//eventually any player data that node wants to set can get thrown in here
			player.setData(new Vector3(0,0,0));
	//		player.setData (readVector(playerObjectData, "position"));
			PolyServer.onLoginData(PolyServer.getPlayerPId(playerId));
		}

		public static void emit(string identifier, JSONObject data) {
			if (socket.IsConnected) {
				socket.Emit (identifier, data);
			} else {
				Debug.Log ("Failed Node Emit");
			}
		}

		public static void receiveDisconnect(SocketIOEvent e) {
			socket.Close ();
		}

		public static Vector3 readVector(JSONObject json, string identifier) {

			return new Vector3(json.GetField(identifier + "-x").n, json.GetField(identifier + "-y").n, json.GetField(identifier + "-z").n);

		}

	}


}