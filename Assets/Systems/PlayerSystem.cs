using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref PlayerComponent player, ref DynamicBuffer<PlayerBoard> board, ref DynamicBuffer<PlayerPiece> piece) => {
            NativeArray<bool> isLineFull = new NativeArray<bool>(40, Allocator.TempJob);
            player.fallenTiles += player.gravity * deltaTime;
            player.posToMove.y -= (int)math.floor(player.fallenTiles);
            PlayerPiece modPiece = new PlayerPiece();
            PlayerBoard modBoard = new PlayerBoard();
            for (int i = 0; i < piece.Length; i++)
            {
                modPiece.value = piece[i].value + player.posToMove;
                piece[i] = modPiece;
            }
            player.posToMove = int2.zero;
            player.fallenTiles -= math.floor(player.fallenTiles);
            for (int y = 0; y < board.Length / 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if(board[x + (y * 10)].value < 128) {}
                }
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
        }).ScheduleParallel();
    }
}
