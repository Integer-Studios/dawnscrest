using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {

	public class ItemConsumable : Item {
		public float nutrition;
		public ConsumableType consumableType = ConsumableType.None;
	}

}