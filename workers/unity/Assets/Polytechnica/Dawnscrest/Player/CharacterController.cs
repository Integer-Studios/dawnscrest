using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Unity.Visualizer;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable;
using Improbable.Math;
using Polytechnica.Dawnscrest.Core;
using Improbable.Worker.Query;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Worker;
using Improbable.Collections;
using Improbable.Unity.Core.Acls;

namespace Polytechnica.Dawnscrest.Player {

	/*
	 * This class handles switching between NPCs and Player control
	 * The two main functions are set to player and set to npc
	 * these flip the ACL and give access to remote clients or
	 * take the access back to the server side
	 */
	public class CharacterController : MonoBehaviour {

		[Require] private Character.Writer characterWriter;
		[Require] private EntityAcl.Writer aclWriter;

		private PlayerOnline playerOnline;
		private CharacterVisualizer characterVisualizer;

		private void OnEnable () {
			characterWriter.CommandReceiver.OnEmbody.RegisterResponse (OnEmbody);
			playerOnline = GetComponent<PlayerOnline> ();
			characterVisualizer = GetComponent<CharacterVisualizer> ();

			// Initialize to NPC ACL
			playerOnline.enabled = false;
			characterVisualizer.enabled = false;
		}

		private void OnDisable () {
			characterWriter.CommandReceiver.OnEmbody.DeregisterResponse();
		}

		/*
		 * Sets the character to a player auth mode
		 * This flips the acl to be client authoritative to
		 * whichever worker id is provided
		 * This is essentially the embodiement function
		 */
		public void setToPlayer(string workerId) {
			Map<uint, WorkerRequirementSet> write = new Map<uint, WorkerRequirementSet>();
			write.Add (WorldTransform.ComponentId, CommonRequirementSets.SpecificClientOnly (workerId));
			write.Add (DynamicTransform.ComponentId, CommonRequirementSets.SpecificClientOnly (workerId));
			write.Add (PlayerAnim.ComponentId, CommonRequirementSets.SpecificClientOnly (workerId));
			write.Add (Character.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (EntityAcl.ComponentId, CommonRequirementSets.PhysicsOnly);

			ComponentAcl acl = new ComponentAcl (write);
			Debug.Log ("Setting Character to Player ACL Configuration...");
			aclWriter.Send (new EntityAcl.Update ()
				.SetComponentAcl (acl)
			);

			playerOnline.enabled = true;

			// Visualizer is now needed server side because server isnt producing the values
			characterVisualizer.enabled = true;
		}

		/*
		 * This resets a character to NPC mode. Flips its ACL to server authoritative
		 * This is called when a player logs out or switched bodies
		 */
		public void setToNPC() {

			// Flip the ACL to server Auth
			Map<uint, WorkerRequirementSet> write = new Map<uint, WorkerRequirementSet>();
			write.Add (WorldTransform.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (DynamicTransform.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (PlayerAnim.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (Character.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (EntityAcl.ComponentId, CommonRequirementSets.PhysicsOnly);

			ComponentAcl acl = new ComponentAcl (write);
			Debug.Log ("Setting Character to NPC ACL Configuration...");
			aclWriter.Send (new EntityAcl.Update ()
				.SetComponentAcl (acl)
			);

			playerOnline.enabled = false;

			// Visualizer not needed server side because server is producing the written values
			characterVisualizer.enabled = false;
		}

		/*
		 * This is a command handler for an embodiment request. It is called by a client when login occurs
		 * And after that client has identiifed this haracter as its body. From here the character is set
		 * to player mode
		 */
		private Nothing OnEmbody(Nothing n, ICommandCallerInfo callerInfo) {
			Debug.Log ("Embody request with worker ID:" + callerInfo.CallerWorkerId);
			setToPlayer (callerInfo.CallerWorkerId);
			return new Nothing ();
		}

	}

}
