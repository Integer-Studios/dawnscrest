using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine;

namespace PolyNet {

	public class PolyServlet {

		private PolyNetPlayer player;
		private PolySocket socket;
		private int id;

		public PolyServlet(PolyNetPlayer p, Socket s) {
			player = p;
			player.servlet = this;
			socket = new PolySocket(s, handleMessage, onDisconnect);
			socket.start ();
		}

		public void stop () {
			socket.stop ();
		}

		// thread safe, queued
		public void send(byte[] b) {
			socket.queueMessage (b);
		}

		// not main thread
		private void handleMessage(byte[] b) {
			PacketHandler.receivePacket (b, player);
		}

		private void onDisconnect() {
			PolyServer.onDisconnect (player);
		}

	}

}