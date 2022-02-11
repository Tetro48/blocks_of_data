using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;

public static class StaticPiecePositions
{
    //
    public static readonly NativeList<int2> JLSTZpieceRotationOffset = new NativeList<int2> (Allocator.Persistent){
        int2.zero, int2.zero, int2.zero, int2.zero,
        int2.zero, new int2(1,0), int2.zero, new int2(-1, 0),
        int2.zero, new int2(1, -1), int2.zero, new int2(-1, -1),
        int2.zero, new int2(0, 2), int2.zero, new int2(0, 2),
        int2.zero, new int2(1, 2), int2.zero, new int2(-1, 2) 
    };
    public static readonly NativeList<int2> pieceCollision = new NativeList<int2>(Allocator.Persistent)
    {
        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 0
        new int2(0,0), new int2(-1,0), new int2(2,0), new int2(1,0), // I piece rotation 0
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(0,1), // S piece rotation 0
        new int2(0,0), new int2(0,1), new int2(-1, 1), new int2(1,0), // Z piece rotation 0
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(1,0), // L piece rotation 0
        new int2(0,0), new int2(-1,0), new int2(-1, 1), new int2(1,0), // J piece rotation 0
        new int2(0,0), new int2(-1,0), new int2(0,1), new int2(1,0), // T piece rotation 0

        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(2,0), new int2(1,0), // I piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(0,1), // S piece rotation 1
        new int2(0,0), new int2(0,1), new int2(-1, 1), new int2(1,0), // Z piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(1,0), // L piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(-1, 1), new int2(1,0), // J piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(0,1), new int2(1,0), // T piece rotation 1

        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 2
        new int2(0,0), new int2(-1,0), new int2(2,0), new int2(1,0), // I piece rotation 2
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(0,1), // S piece rotation 2
        new int2(0,0), new int2(0,1), new int2(-1, 1), new int2(1,0), // Z piece rotation 2
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(1,0), // L piece rotation 2
        new int2(0,0), new int2(-1,0), new int2(-1, 1), new int2(1,0), // J piece rotation 2
        new int2(0,0), new int2(-1,0), new int2(0,1), new int2(1,0), // T piece rotation 2

        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 3
        new int2(0,0), new int2(-1,0), new int2(2,0), new int2(1,0), // I piece rotation 3
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(0,1), // S piece rotation 3
        new int2(0,0), new int2(0,1), new int2(-1, 1), new int2(1,0), // Z piece rotation 3
        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(1,0), // L piece rotation 3
        new int2(0,0), new int2(-1,0), new int2(-1, 1), new int2(1,0), // J piece rotation 3
        new int2(0,0), new int2(-1,0), new int2(0,1), new int2(1,0), // T piece rotation 3

    };
}
