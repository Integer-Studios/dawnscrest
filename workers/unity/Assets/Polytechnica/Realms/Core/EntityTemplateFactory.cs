using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using UnityEngine;
using Polytechnica.Realms.Player;

namespace Polytechnica.Realms.Core {
	
    public static class EntityTemplateFactory {

		public static Entity CreatePlayerTemplate(string clientId) {
			var template = new Entity();
			template.Add(new WorldTransform.Data(Coordinates.ZERO, new Vector3d(0,0,0), new Vector3d(1,1,1)));
			template.Add(new DynamicTransform.Data(new Vector3d(0,0,0), 0f));
			template.Add (new PlayerAnim.Data (false, 0, false, false));
			var acl = Acl.Build ()
				.SetReadAccess (CommonRequirementSets.PhysicsOrVisual)
				.SetWriteAccess<WorldTransform> (CommonRequirementSets.SpecificClientOnly (clientId))
				.SetWriteAccess<DynamicTransform> (CommonRequirementSets.SpecificClientOnly (clientId))
				.SetWriteAccess<PlayerAnim> (CommonRequirementSets.SpecificClientOnly (clientId));
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

		public static SnapshotEntity CreateLoginManagerTemplate() {
			var template = new SnapshotEntity { Prefab = "LoginManager" };
			template.Add(new WorldTransform.Data(Coordinates.ZERO, new Vector3d(0,0,0), new Vector3d(0,0,0)));
			template.Add(new PlayerSpawning.Data());
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

    }
}
