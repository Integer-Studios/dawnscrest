using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyEntity {

	public class Action {
		
		public delegate void OnCompleteDelegate(Action a);

		public Entity owner;
		public OnCompleteDelegate onComplete;
		public Action chained;

		/*
		 * 
		 * Public Interface
		 * 
		 */ 

		public Action(Entity o, OnCompleteDelegate oc) {
			owner = o;
			onComplete = oc;
		}

		public virtual void Update () {
			kill();
		}
			
		public virtual void kill() {
			owner.stopAction (this);

			if (chained != null)
				owner.startAction (chained);
			
			if (onComplete != null)
				onComplete(this);
			
		}


		public virtual Action chain (Action a) {
			chained = a;
			return this;
		}

		/*
		 * 
		 * Private
		 * 
		 */ 


	}
}