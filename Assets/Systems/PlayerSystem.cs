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
        
        Entities.ForEach((ref PlayerComponent player) => {
            NativeArray<bool> isLineFull = new NativeArray<bool>(40, Allocator.TempJob);
            player.fallenTiles += player.gravity * deltaTime;
            player.posToMove.y -= (int)math.floor(player.fallenTiles);
            for (int i = 0; i < player.activePiece.Length; i++)
            {
                player.activePiece[i] += player.posToMove;
            }
            player.fallenTiles -= math.floor(player.fallenTiles);
            for (int y = 0; y < player.boardState.Length / 10; y++)
            {
                isLineFull[y] = true;
                for (int i = 0; i < 10; i++)
                {
                    if (player.boardState[i + (y * 10)] < 128)
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
                int count = player.boardState.Length;
                for (int i = 0; i < 10; i++)
                {
                    player.boardState[count - i] = 128;
                }
                for (int i = y * 10; i < count - 1; i++)
                {
                    player.boardState[i] = player.boardState[i+10];
                }
            }
            isLineFull.Dispose();
        }).ScheduleParallel();
    }
}
