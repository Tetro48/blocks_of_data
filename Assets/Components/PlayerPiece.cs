using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, InternalBufferCapacity(4), GenerateAuthoringComponent]
public struct PlayerPiece : IBufferElementData
{
    public int2 value;

    public PlayerPiece(int x, int y) => value = new int2(x, y);
    public PlayerPiece(int2 input) => value = input;
    public static PlayerPiece operator +(PlayerPiece left, int2 right)
    {
        return new PlayerPiece(left.value += right);
    }
}
