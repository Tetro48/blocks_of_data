using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, InternalBufferCapacity(13), GenerateAuthoringComponent]
//previously known as PlayerPiece is now reused as bag.
public struct PlayerBag : IBufferElementData
{
    public byte2 value;

    public static implicit operator PlayerBag(byte2 data) => new PlayerBag {value = data};
    public static implicit operator byte(PlayerBag bag) => bag.value.x;
}

public struct byte2
{
    public byte x, y;
    public byte2(byte x, byte y) {this.x = x; this.y = y;}
}