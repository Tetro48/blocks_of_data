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
    
    protected override void OnCreate()
    {
        inputs = new Inputs();
        inputs.Main.Enable();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float2 movement = inputs.Main.Movement.ReadValue<UnityEngine.Vector2>();
        float2 prevMovement = previousMovement;
        Entities.ForEach((ref PlayerComponent player, ref DynamicBuffer<PlayerBoard> board, ref DynamicBuffer<PlayerPiece> piece) => 
        {
            if (player.spawnTicks > player.spawnDelay && !player.pieceSpawned)
            {
                // UnityEngine.Debug.Log("Spawned!");
                // piece = spawnPiece(player.random.NextInt(0,6));
                movePiece(board, ref piece, new int2(4, 21));
                player.spawnTicks = 0f;
                player.pieceSpawned = true;
                throw new System.NotImplementedException("This spawning mechanic is not implemented!");
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
            
            if (player.autoShiftTicks < -player.delayedAutoShift || math.any(prevMovement != movement))
            {
                player.shiftPos -= deltaTime * 60;
            }
            if (player.autoShiftTicks >= player.delayedAutoShift || math.any(prevMovement != movement))
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
            if(movePiece(board, ref piece, player.posToMove))
            {
                player.LockTicks = 0f;
                // if(player.posToMove.y == 0)player.touchedGround = false;
            }
            player.posToMove = int2.zero;
            int tilesCounted = 0;
            while (player.fallenTiles > 1)
            {
                if (!checkMovement(board, piece, new int2(0,-1 -tilesCounted)))
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
            if (tilesCounted > 0) movePiece(board, ref piece, new int2(0,-tilesCounted));
            if (player.touchedGround) player.LockTicks += deltaTime;
            if (player.LockTicks > player.LockDelay && player.pieceSpawned)
            {
                lockPiece(ref board, ref piece, player.textureID, ref player.lines);
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

    private static bool movePiece(in DynamicBuffer<PlayerBoard> board, ref DynamicBuffer<PlayerPiece> piece, int2 pos)
    {
        if (checkMovement(board, piece, pos))
        for (int i = 0; i < piece.Length; i++)
        {
            piece[i] += pos;
        }
        return true;
    }

    private static bool checkMovement(in DynamicBuffer<PlayerBoard> board, in DynamicBuffer<PlayerPiece> piece, in int2 pos)
    {
        //works just fine without {}
        for (int i = 0; i < piece.Length; i++)
        if (CheckCollision(board, piece[i].value + pos)) return false;
        return true;
    }

    private static void checkAndClearLines(ref DynamicBuffer<PlayerBoard> board, ref int lineCount)
    {
        NativeArray<bool> isLineFull = new NativeArray<bool>(40, Allocator.Temp);
        PlayerBoard modBoard = new PlayerBoard();
        for (int y = 0; y < board.Length / 10; y++)
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
                modBoard.value = 128;
                board[count - i] = modBoard;
            }
            for (int i = y * 10; i < count - 1; i++)
            {
                board[i] = board[i + 10];
            }
        }
        isLineFull.Dispose();
    }

    private static void lockPiece(ref DynamicBuffer<PlayerBoard> board, ref DynamicBuffer<PlayerPiece> piece, in byte textureID, ref int lineCount)
    {
        for (int i = 0; i < piece.Length; i++)
        {
            int2 coord = piece[i].value;
            PlayerBoard modBoard = new PlayerBoard();
            modBoard.value = textureID;
            board[coord.x + (coord.y * 10)] = modBoard;
        }
        checkAndClearLines(ref board, ref lineCount);
    }

}
