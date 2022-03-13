using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
[Serializable, GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    public byte textureID;
    public sbyte rotationIndex;
    public int lines;
    public int minoIndex, minos, holdMinoIndex, holdMinos;
    public bool isControllable, pieceSpawned, touchedGround, isHoldExecuted;
    public float lockDelay, lockTicks, delayedAutoShift, autoShiftTicks, autoShiftRate, softDropMultiplier, spawnDelay, spawnTicks, lineSpawnDelay, lineDropDelay, lineDropTicks, gravity, fallenTiles;
    public bool4x2 inputs;
    public int2 piecePos, posToMove;
    public float2 movement, prevMovement;
    public float shiftPos;
    public Unity.Mathematics.Random random;
}

//a bit oddball.
public struct PieceBlob
{
    public BlobArray<int2> array;
}
