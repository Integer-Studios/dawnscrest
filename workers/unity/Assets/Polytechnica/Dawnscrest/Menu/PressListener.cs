using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Polytechnica.Dawnscrest.Menu {

	public class PressListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		public int id;
		public Menu m;

		public void OnPointerDown(PointerEventData eventData)
		{
			m.OnPointerDown (eventData, id);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			m.OnPointerUp (eventData, id);
		}


	}

}