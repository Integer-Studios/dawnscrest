using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

namespace PolyNet {

	public class PolyNodeHandler {

		private static SocketIOComponent socket;
		private static PolyNetManager manager;

		// delegates
		public delegate void NodeRequestHandler(JSONObject obj);
		private static Dictionary<string, NodeRequestHandler> handlers = new Dictionary<string, NodeRequestHandler>();
		private static PolyNetManager.StartSequenceDelegate onConnect;
		public static int startSequenceId;

		public static void initialize(PolyNetManager m, PolyNetManager.StartSequenceDelegate del, int id) {
			onConnect = del;
			startSequenceId = id;
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
			socket.Connect ();
			manager.StartCoroutine (Connect ());
		}

		/*
		 * 
		 * SocketIO
		 * 
		 */

		private static IEnumerator Connect() {
			yield return new WaitForSeconds (1f);
			if (!socket.IsConnected) {
				Debug.Log ("Failed to Connect to Node."); 
				Application.Quit ();
			} else {
				socket.On ("playerLogin", onReceive);
				socket.On ("heightmap", onReceive);
				socket.On ("objects", onReceive);

				socket.On ("disconnect", onReceiveDisconnect);

				Debug.Log ("Startup[" + startSequenceId + "]: Connected to Node.");
				onConnect (startSequenceId);
			}
		}
	
		private static void emit(string identifier, JSONObject data) {
			if (socket.IsConnected) {
				socket.Emit (identifier, data);
			} else {
				Debug.Log ("Failed Node Emit");
			}
		}


		/*
		 * 
		 * Public Requests
		 * 
		 */

		public static void sendRequest(string request, JSONObject data, NodeRequestHandler handler) {
			handlers.Add (request, handler);
			emit (request, data);
		}

		/*
		 * 
		 * Handlers
		 * 
		 */
			
		private static void onReceive(SocketIOEvent e) {
			NodeRequestHandler h;
			if (handlers.TryGetValue(e.name, out h)) {
				h(e.data);
				handlers.Remove(e.name);
			}
		}

		private static void onReceiveDisconnect(SocketIOEvent e) {
			socket.Close ();
		}

		/*
		 * 
		 * Private Helpers
		 * 
		 */

		private static Vector3 readVector(JSONObject json, string identifier) {
			return new Vector3(json.GetField(identifier + "-x").n, json.GetField(identifier + "-y").n, json.GetField(identifier + "-z").n);
		}

	}

}