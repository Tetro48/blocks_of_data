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
    private BlobAssetReference<PieceBlob> pieceCollisionReference, JLSTZpieceOffsetReference;
    
    protected override void OnCreate()
    {
        inputs = new Inputs();
        inputs.Main.Enable();
        pieceCollisionReference = CreatePieceBlobAsset(StaticPiecePositions.pieceCollision);
        JLSTZpieceOffsetReference = CreatePieceBlobAsset(StaticPiecePositions.JLSTZpieceRotationOffset);
    }
    //friendly reminder that we're going to create immutable data of int2s.
    private static BlobAssetReference<PieceBlob> CreatePieceBlobAsset(NativeList<int2> array)
    {
        using BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
        ref PieceBlob pieceCollisionBlobAsset = ref blobBuilder.ConstructRoot<PieceBlob>();
        var blobArray = blobBuilder.Allocate(ref pieceCollisionBlobAsset.array, array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            blobArray[i] = array[i];
        }
        return blobBuilder.CreateBlobAssetReference<PieceBlob>(Allocator.Persistent);
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float2 movement = inputs.Main.Movement.ReadValue<UnityEngine.Vector2>();
        float2 prevMovement = previousMovement;
        bool4x2 jobBoolInputs = new bool4x2(
            inputs.Main.CWRotation.WasPressedThisFrame(),
            inputs.Main.CW2Rotation.WasPressedThisFrame(),
            inputs.Main.CCWRotation.WasPressedThisFrame(),
            inputs.Main.CCW2Rotation.WasPressedThisFrame(),
            inputs.Main.UDRotation.WasPressedThisFrame(),
            inputs.Main.UD2Rotation.WasPressedThisFrame(),
            false,
            false);
        BlobAssetReference<PieceBlob> collisionRef = pieceCollisionReference;
        //for easier debugging, use .WithoutBurst().Run(), otherwise, .ScheduleParallel()
        Entities.ForEach((ref PlayerComponent player, ref DynamicBuffer<PlayerBoard> board) =>
        {
            if (player.spawnTicks > player.spawnDelay && !player.pieceSpawned)
            {
                uint uint1 = player.random.NextUInt(6);
                byte convertedInt = (byte)uint1;
                player.textureID = convertedInt;
                //bitwise left shifting?
                player.minoIndex = player.textureID << 4;
                player.rotationIndex = 0;
                player.piecePos = new int2(4, 21);
                player.spawnTicks = 0f;
                player.lockTicks = 0f;
                player.pieceSpawned = true;
                player.touchedGround = false;
                // throw new System.NotImplementedException("This spawning mechanic is not implemented!");
            }
            if (!player.pieceSpawned)
            {
                player.spawnTicks += deltaTime;
                return;
            }
            player.fallenTiles += player.gravity * deltaTime;
            if(player.isControllable)
            {
                player.movement = movement;
                player.inputs = jobBoolInputs;    
            }
                
            #region Piece Movement
                
            if (player.movement.y < -0.5f)
            {
                player.fallenTiles += player.gravity * deltaTime * player.softDropMultiplier;
            }
            if (player.movement.y > 0.5f)
            {
                player.fallenTiles = 999;
                player.lockTicks = player.lockDelay;
            }
            if (player.autoShiftTicks == 0f && player.movement.x != 0)
            {
                movePiece(board, collisionRef, ref player, new int2(player.movement.x > 0f ? 1 : -1, 0));
            }
            if (player.movement.x > 0.3f) player.autoShiftTicks += deltaTime;
            if (player.movement.x < -0.3f) player.autoShiftTicks -= deltaTime;
            if (player.movement.x < 0.3f && player.movement.x > -0.3f && player.autoShiftTicks != 0f)
            {
                player.autoShiftTicks = 0f;
                player.shiftPos = 0f;
            }

            if (player.autoShiftTicks < -player.delayedAutoShift)
            {
                player.shiftPos -= deltaTime * 60 * player.autoShiftRate;
            }
            if (player.autoShiftTicks >= player.delayedAutoShift)
            {
                player.shiftPos += deltaTime * 60 * player.autoShiftRate;
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
            if (math.any(player.posToMove != int2.zero))
            if (horizontalMovePiece(board, in collisionRef, ref player))
            {
                player.lockTicks = 0f;
            }
            #endregion
            
            #region Piece Rotation
            if (player.inputs.c0.x | player.inputs.c0.y)
            {
                RotatePiece(ref player, 1);
            }
            if (player.inputs.c0.z | player.inputs.c0.w)
            {
                RotatePiece(ref player, -1);
            }
            if (player.inputs.c1.x | player.inputs.c1.y)
            {
                RotatePiece(ref player, 2);
            }
            #endregion
            
            #region Piece Gravity
            player.posToMove = int2.zero;
            int tilesCounted = 0;
            while (player.fallenTiles > 1)
            {
                if (!checkMovement(board, in collisionRef, player.minoIndex, player.minos, new int2(player.piecePos.x, player.piecePos.y - 1 - tilesCounted)))
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
            if (tilesCounted > 0) movePiece(board, in collisionRef, ref player, new int2(0, -tilesCounted));
            if (player.touchedGround) player.lockTicks += deltaTime;
            if (player.lockTicks > player.lockDelay && player.pieceSpawned)
            {
                lockPiece(ref board, ref player, in collisionRef);
                player.lockTicks = 0f;
                player.pieceSpawned = false;
            }
            #endregion
            player.prevMovement = movement;
        }).ScheduleParallel();
        previousMovement = movement;
    }

    private static void RotatePiece(ref PlayerComponent player, in sbyte addRotIndex, in byte maxRotIndex = 4)
    {
        byte oldRotIndex = player.rotationIndex;
        player.rotationIndex += (byte)addRotIndex;
        if (player.rotationIndex > maxRotIndex - 1)
        {
            player.rotationIndex -= maxRotIndex;
        }
        if (player.rotationIndex > 0x80)
        {
            player.rotationIndex += maxRotIndex;
        }
        player.minoIndex += (player.rotationIndex - oldRotIndex)<<2;
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
    /// <summary>
    /// Checks collisions. Returns true if there's a block in specified position. Otherwise, returns false.
    /// </summary>
    private static bool CheckCollision(in DynamicBuffer<PlayerBoard> board, in int2 pos)
    {
        if (pos.x < 0 || pos.x > 9 || pos.y < 0 || pos.y > 39) return true;
        
        return board[pos.y * 10 + pos.x] < 128;
    }
    private static bool horizontalMovePiece(in DynamicBuffer<PlayerBoard> board, in BlobAssetReference<PieceBlob> array, ref PlayerComponent player)
    {
        int tilesCounted = 0;
        while (player.posToMove.x != 0)
        {
            if (!checkMovement(board, in array, player.minoIndex, player.minos, new int2(player.piecePos.x+tilesCounted, player.piecePos.y)))
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
            return false;
        }
        return true;
    }

    private static bool checkMovement(in DynamicBuffer<PlayerBoard> board, in BlobAssetReference<PieceBlob> blob, in int minoIndex, in int minos, in int2 pos)
    {
        //works just fine without {}
        //notice: {} is here in this function for... debugging???
        for (int i = 0; i < minos; i++)
        if (CheckCollision(board, blob.Value.array[minoIndex + i] + pos)) return false;
        return true;
    }

    //Notice: Line dropping is instant. The original creator is quite concerned with memory use on this ECS project.
    private static void checkAndClearLines(ref DynamicBuffer<PlayerBoard> board, ref int lineCount)
    {
        NativeArray<bool> isLineFull = new NativeArray<bool>(40, Allocator.Temp);
        //reversed for loop because, why not?
        for (int y = board.Length / 10 - 1; y >= 0; y--)
        {
            isLineFull[y] = true;
            int transformedY = y * 10;
            //Checking if a single mino is empty on a line.
            for (int i = 0; i < 10; i++)
            {
                if (board[i + (transformedY)] < 128)
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
                board[transformedY + i] = 128;
            }
            //Matrix drop
            for (int i = transformedY; i < board.Length - 10; i++)
            {
                board[i] = board[i + 10];
            }
            for (int i = 390; i < 400; i++)
            {
                board[i] = 128;
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
