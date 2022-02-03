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
        
        Entities.ForEach((PlayerComponent player) => {
            NativeArray<bool> isLineFull = new NativeArray<bool>(40, Allocator.TempJob);
            for (int y = 0; y < player.boardState.Length / 10; y++)
            {
                isLineFull[y] = true;
                for (int i = 0; i < 10; i++)
                {
                    if (player.boardState[i + (y * 10)] > 127)
                    {
                        isLineFull[y] = false;
                        break;
                    }
                }
                if (isLineFull[y])
                {
                    int count = player.boardState.Length;
                    for (int i = y * 10; i < count - 1; i++)
                    {
                        player.boardState[i+10] = player.boardState[i];
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        player.boardState[count - i] = 128;
                    }
                }
            }
            isLineFull.Dispose();
        }).Schedule();
    }
}
