using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    public byte textureID;
    public int lines, frames;
    public bool isControllable, pieceSpawned, touchedGround;
    public float LockDelay, LockTicks, delayedAutoShift, delayedAutoShift_L, delayedAutoShift_R, softDropMultiplier, spawnDelay, spawnTicks, lineSpawnDelay, lineDropDelay, lineDropTicks, gravity, fallenTiles;
    public bool4x2 inputs;
    public int2 posToMove;
    public float shiftPos;
    public Unity.Mathematics.Random random;
}
