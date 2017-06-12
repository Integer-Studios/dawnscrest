using System.Collections;
using System.Collections.Generic;
using Improbable;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Improbable.Entity.Component;
using Improbable.Unity.Core;
using Improbable.Worker;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Item {

	public class Item : Interactable {

		public int id;
		public Sprite sprite;
		public int weight = 1;
		public int maxStackSize = 1;
		public ItemType type = ItemType.Default;
		public int quality = 0;
		public GameObject onPlaced;

		protected override void OnComplete(EntityId i) {
			SpatialOS.Commands.SendCommand (
				interactableWriter, 
				InventoryComponent.Commands.Give.Descriptor, 
				new ItemStackData (id,quality,1),
				i
			).OnSuccess(OnGiveResponse);
		}

		private void OnGiveResponse(GiveResponse res) {
			if (res.success)
				SpatialOS.Commands.DeleteEntity (interactableWriter, gameObject.EntityId ());
//			else
//				SetMaxStrength (0);
		}

	}

	public enum ItemType {
		Default,
		Edible,
		Wearable,
		Placeable
	}

}