using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;

public class Spawner : MonoBehaviour
{
    public EntityManager manager;
    public Unity.Mathematics.Random random = new Unity.Mathematics.Random();
    public bool isRandom;
    public int playerCount = 2;
    // Start is called before the first frame update
    void Start()
    {
        System.Collections.Generic.List<PlayerBoard> board = new System.Collections.Generic.List<PlayerBoard>();
        for (int i = 0; i < 400; i++)
        {
            board.Add(128);
        }
        NativeArray<PlayerBoard> nativeBoard = board.ToNativeArray(Allocator.Persistent);
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < playerCount; i++)
        {
            Entity newEntity = manager.CreateEntity();
            if(isRandom)
            manager.AddComponentData(newEntity, new PlayerComponent {autoShiftRate = random.NextFloat(1), autoShiftTicks = 0, 
            delayedAutoShift = random.NextFloat(1), fallenTiles = 0f, gravity = random.NextFloat(600), inputs = new bool4x2(), isControllable = false,
            lineDropDelay = random.NextFloat(1), lineDropTicks = 0, lines = 0, lineSpawnDelay = random.NextFloat(1), lockDelay = random.NextFloat(1), lockTicks = 0,
            minoIndex = random.NextInt(0,6) << 4, minos = 4, piecePos = new int2(4,21), pieceSpawned = true, posToMove = int2.zero, random = new Unity.Mathematics.Random(random.NextUInt()), 
            rotationIndex = 0, shiftPos = 0f, softDropMultiplier = random.NextFloat(120), spawnDelay = random.NextFloat(1), spawnTicks = 0f, 
            textureID = (byte)random.NextInt(0,127), touchedGround = false});
            else
            manager.AddComponentData(newEntity, new PlayerComponent {autoShiftRate = 0.5f, autoShiftTicks = 0, 
            delayedAutoShift = 0.5f, fallenTiles = 0f, gravity = 0.5f, inputs = new bool4x2(), isControllable = false,
            lineDropDelay = 0.5f, lineDropTicks = 0, lines = 0, lineSpawnDelay = 0.5f, lockDelay = 0.5f, lockTicks = 0,
            minoIndex = random.NextInt(0,6) << 4, minos = 4, piecePos = new int2(4,21), pieceSpawned = true, posToMove = int2.zero, random = new Unity.Mathematics.Random(random.NextUInt()), 
            rotationIndex = 0, shiftPos = 0f, softDropMultiplier = 60f, spawnDelay = 0.5f, spawnTicks = 0f, 
            textureID = 63, touchedGround = false});
            manager.AddBuffer<PlayerBag>(newEntity);
            manager.AddBuffer<PlayerBoard>(newEntity).AddRange(nativeBoard);
            manager.AddComponentData(newEntity, new LocalToWorld());
            manager.AddComponentData(newEntity, new Translation { Value = random.NextFloat3(new float3(-100,-100,-100), new float3(100,100,100))});
        }
        nativeBoard.Dispose();
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
