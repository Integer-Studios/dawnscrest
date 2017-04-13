using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyNetwork {

	public interface IPolyPlayer {

		void polyPlayer_receiveChat(string name, string message, float distance);
		void polyPlayer_sendPlayerData(int id);
		void polyPlayer_setPlayerID(int id);

	}

}