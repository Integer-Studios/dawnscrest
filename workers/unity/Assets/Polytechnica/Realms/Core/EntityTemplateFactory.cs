using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using UnityEngine;

namespace Polytechnica.Realms.Core {
    public static class EntityTemplateFactory {

		public static Entity CreateCubeTemplate(string clientId) {
			var template = new Entity();
			template.Add(new StaticTransform.Data(Coordinates.ZERO, new Vector3d(0,0,0), new Vector3d(0,0,0)));
			var acl = Acl.GenerateClientAuthoritativeAcl (template, clientId);
			template.SetAcl(acl);
			return template;
		}

		public static SnapshotEntity CreateCube2Template(int x, int z) {
			var template = new SnapshotEntity { Prefab = "Cube2" };
			template.Add(new StaticTransform.Data(new Coordinates(x*50d, 0d, z*50d), new Vector3d(0,0,0), new Vector3d(0,0,0)));
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

		public static SnapshotEntity CreateLoginManagerTemplate() {
			var template = new SnapshotEntity { Prefab = "LoginManager" };
			template.Add(new StaticTransform.Data(Coordinates.ZERO, new Vector3d(0,0,0), new Vector3d(0,0,0)));
			template.Add(new PlayerSpawning.Data());
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}
    }
}
