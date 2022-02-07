using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;

public static class StaticPiecePositions
{
    public static readonly PlayerPiece[] pieceCollision = new PlayerPiece[]
    {
        new PlayerPiece(0,0), new PlayerPiece(1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // O piece rotation 0
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(2,0), new PlayerPiece(1,0), // I piece rotation 0
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // S piece rotation 0
        new PlayerPiece(0,0), new PlayerPiece(0,1), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // Z piece rotation 0
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(1,0), // L piece rotation 0
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // J piece rotation 0
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(0,1), new PlayerPiece(1,0), // T piece rotation 0

        new PlayerPiece(0,0), new PlayerPiece(1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // O piece rotation 1
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(2,0), new PlayerPiece(1,0), // I piece rotation 1
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // S piece rotation 1
        new PlayerPiece(0,0), new PlayerPiece(0,1), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // Z piece rotation 1
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(1,0), // L piece rotation 1
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // J piece rotation 1
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(0,1), new PlayerPiece(1,0), // T piece rotation 1

        new PlayerPiece(0,0), new PlayerPiece(1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // O piece rotation 2
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(2,0), new PlayerPiece(1,0), // I piece rotation 2
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // S piece rotation 2
        new PlayerPiece(0,0), new PlayerPiece(0,1), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // Z piece rotation 2
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(1,0), // L piece rotation 2
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // J piece rotation 2
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(0,1), new PlayerPiece(1,0), // T piece rotation 2

        new PlayerPiece(0,0), new PlayerPiece(1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // O piece rotation 3
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(2,0), new PlayerPiece(1,0), // I piece rotation 3
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(0,1), // S piece rotation 3
        new PlayerPiece(0,0), new PlayerPiece(0,1), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // Z piece rotation 3
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(1,1), new PlayerPiece(1,0), // L piece rotation 3
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(-1, 1), new PlayerPiece(1,0), // J piece rotation 3
        new PlayerPiece(0,0), new PlayerPiece(-1,0), new PlayerPiece(0,1), new PlayerPiece(1,0), // T piece rotation 3

    };
}
