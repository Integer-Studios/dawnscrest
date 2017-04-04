using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyNetwork {

	public interface IPolyPlayer {

		void receiveChat(string name, string message, float distance);

	}

}