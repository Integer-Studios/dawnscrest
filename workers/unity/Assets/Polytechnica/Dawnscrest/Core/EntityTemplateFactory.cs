using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using Improbable;
using UnityEngine;
using Polytechnica.Dawnscrest.Player;
using Polytechnica.Dawnscrest.World;
using Polytechnica.Dawnscrest.Item;
using Improbable.Collections;

namespace Polytechnica.Dawnscrest.Core {

	[System.Serializable]
	public enum EntityTemplateType {
		Basic,
		Interactable,
	}
	
    public static class EntityTemplateFactory {

		public static SnapshotEntity CreateEntityTemplate(WorldObject w) {
			switch (w.type) {
			case EntityTemplateType.Basic:
				return CreateBasicEntityTemplate (w.name, w.position, w.rotation, w.scale);
			case EntityTemplateType.Interactable:
				return CreateInteractableEntityTemplate (w.name, w.position, w.rotation, w.scale);
			default:
				return CreateBasicEntityTemplate (w.name, w.position, w.rotation, w.scale);
			}

		}

		public static SnapshotEntity CreateBasicEntityTemplate(string name, Vector3 pos, Vector3 rot, Vector3 scale) {
			var template = new SnapshotEntity { Prefab = name };
			template.Add(new WorldTransform.Data(new Coordinates (pos.x, pos.y, pos.z), new Vector3d(rot.x,rot.y,rot.z), new Vector3d(scale.x,scale.y,scale.z)));
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

		public static SnapshotEntity CreateInteractableEntityTemplate(string name, Vector3 pos, Vector3 rot, Vector3 scale) {
			var template = new SnapshotEntity { Prefab = name };
			template.Add(new WorldTransform.Data(new Coordinates (pos.x, pos.y, pos.z), new Vector3d(rot.x,rot.y,rot.z), new Vector3d(scale.x,scale.y,scale.z)));
			template.Add(new InteractableComponent.Data(10,10));
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

		public static Entity CreateCharacterTemplate(int houseId, bool active, AppearanceSet a) {
			var template = new Entity();
			Coordinates c = new Coordinates (Random.Range (-10f, 10f), 200f, Random.Range (-10f, 10f));
			template.Add(new WorldTransform.Data(c, new Vector3d(0,0,0), new Vector3d(1,1,1)));
			template.Add(new DynamicTransform.Data(new Vector3d(0,0,0), 0f));
			template.Add (new PlayerAnim.Data (false, 0, false, false));
			template.Add (new Character.Data ((uint)houseId, active));
			template.Add (new CharacterVitals.Data (100f,100f,100f,100f,100f,100f));
			template.Add (new CharacterAppearance.Data (a.sex,(uint)a.hairColor,(uint)a.eyeColor,(uint)a.build,(uint)a.hair,(uint)a.eyebrows,(uint)a.facialHair));
			var acl = Acl.Build ()
				.SetReadAccess (CommonRequirementSets.PhysicsOrVisual)
				.SetWriteAccess<WorldTransform> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<DynamicTransform> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<PlayerAnim> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<Character> (CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<CharacterVitals>(CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<CharacterAppearance>(CommonRequirementSets.PhysicsOnly)
				.SetWriteAccess<EntityAcl>(CommonRequirementSets.PhysicsOnly);
			template.SetAcl(acl);
			return template;
		}

		public static SnapshotEntity CreateCharacterCreatorTemplate() {
			var template = new SnapshotEntity { Prefab = "CharacterCreator" };
			template.Add(new WorldTransform.Data(Coordinates.ZERO, new Vector3d(0,0,0), new Vector3d(0,0,0)));
			template.Add (new CharacterCreatorController.Data ());
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

		public static SnapshotEntity CreateWorldTimeTemplate() {
			var template = new SnapshotEntity { Prefab = "WorldTime" };
			template.Add(new WorldTransform.Data(Coordinates.ZERO, new Vector3d(0,0,0), new Vector3d(0,0,0)));
			template.Add (new TimeComponent.Data (0));
			template.Add (new GloballyVisibile.Data ());
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

		public static SnapshotEntity CreateTerrainChunkTemplate(int x, int z) {
			Coordinates c = Coordinates.ZERO;
			c.X = WorldTerrain.chunkSize * x - (WorldTerrain.size/2);
			c.Z = WorldTerrain.chunkSize * z - (WorldTerrain.size/2);
			var template = new SnapshotEntity { Prefab = "TerrainChunk" };
			template.Add(new WorldTransform.Data(c, new Vector3d(0,0,0), new Vector3d(1,1,1)));
			template.Add (WorldTerrain.GetTerrainData(x,z));
			var acl = Acl.GenerateServerAuthoritativeAcl(template);
			template.SetAcl(acl);
			return template;
		}

    }
}
