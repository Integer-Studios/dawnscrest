using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using UnityEngine;
using Improbable.User;

namespace Assets.Gamelogic.EntityTemplates
{
    public static class EntityTemplateFactory
    {
        // Add methods to define entity templates
		public static Entity CreateCubeTemplate(string clientId)
		{
			var template = new Entity();

			template.Add(new WorldTransform.Data(Coordinates.ZERO));

			var acl = Acl.GenerateClientAuthoritativeAcl (template, clientId);
			template.SetAcl(acl);

			return template;
		}

		// Add methods to define entity templates
		public static SnapshotEntity CreateCube2Template(int x, int z)
		{
			var template = new SnapshotEntity { Prefab = "Cube2" };
			template.Add(new WorldTransform.Data(new Coordinates(x*50d, 0d, z*50d)));
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);

			return template;
		}

		public static SnapshotEntity CreateLoginManagerTemplate()
		{
			var template = new SnapshotEntity { Prefab = "LoginManager" };

			template.Add(new WorldTransform.Data(Coordinates.ZERO));
			template.Add(new PlayerSpawning.Data());

			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);

			return template;
		}
    }
}
