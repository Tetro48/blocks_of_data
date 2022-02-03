using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    public NativeArray<int3> activePiece;
    public NativeArray<Byte> boardState;
    public int lines, frames;
    public bool isControllable;
    public float LockDelay, LockDelayf, DAS, SDF, ARE, AREf, AREline, lineDelay, gravity;
    public int lineDelayf;
}
