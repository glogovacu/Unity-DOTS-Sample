using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public struct RandomComponent : IComponentData {
    public Random Instance;
}
