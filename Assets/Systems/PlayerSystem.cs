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
                UnityEngine.Debug.Log("Spawned!");
                player.minoIndex = 4 * player.random.NextInt(0,6);
                player.piecePos = new int2(4,21);
                player.spawnTicks = 0f;
                player.lockTicks = 0f;
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
                player.lockTicks = player.lockDelay;
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
            if(horizontalMovePiece(board, in collisionRef, ref player))
            {
                player.lockTicks = 0f;
            }
            player.posToMove = int2.zero;
            int tilesCounted = 0;
            while (player.fallenTiles > 1)
            {
                if (!checkMovement(board, in collisionRef, player.minoIndex, player.minoIndex, new int2(0,player.piecePos.y -1 -tilesCounted)))
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
            if (player.touchedGround) player.lockTicks += deltaTime;
            if (player.lockTicks > player.lockDelay && player.pieceSpawned)
            {
                lockPiece(ref board, ref player, in collisionRef);
                player.lockTicks = 0f;
                player.pieceSpawned = false;
            }
        }).WithoutBurst().Run();
        previousMovement = movement;
    }
    private static int ArrayFlatIndexing(int[] arraySizes, params int[] indexArray)
    {
        if (arraySizes.Length +1 != indexArray.Length)
        {
            throw new System.ArgumentException("Array dimensions don't match!");
        }
        int result = indexArray[0];
        for (int i = 0; i < arraySizes.Length; i++)
        {
            result += indexArray[i+1] * arraySizes[i]; 
        }
        return result;
    }
    private static bool CheckCollision(in DynamicBuffer<PlayerBoard> board, in int2 pos)
    {
        if (pos.x < 0 || pos.x > 9) return true;
        if (pos.y < 0 || pos.y > 39) return true;
        return board[pos.y * 10 + pos.x] < 128;
    }
    private static bool horizontalMovePiece(in DynamicBuffer<PlayerBoard> board, in BlobAssetReference<PieceBlob> array, ref PlayerComponent player)
    {
        int tilesCounted = 0;
        while (player.posToMove.x != 0)
        {
            if (!checkMovement(board, in array, player.minoIndex, player.minoIndex, new int2(0,player.piecePos.x+tilesCounted)))
            {
                player.posToMove.x = 0;
                player.touchedGround = false;
            }
            else
            {
                tilesCounted += player.posToMove.x > 0 ? 1 : -1;
                player.posToMove += player.posToMove.x > 0 ? -1 : 1;
            }
        }
        return movePiece(board, array, ref player, new int2(tilesCounted, 0));
    }

    private static bool movePiece(in DynamicBuffer<PlayerBoard> board, in BlobAssetReference<PieceBlob> array, ref PlayerComponent player, int2 pos)
    {
        if (checkMovement(board, array, player.minoIndex, player.minos, player.piecePos + pos)) player.piecePos += pos;
        else
        {
            UnityEngine.Debug.Log("Piece collision at "+ player.piecePos + " trying to move by " + pos);
            return false;
        }
        return true;
    }

    private static bool checkMovement(in DynamicBuffer<PlayerBoard> board, in BlobAssetReference<PieceBlob> blob, in int minoIndex, in int minos, in int2 pos)
    {
        //works just fine without {}
        for (int i = 0; i < minos; i++)
        if (CheckCollision(board, blob.Value.array[minoIndex + i] + pos)) {UnityEngine.Debug.Log("returned false"); return false;}
        UnityEngine.Debug.Log("returned true");
        return true;
    }

    //Notice: Line dropping is instant. The original owner is quite concerned with memory use on this ECS project.
    private static void checkAndClearLines(ref DynamicBuffer<PlayerBoard> board, ref int lineCount)
    {
        NativeArray<bool> isLineFull = new NativeArray<bool>(40, Allocator.Temp);
        //reversed for loop because, why not?
        for (int y = board.Length / 10 - 1; y >= 0; y--)
        {
            isLineFull[y] = true;
            //Checking if a single mino is empty on a line.
            for (int i = 0; i < 10; i++)
            {
                if (board[i + (y * 10)] < 128)
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
            lineCount++;
            //Line clearing
            for (int i = 0; i < 10; i++)
            {
                board[y * 10 + i] = 128;
            }
            //Matrix drop
            for (int i = y * 10; i < board.Length - 1; i++)
            {
                board[i] = board[i + 10];
            }
        }
        //This had to be used to avoid 40-50 bytes worth of memory leak.
        isLineFull.Dispose();
    }

    private static void lockPiece(ref DynamicBuffer<PlayerBoard> board, ref PlayerComponent player, in BlobAssetReference<PieceBlob> blob)
    {
        for (int i = 0; i < player.minos; i++)
        {
            int2 coord = blob.Value.array[player.minoIndex + i] + player.piecePos;
            board[coord.x + (coord.y * 10)] = player.textureID;
        }
        checkAndClearLines(ref board, ref player.lines);
    }

}
