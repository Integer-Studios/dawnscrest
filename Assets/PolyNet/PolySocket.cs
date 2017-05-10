using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PolyNet {

	public class PolySocket {

		public delegate void MessageHandler(byte[] b);
		public delegate void DisconnectHandler();

		private Socket socket;
		private MessageHandler handler;
		private DisconnectHandler onDisconnect;
		private Queue<byte[]> messages;
		private bool busy = false;
		private byte[] toSend;
		private bool receivingSize;
		private bool sendingSize;
		private bool isActive;

		private byte[] finalReceiveBuffer;
		private byte[] receiveBuffer;
		private int receiveBufferSize;
		private int currentReceived;
		private byte[] sendBuffer;
		private int sendBufferSize;
		private int currentSent;

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public PolySocket(Socket s, MessageHandler h, DisconnectHandler d) {
			socket = s;
			handler = h;
			onDisconnect = d;
			messages = new Queue<byte[]> ();
			isActive = false;
		}

		public void start() {
			isActive = true;
			setReceiveSize (4);
			receivingSize = true;
			socket.BeginReceive (receiveBuffer, 0, receiveBufferSize, SocketFlags.None, new AsyncCallback (receiveCallback), null);
		}

		public void stop () {
			isActive = false;
			socket.Shutdown (SocketShutdown.Both);
			socket.Close ();
			onDisconnect ();
		}

		public void queueMessage(byte[] b) {
			if (!isActive)
				return;
			if (!busy) {
				busy = true;
				send (b);
			} else {
				busy = true;
				lock (messages) {
					messages.Enqueue (b);
				}
			}
		}

		/*
		 * 
		 * Private
		 * 
		 */

		// Abstraction 2

		private void send(byte[] b) {
			if (!isActive)
				return;
			toSend = b;
			// send size packet
			sendingSize = true;
			setSend (BitConverter.GetBytes(toSend.Length));
			socket.BeginSend(sendBuffer, 0, sendBufferSize, SocketFlags.None, new AsyncCallback(sendCallback), null); 
		}

		private void onSendComplete() {
			if (!isActive)
				return;
			if (messages.Count > 0) {
				if (!socket.Poll (0, SelectMode.SelectWrite)) {
					Thread.Sleep (2);
					onSendComplete ();
				} else {
					lock (messages) {
						send (messages.Dequeue ());
					}
				}
			} else {
				busy = false;
			}
		}

		private void onReceiveComplete() {
			if (!isActive)
				return;
			byte[] data = new byte[finalReceiveBuffer.Length];
			Array.Copy (finalReceiveBuffer, data, finalReceiveBuffer.Length);
			handler(data);

			// begin listening for size again
			setReceiveSize (4);
			receivingSize = true;
			socket.BeginReceive (receiveBuffer, 0, receiveBufferSize, SocketFlags.None, new AsyncCallback (receiveCallback), null);
		}

		// Abstraction 1

		private void onReceived() {
			if (!isActive)
				return;
			// final receive buffer is filled
			if (receivingSize) {
				receivingSize = false;
				setReceiveSize (BitConverter.ToInt32 (finalReceiveBuffer, 0));
				socket.BeginReceive (receiveBuffer, 0, receiveBufferSize, SocketFlags.None, new AsyncCallback (receiveCallback), null);
			} else {
				onReceiveComplete ();
			}
		}

		private void onSent() {
			if (!isActive)
				return;
			// send buffer fully sent
			if (sendingSize) {
				// send data packet
				sendingSize = false;
				setSend (toSend);
				socket.BeginSend (sendBuffer, 0, sendBufferSize, SocketFlags.None, new AsyncCallback (sendCallback), null); 
			} else {
				onSendComplete ();
			}
		}

		// Low Level

		private void receiveCallback(IAsyncResult ar) {
			if (!isActive)
				return;
			try {
				int prev = currentReceived;
				currentReceived += socket.EndReceive (ar);
				Buffer.BlockCopy(receiveBuffer, 0, finalReceiveBuffer, prev, currentReceived-prev);
				if (currentReceived < receiveBufferSize) {
					socket.BeginReceive (receiveBuffer, 0, receiveBufferSize-currentReceived, SocketFlags.None, new AsyncCallback (receiveCallback), null);
				} else {
					onReceived ();
				}
			} catch (SocketException e) {
				if (isActive)
					stop ();
			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}

		private void sendCallback(IAsyncResult ar) {
			if (!isActive)
				return;
			try {
				currentSent += socket.EndSend (ar);
				if (currentSent < sendBufferSize) {
					socket.BeginSend(sendBuffer, currentSent, sendBufferSize-currentSent, SocketFlags.None, new AsyncCallback(sendCallback), null);
				} else {
					onSent ();
				}
			} catch (SocketException e) {
				if (isActive)
					stop ();
			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}

		private void setReceiveSize(int i) {
			receiveBufferSize = i;
			currentReceived = 0;
			receiveBuffer = new byte[receiveBufferSize];
			finalReceiveBuffer = new byte[receiveBufferSize];
		}

		private void setSend(byte[] b) {
			sendBufferSize = b.Length;
			sendBuffer = b;
		}

	}


}