using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyNet {

	public class PolyServer {

		public static bool isActive = false;
		public static Dictionary<int, PolyNetPlayer> players = new Dictionary<int, PolyNetPlayer> ();

		private static int port;
		private static Socket serverSocket;
		private static List<PolyServlet> unidentifiedClients = new List<PolyServlet>();

		/*
		 * 
		 * Public Socket System
		 * 
		 */

		public static void start (int sPort, string sAddress) {
			port = sPort;
			attemptListen (sAddress);
			isActive = true;
		}

		public static void stop () {
			foreach (PolyNetPlayer p in players.Values) {
				p.servlet.stop ();
			}
			serverSocket.Close ();
		}

		public static void sendMessage (byte[] b, PolyNetPlayer player) {
			player.servlet.send (b);
		}

		public static void onDisconnect(PolyNetPlayer p) {
			PolyNetWorld.removePlayer (p);
			players.Remove (p.playerId);
			Debug.Log ("Player Disconnected with player ID: " + p.playerId);
		}

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public static PolyNetPlayer getPlayer(int playerId) {
			PolyNetPlayer p;
			if (players.TryGetValue (playerId, out p))
				return p;
			else {
				Debug.Log ("Player not found with id: " + playerId);
				return null;
			}
		}

		public static void onLogin(PolyNetPlayer p) {
			players.Add (p.playerId, p);
			unidentifiedClients.Remove (p.servlet);
			PolyNodeHandler.sendPlayerLogin(p);
		}

		public static void onLoginData(PolyNetPlayer p) {
			PolyNetWorld.addPlayer (p);
		}

		/*
		 * 
		 * Sockets
		 * 
		 */

		private static void attemptListen(string sAddress) {
			try {
				serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				serverSocket.Bind(new IPEndPoint(IPAddress.Parse(sAddress), port));
				serverSocket.Listen (0);
				serverSocket.BeginAccept (new AsyncCallback (onConnection), null);
			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}

		public static void onConnection(IAsyncResult result) {
			Debug.Log ("Client connected");
			unidentifiedClients.Add(new PolyServlet (new PolyNetPlayer (), serverSocket.EndAccept (result)));
			serverSocket.BeginAccept (new AsyncCallback (onConnection), null);
		}
	}

}