using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerSystem : SystemBase
{
    private Inputs inputs;
    private float2 previousMovement;
    private BlobAssetStore blobAssetStore;
    private BlobAssetReference<PieceBlob> pieceCollisionReference, pieceOffsetReference;
    
    protected override void OnCreate()
    {
        inputs = new Inputs();
        inputs.Main.Enable();
        using BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var pieceCollisionBlobAsset = ref blobBuilder.ConstructRoot<PieceBlob>();
        var pieceCollisionsArray = blobBuilder.Allocate(ref pieceCollisionBlobAsset.array, 112);
        for (int i = 0; i < 112; i++)
        {
            pieceCollisionsArray[i] = StaticPiecePositions.pieceCollision[i];
        }
        pieceCollisionReference = blobBuilder.CreateBlobAssetReference<PieceBlob>(Allocator.Persistent);
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float2 movement = inputs.Main.Movement.ReadValue<UnityEngine.Vector2>();
        float2 prevMovement = previousMovement;
        BlobAssetReference<PieceBlob> collisionRef = pieceCollisionReference;
        Entities.ForEach((ref PlayerComponent player, ref DynamicBuffer<PlayerBoard> board) => 
        {
            if (player.spawnTicks > player.spawnDelay && !player.pieceSpawned)
            {
                // UnityEngine.Debug.Log("Spawned!");
                player.minoIndex = 4 * player.random.NextInt(0,6);
                player.piecePos = new int2(4,21);
                player.spawnTicks = 0f;
                player.LockTicks = 0f;
                player.pieceSpawned = true;
                // throw new System.NotImplementedException("This spawning mechanic is not implemented!");
            }
            if (!player.pieceSpawned)
            {
                player.spawnTicks += deltaTime;
                return;
            }
            player.fallenTiles += player.gravity * deltaTime;
            if (movement.y < -0.5f)
            {
                player.fallenTiles += player.gravity * deltaTime * player.softDropMultiplier; 
            }
            if (movement.y > 0.5f) 
            {
                player.fallenTiles = 999;
                player.LockTicks = player.LockDelay;
            }
            if (movement.x > 0.3f) player.autoShiftTicks += deltaTime * player.autoShiftRate;
            if (movement.x < -0.3f) player.autoShiftTicks -= deltaTime * player.autoShiftRate;
            if (movement.x < 0.3f && movement.x > -0.3f && player.autoShiftTicks != 0f) 
            {
                player.autoShiftTicks = 0f;
                player.shiftPos = 0f;
            }
            
            if (player.autoShiftTicks < -player.delayedAutoShift || (prevMovement.x > -0.3f && prevMovement.x < 0f))
            {
                player.shiftPos -= deltaTime * 60;
            }
            if (player.autoShiftTicks >= player.delayedAutoShift || (prevMovement.x < 0.3f && prevMovement.x > 0f))
            {
                player.shiftPos += deltaTime * 60;
            }
            if (player.shiftPos > 1f)
            {
                player.posToMove.x += (int)(math.floor(player.shiftPos));
                player.shiftPos -= math.floor(player.shiftPos);
            }
            if (player.shiftPos < -1f)
            {
                player.posToMove.x += (int)(math.ceil(player.shiftPos));
                player.shiftPos -= math.ceil(player.shiftPos);
            }
            if(math.any(player.posToMove != int2.zero))
            if(movePiece(board, in collisionRef, ref player, player.posToMove))
            {
                player.LockTicks = 0f;
                if(checkMovement(board, collisionRef, player.minoIndex, player.minos, player.piecePos))player.touchedGround = false;
            }
            player.posToMove = int2.zero;
            int tilesCounted = 0;
            while (player.fallenTiles > 1)
            {
                if (!checkMovement(board, in collisionRef, player.minoIndex, player.minoIndex, new int2(0,-1 -tilesCounted)))
                {
                    player.fallenTiles = 0;
                    player.touchedGround = true;
                }
                else
                {
                    tilesCounted++;
                    player.fallenTiles--;
                }
            }
            if (tilesCounted > 0) movePiece(board, in collisionRef, ref player, new int2(0,-tilesCounted));
            if (player.touchedGround) player.LockTicks += deltaTime;
            if (player.LockTicks > player.LockDelay && player.pieceSpawned)
            {
                lockPiece(ref board, ref player, in collisionRef);
                player.LockTicks = 0f;
                player.pieceSpawned = false;
            }
        }).ScheduleParallel();
        previousMovement = movement;
    }
    private static bool CheckCollision(DynamicBuffer<PlayerBoard> board, in int2 pos)
    {
        if (pos.x > 9 || pos.x < 0) return true;
        if (pos.x + (pos.y * 10) < 0 || pos.x + (pos.y * 10) >= 400) return true;
        return board[pos.x + (pos.y * 10)].value < 128;
    }

    private static bool movePiece(in DynamicBuffer<PlayerBoard> board, in BlobAssetReference<PieceBlob> array, ref PlayerComponent player, int2 pos)
    {
        if (checkMovement(board, array, player.minoIndex, player.minos, player.piecePos + pos)) player.piecePos += pos;
        else return false;
        return true;
    }

    private static bool checkMovement(in DynamicBuffer<PlayerBoard> board, in BlobAssetReference<PieceBlob> blob, in int minoIndex, in int minos, in int2 pos)
    {
        //works just fine without {}
        for (int i = 0; i < minos; i++)
        if (CheckCollision(board, blob.Value.array[minoIndex + i] + pos)) return false;
        return true;
    }

    private static void checkAndClearLines(ref DynamicBuffer<PlayerBoard> board, ref int lineCount)
    {
        NativeArray<bool> isLineFull = new NativeArray<bool>(40, Allocator.Temp);
        //reversed for loop because, why not?
        for (int y = board.Length / 10 - 1; y >= 0; y--)
        {
            isLineFull[y] = true;
            for (int i = 0; i < 10; i++)
            {
                if (board[i + (y * 10)].value < 128)
                {
                    continue;
                }
                isLineFull[y] = false;
                break;
            }
            if (!isLineFull[y])
            {
                continue;
            }
            int count = board.Length;
            lineCount++;
            for (int i = 0; i < 10; i++)
            {
                board[count - i] = new PlayerBoard(128);
            }
            for (int i = y * 10; i < count - 1; i++)
            {
                board[i] = board[i + 10];
            }
        }
        isLineFull.Dispose();
    }

    private static void lockPiece(ref DynamicBuffer<PlayerBoard> board, ref PlayerComponent player, in BlobAssetReference<PieceBlob> blob)
    {
        for (int i = 0; i < player.minos; i++)
        {
            int2 coord = blob.Value.array[player.minoIndex + i] + player.piecePos;
            board[coord.x + (coord.y * 10)] = new PlayerBoard(player.textureID);
        }
        checkAndClearLines(ref board, ref player.lines);
    }

}
