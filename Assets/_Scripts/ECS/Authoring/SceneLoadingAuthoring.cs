using System.Collections;
using System.Collections.Generic;
using Unity.Entities.Serialization;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Unity.Collections;
using Unity.Scenes;

public class SceneLoadingAuthoring : MonoBehaviour {
#if UNITY_EDITOR
    public SceneAsset Scene;
    public GameObject SpawnerPrefab;

    class Baker : Baker<SceneLoadingAuthoring> {
        public override void Bake(SceneLoadingAuthoring authoring) {
            // We want to create a dependency to the scene in case the scene gets deleted.
            // This needs to be outside the null check below in case the asset file gets deleted and then restored.
            DependsOn(authoring.Scene);

            if (authoring.Scene != null) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SceneReference {
                    // Bake a reference to the scene, to be used at runtime to load the scene
                    Value = new EntitySceneReference(authoring.Scene),
                    SpawnerEntity = GetEntity(authoring.SpawnerPrefab, TransformUsageFlags.None)
                });
            }
        }
    }
#endif
}

// Triggers the load of the referenced scene
public struct SceneReference : IComponentData {
    // Reference to the scene to load
    public EntitySceneReference Value;
    public Entity SpawnerEntity;
}
