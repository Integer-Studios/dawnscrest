using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyNet {

	public class PolyServer {

		public static bool isActive = false;

		private static int port;
		private static int reliableChannelId, socketId;
		private static Dictionary<int, PolyNetPlayer> players = new Dictionary<int, PolyNetPlayer> ();
		private static Dictionary<int, int> playerIdMap = new Dictionary<int, int> ();

		public static void start (int sPort) {
			port = sPort;
			NetworkTransport.Init();
			ConnectionConfig config = new ConnectionConfig();
			reliableChannelId  = config.AddChannel(QosType.Reliable);
			HostTopology topology = new HostTopology(config, 10);
			socketId = NetworkTransport.AddHost(topology, port);
			Debug.Log ("PolyNet Server Started on Port: "+ port +", socketId: " + socketId);
			isActive = true;
		}

		public static void update() {
			int recHostId, recConnectionId, recChannelId;
			byte[] recBuffer = new byte[1024];
			int bufferSize = 1024;
			int dataSize;
			byte error;
			NetworkEventType recNetworkEvent = NetworkTransport.Receive (out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
			switch (recNetworkEvent) {
			case NetworkEventType.Nothing:
				break;
			case NetworkEventType.ConnectEvent:
				onConnect (new PolyNetPlayer (recConnectionId));
				break;
			case NetworkEventType.DataEvent:
				onRecieveMessage (recBuffer, getPlayerCId(recConnectionId));
				break;
			case NetworkEventType.DisconnectEvent:
				onDisconnect (getPlayerCId(recConnectionId));
				break;
			}
		}

		public static void sendMessage(byte[] buffer, PolyNetPlayer player) {
			byte error; NetworkTransport.Send(socketId, player.connectionId, reliableChannelId, buffer, 1024, out error);
		}

		public static PolyNetPlayer getPlayerCId(int connectionId) {
			PolyNetPlayer player;
			if (players.TryGetValue (connectionId, out player))
				return player;
			else {
				playerNotFoundException (true, connectionId);
				return null;
			}
		}

		public static PolyNetPlayer getPlayerPId(int playerId) {
			int conId;
			if (playerIdMap.TryGetValue (playerId, out conId))
				return getPlayerCId (conId);
			else {
				playerNotFoundException (true, playerId);
				return null;
			}
		}

		public static void onLogin(PolyNetPlayer p) {
			playerIdMap.Add (p.playerId, p.connectionId);
			PolyNodeHandler.sendPlayerLogin(p);
		}

		public static void onLoginData(PolyNetPlayer p) {
			PolyNetWorld.addPlayer (p);
		}

		private static void onRecieveMessage(byte[] buffer, PolyNetPlayer player) {
			PacketHandler.handlePacket (buffer, player);
		}

		private static void onConnect(PolyNetPlayer p) {
			Debug.Log ("Player connected with connection ID: " + p.connectionId);
			players.Add (p.connectionId, p);
		}

		private static void onDisconnect(PolyNetPlayer p) {
			PolyNetWorld.removePlayer (p);
			players.Remove (p.connectionId);
			playerIdMap.Remove (p.playerId);
			Debug.Log ("Player Disconnected with connection ID: " + p.connectionId);
		}

		private static void playerNotFoundException(bool c, int id) {
			if (c)
				Debug.Log ("No Player found for connection ID: " + id + ".");
			else
				Debug.Log ("No Player found for player ID: " + id + ".");
		}
	}

}