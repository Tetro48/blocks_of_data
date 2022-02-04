using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, InternalBufferCapacity(400), GenerateAuthoringComponent]
public struct PlayerBoard : IBufferElementData
{
    public byte value;
    
    public PlayerBoard(byte data) => value = data; 
}