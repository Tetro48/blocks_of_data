using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerSystem : SystemBase
{
    Random randomizer;
    Inputs inputs;
    protected override void OnCreate()
    {
        inputs = new Inputs();
        inputs.Main.Enable();
        randomizer = new Random();
    }
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float2 movement = inputs.Main.Movement.ReadValue<UnityEngine.Vector2>();

        Entities.ForEach((ref PlayerComponent player, ref DynamicBuffer<PlayerBoard> board, ref DynamicBuffer<PlayerPiece> piece) => 
        {
            player.fallenTiles += player.gravity * deltaTime;
            // if (player.spawnTicks > player.spawnDelay)
            // {
                // piece.CopyFrom(spawnPieceInt[player.random.NextInt(0,6)]);
            //     movePiece(board, ref piece, new int2(4, 21));
            //     player.pieceSpawned = true;
            // }
            if (!player.pieceSpawned)
            {
                player.spawnTicks += deltaTime;
                return;
            }
            if (movement.y < -0.5)
            {
                player.fallenTiles += player.gravity * deltaTime * player.softDropMultiplier; 
            }
            // PlayerPiece modMino = new PlayerPiece();
            if (player.delayedAutoShift_L >= player.delayedAutoShift)
            {
                player.shiftPos -= deltaTime * 60;
            }
            if (player.delayedAutoShift_R >= player.delayedAutoShift)
            {
                player.shiftPos += deltaTime * 60;
            }
            if (math.any(player.shiftPos > new float2(1f,1f)))
            {
                player.posToMove.x += (int)(math.floor(player.shiftPos));
                player.shiftPos -= math.floor(player.shiftPos);
            }
            if(math.any(player.posToMove != int2.zero))
            if(movePiece(board, ref piece, player.posToMove))
            {
                player.LockTicks = 0f;
                player.touchedGround = false;
            }
            player.posToMove = int2.zero;
            DynamicBuffer<PlayerPiece> modPiece = piece;
            while (player.fallenTiles > 1)
            {
                if(!movePiece(board, ref piece, new int2(0,-1)) && checkMovement(board, piece, new int2(0,-1)))
                {
                    player.touchedGround = true;
                }
                player.fallenTiles--;
            }
            if (player.touchedGround) player.LockTicks += deltaTime;
            if (player.LockTicks > player.LockDelay)
            {
                lockPiece(ref board, ref piece, player.textureID);
                player.pieceSpawned = false;
            }
            checkAndClearLines(ref board);
        }).ScheduleParallel();
        
        bool CheckCollision(DynamicBuffer<PlayerBoard> board, in int2 pos)
        {
            if(pos.x > 9 || pos.x < 0) return true;
            if(pos.x + (pos.y * 10) < 0 || pos.x + (pos.y * 10) >= 400) return true;
            return board[pos.x + (pos.y * 10)].value < 128;
        }
        bool movePiece(in DynamicBuffer<PlayerBoard> board, ref DynamicBuffer<PlayerPiece> piece, int2 pos)
        {
            PlayerPiece modMino = new PlayerPiece();
            if (checkMovement(board, piece, pos))
            for (int i = 0; i < piece.Length; i++)
            {
                modMino.value = piece[i].value + pos;
                piece[i] = modMino;
            }
            return true;
        }
        bool checkMovement(in DynamicBuffer<PlayerBoard> board, in DynamicBuffer<PlayerPiece> piece, in int2 pos)
        {
            for (int i = 0; i < piece.Length; i++)
            {
                if (CheckCollision(board, piece[i].value + pos))
                {
                    return false;
                }
            }
            return true;
        }
        void checkAndClearLines(ref DynamicBuffer<PlayerBoard> board)
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
                for (int i = 0; i < 10; i++)
                {
                    modBoard.value = 128;
                    board[count - i] = modBoard;
                }
                for (int i = y * 10; i < count - 1; i++)
                {
                    board[i] = board[i+10];
                }
            }
            isLineFull.Dispose();
        }
        void lockPiece(ref DynamicBuffer<PlayerBoard> board, ref DynamicBuffer<PlayerPiece> piece, in byte textureID)
        {
            for (int i = 0; i < piece.Length; i++)
            {
                int2 coord = piece[i].value;
                PlayerBoard modBoard = new PlayerBoard();
                modBoard.value = textureID;
                board[coord.x + (coord.y * 10)] = modBoard;
            }
        }
    }
}
