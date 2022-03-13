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
    public static readonly NativeList<int2> IpieceRotationOffset = new NativeList<int2> (Allocator.Persistent){
        int2.zero, int2.zero, int2.zero, int2.zero,
        int2.zero, new int2(2,0), new int2(3,0), new int2(-1,0),
        new int2(2,0), new int2(-1,0), new int2(1, 0), new int2(0, 0),
        new int2(-1,0), new int2(1, 1), new int2(2, -1), new int2(0, -2),
        new int2(2,0), new int2(0, -2), new int2(-2, 0), new int2(0, 2),
    };
    public static readonly NativeList<int2> OpieceRotationOffset = new NativeList<int2> (Allocator.Persistent){
        int2.zero, int2.zero, int2.zero, int2.zero
    };
    public static readonly NativeList<int2> pieceCollision = new NativeList<int2>(Allocator.Persistent)
    {
        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 0
        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 1
        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 2
        new int2(0,0), new int2(1,0), new int2(1,1), new int2(0,1), // O piece rotation 3

        new int2(0,0), new int2(-1,0), new int2(2,0), new int2(1,0), // I piece rotation 0
        new int2(1,0), new int2(1,-1), new int2(1,1), new int2(1,2), // I piece rotation 1
        new int2(0,-1), new int2(-1,-1), new int2(2,-1), new int2(1,-1), // I piece rotation 2
        new int2(0,0), new int2(0,-1), new int2(0,1), new int2(0,2), // I piece rotation 2

        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(0,1), // S piece rotation 0
        new int2(0,0), new int2(0,1), new int2(1,0), new int2(1,-1), // S piece rotation 1
        new int2(0,0), new int2(-1,-1), new int2(1,0), new int2(0,-1), // S piece rotation 2
        new int2(0,0), new int2(-1,1), new int2(-1,0), new int2(0,-1), // S piece rotation 3

        new int2(0,0), new int2(0,1), new int2(-1,1), new int2(1,0), // Z piece rotation 0
        new int2(0,0), new int2(1,1), new int2(0,-1), new int2(1,0), // Z piece rotation 1
        new int2(0,0), new int2(0,-1), new int2(-1,0), new int2(1,-1), // Z piece rotation 2
        new int2(0,0), new int2(0,1), new int2(-1,-1), new int2(-1,0), // Z piece rotation 3

        new int2(0,0), new int2(-1,0), new int2(1,1), new int2(1,0), // L piece rotation 0
        new int2(0,0), new int2(0,-1), new int2(0,1), new int2(1,-1), // L piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(-1,-1), new int2(1,0), // L piece rotation 2
        new int2(0,0), new int2(0,-1), new int2(0,1), new int2(-1,1), // L piece rotation 3

        new int2(0,0), new int2(-1,0), new int2(-1,1), new int2(1,0), // J piece rotation 0
        new int2(0,0), new int2(0,-1), new int2(0,1), new int2(1,1), // J piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(1,-1), new int2(1,0), // J piece rotation 2
        new int2(0,0), new int2(0,-1), new int2(0,1), new int2(-1,-1), // J piece rotation 3

        new int2(0,0), new int2(-1,0), new int2(0,1), new int2(1,0), // T piece rotation 0
        new int2(0,0), new int2(0,-1), new int2(0,1), new int2(1,0), // T piece rotation 1
        new int2(0,0), new int2(-1,0), new int2(0,-1), new int2(1,0), // T piece rotation 2
        new int2(0,0), new int2(-1,0), new int2(0,1), new int2(0,-1), // T piece rotation 3

    };
    // note: this part is from ti_srs.lua in Cambridge's code. Notice how it kinda looks familiar?
    // SRS.block_offsets = {
    //     I={
    //         { {x=0, y=0}, {x=-1, y=0}, {x=-2, y=0}, {x=1, y=0} }, -- rot 0
    //         { {x=0, y=0}, {x=0, y=-1}, {x=0, y=1}, {x=0, y=2} }, -- rot 1
    //         { {x=0, y=1}, {x=-1, y=1}, {x=-2, y=1}, {x=1, y=1} }, -- rot 2
    //         { {x=-1, y=0}, {x=-1, y=-1}, {x=-1, y=1}, {x=-1, y=2} }, -- rot 3
    //     },
    //     J={
    //         { {x=0, y=0}, {x=-1, y=0}, {x=1, y=0}, {x=-1, y=-1} }, -- rot 0
    //         { {x=0, y=0}, {x=0, y=-1}, {x=0, y=1} , {x=1, y=-1} }, -- rot 1
    //         { {x=0, y=0}, {x=1, y=0}, {x=-1, y=0}, {x=1, y=1} }, -- rot 2
    //         { {x=0, y=0}, {x=0, y=1}, {x=0, y=-1}, {x=-1, y=1} }, -- rot 3
    //     },
    //     L={
    //         { {x=0, y=0}, {x=-1, y=0}, {x=1, y=0}, {x=1, y=-1} },
    //         { {x=0, y=0}, {x=0, y=-1}, {x=0, y=1}, {x=1, y=1} },
    //         { {x=0, y=0}, {x=1, y=0}, {x=-1, y=0}, {x=-1, y=1} },
    //         { {x=0, y=0}, {x=0, y=1}, {x=0, y=-1}, {x=-1, y=-1} },
    //     },
    //     O={
    //         { {x=0, y=0}, {x=-1, y=0}, {x=-1, y=-1}, {x=0, y=-1} },
    //         { {x=0, y=0}, {x=-1, y=0}, {x=-1, y=-1}, {x=0, y=-1} },
    //         { {x=0, y=0}, {x=-1, y=0}, {x=-1, y=-1}, {x=0, y=-1} },
    //         { {x=0, y=0}, {x=-1, y=0}, {x=-1, y=-1}, {x=0, y=-1} },
    //     },
    //     S={
    //         { {x=1, y=-1}, {x=0, y=-1}, {x=0, y=0}, {x=-1, y=0} },
    //         { {x=1, y=1}, {x=1, y=0}, {x=0, y=0}, {x=0, y=-1} },
    //         { {x=-1, y=1}, {x=0, y=1}, {x=0, y=0}, {x=1, y=0} },
    //         { {x=-1, y=-1}, {x=-1, y=0}, {x=0, y=0}, {x=0, y=1} },
    //     },
    //     T={
    //         { {x=0, y=0}, {x=-1, y=0}, {x=1, y=0}, {x=0, y=-1} },
    //         { {x=0, y=0}, {x=0, y=-1}, {x=0, y=1}, {x=1, y=0} },
    //         { {x=0, y=0}, {x=1, y=0}, {x=-1, y=0}, {x=0, y=1} },
    //         { {x=0, y=0}, {x=0, y=1}, {x=0, y=-1}, {x=-1, y=0} },
    //     },
    //     Z={
    //         { {x=-1, y=-1}, {x=0, y=-1}, {x=0, y=0}, {x=1, y=0} },
    //         { {x=1, y=-1}, {x=1, y=0}, {x=0, y=0}, {x=0, y=1} },
    //         { {x=1, y=1}, {x=0, y=1}, {x=0, y=0}, {x=-1, y=0} },
    //         { {x=-1, y=1}, {x=-1, y=0}, {x=0, y=0}, {x=0, y=-1} },
    //     }
    // }
}

