using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    
    public List<int3> piece;
    public List<byte> board;
    public int lines;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlayerComponent { activePiece = piece.ToNativeList(Allocator.Persistent), boardState = board.ToNativeList(Allocator.Persistent), lines = lines});
    }
}
