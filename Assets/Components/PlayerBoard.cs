using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, InternalBufferCapacity(400), GenerateAuthoringComponent]
//this name is somewhat misnomer and confusing.
public struct PlayerBoard : IBufferElementData
{
    public byte value;
    
    public static implicit operator PlayerBoard(byte data) => new PlayerBoard {value = data};
    public static implicit operator byte(PlayerBoard tile) => tile.value;
}