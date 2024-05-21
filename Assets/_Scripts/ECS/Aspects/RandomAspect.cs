using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct RandomAspect : IAspect {

    private readonly RefRW<RandomComponent> _random;

    public float3 GetRandomSpawnPosition(float3 startingPosition) {
        uint index = _random.ValueRW.Instance.NextUInt(UInt32.MaxValue);
        _random.ValueRW.Instance = Unity.Mathematics.Random.CreateFromIndex(index);
        float randomPositionX = startingPosition.x + _random.ValueRW.Instance.NextFloat(-3, 3);
        float randomPositionZ = startingPosition.z + _random.ValueRW.Instance.NextFloat(-3, 3);
        return new float3(randomPositionX, 1, randomPositionZ);
    }
}
