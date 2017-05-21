using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using Improbable;
using UnityEngine;
using Polytechnica.Realms.Player;

namespace Polytechnica.Realms.Core {
	
    public static class EntityTemplateFactory {

		public static SnapshotEntity CreateCharacterTemplate() {
			var template = new SnapshotEntity { Prefab = "Player" };
			template.Add(new WorldTransform.Data(Coordinates.ZERO, new Vector3d(0,0,0), new Vector3d(1,1,1)));
			template.Add(new DynamicTransform.Data(new Vector3d(0,0,0), 0f));
			template.Add (new PlayerAnim.Data (false, 0, false, false));
			template.Add (new Character.Data (1, true));
			var acl = Acl.Build ()
				.SetReadAccess (CommonRequirementSets.PhysicsOrVisual)
				.SetWriteAccess<WorldTransform> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<DynamicTransform> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<PlayerAnim> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<Character> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<EntityAcl>(CommonRequirementSets.PhysicsOnly);
			template.SetAcl(acl);
			return template;
		}

		public static SnapshotEntity CreateGroundTemplate() {
			var template = new SnapshotEntity { Prefab = "Ground" };
			template.Add(new WorldTransform.Data(new Coordinates(0d, -1d, 0d), new Vector3d(0,0,0), new Vector3d(100,1,100)));
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

    }
}
