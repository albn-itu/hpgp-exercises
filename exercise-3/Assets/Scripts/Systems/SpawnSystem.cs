using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct SpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false; // Disable the system after first run

        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();
        state.Dependency = new SpawnCubesJob
        {
            ECB = ecb,
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
partial struct SpawnCubesJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public void Execute([ChunkIndexInQuery] int chunkIndex, in ConfigComponent config)
    {
        int n = config.spawnCount;
        for (int i = 0; i < n * n * n; i++)
        {
            var e = ECB.Instantiate(chunkIndex, config.prefab);
            float x = (i % n) * 2f;
            float y = ((i / n) % n) * 2f;
            float z = (i / (n * n)) * 2f;
            
            ECB.AddComponent(chunkIndex, e, LocalTransform.FromPosition(new float3(x, y, z)));
        }
    }
}