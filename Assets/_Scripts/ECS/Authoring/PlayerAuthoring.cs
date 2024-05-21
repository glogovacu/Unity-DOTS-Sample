using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour {
    public int PlayerTeam;
    public Transform CurrentTransform;
}

public class PlayerBaker : Baker<PlayerAuthoring> {
    public override void Bake(PlayerAuthoring authoring) {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerComponent {
            PlayerTeam = authoring.PlayerTeam,
            PlayerTarget = Entity.Null,
            ThisEntity = entity,
        });
        
    }
}
