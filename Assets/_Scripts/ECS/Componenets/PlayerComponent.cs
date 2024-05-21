using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct PlayerComponent : IComponentData {
    public int PlayerTeam;
    public Entity ThisEntity;
    public Entity PlayerTarget;
    public LocalTransform PlayerTargetTransform;
}
