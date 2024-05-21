using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RandomAuthoring : MonoBehaviour {
    public uint InitialIndex => (uint) new System.Random().Next(0, Int32.MaxValue);
}

public class RandomBaker : Baker<RandomAuthoring> {
    public override void Bake(RandomAuthoring authoring) {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new RandomComponent{
            Instance = Unity.Mathematics.Random.CreateFromIndex(authoring.InitialIndex)
        });
    }
}
