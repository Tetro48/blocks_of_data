using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    public int textureID;
    public int lines, frames;
    public bool isControllable;
    public float LockDelay, LockDelayf, DAS, SDF, ARE, AREf, AREline, lineDelay, gravity, fallenTiles;
    public int2 posToMove;
    public int lineDelayf;
}
