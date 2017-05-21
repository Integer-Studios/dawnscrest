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

	public class CharacterController : MonoBehaviour {

		[Require] private WorldTransform.Reader reader;
		[Require] private Character.Writer CharacterWriter;
		[Require] private EntityAcl.Writer AclWriter;

		private PlayerOnline PlayerOnline;
		private PlayerVisualizer PlayerVisualizer;

		private void OnEnable () {
			CharacterWriter.CommandReceiver.OnEmbody.RegisterResponse (OnEmbody);
			PlayerOnline = GetComponent<PlayerOnline> ();
			PlayerVisualizer = GetComponent<PlayerVisualizer> ();

			// Initialize to NPC ACL
			PlayerOnline.enabled = false;
			PlayerVisualizer.enabled = false;
		}

		private void OnDisable () {
			CharacterWriter.CommandReceiver.OnEmbody.DeregisterResponse();
		}


		public void setToPlayer(string workerId) {
			Map<uint, WorkerRequirementSet> write = new Map<uint, WorkerRequirementSet>();
			write.Add (WorldTransform.ComponentId, CommonRequirementSets.SpecificClientOnly (workerId));
			write.Add (DynamicTransform.ComponentId, CommonRequirementSets.SpecificClientOnly (workerId));
			write.Add (PlayerAnim.ComponentId, CommonRequirementSets.SpecificClientOnly (workerId));
			write.Add (Character.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (EntityAcl.ComponentId, CommonRequirementSets.PhysicsOnly);

			ComponentAcl acl = new ComponentAcl (write);
			Debug.Log ("Setting Character to Player ACL Configuration...");
			AclWriter.Send (new EntityAcl.Update ()
				.SetComponentAcl (acl)
			);

			PlayerOnline.enabled = true;
			PlayerVisualizer.enabled = true;
		}

		public void setToNPC() {
			Map<uint, WorkerRequirementSet> write = new Map<uint, WorkerRequirementSet>();
			write.Add (WorldTransform.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (DynamicTransform.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (PlayerAnim.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (Character.ComponentId, CommonRequirementSets.PhysicsOnly);
			write.Add (EntityAcl.ComponentId, CommonRequirementSets.PhysicsOnly);

			ComponentAcl acl = new ComponentAcl (write);
			Debug.Log ("Setting Character to NPC ACL Configuration...");
			AclWriter.Send (new EntityAcl.Update ()
				.SetComponentAcl (acl)
			);

			PlayerOnline.enabled = false;
			PlayerVisualizer.enabled = true;
		}

		private Nothing OnEmbody(Nothing n, ICommandCallerInfo callerInfo) {
			Debug.Log ("Embody request with worker ID:" + callerInfo.CallerWorkerId);
			setToPlayer (callerInfo.CallerWorkerId);
			return new Nothing ();
		}

	}

}
