using Unity.Burst;
using Unity.Entities;

partial struct DestroyStoppedSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();
        var elapsedTime = (float)SystemAPI.Time.ElapsedTime;
        
        state.Dependency = new DestroyStoppedJob
        {
            ElapsedTime = elapsedTime,
            ECB = ecb,
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
[WithAll(typeof(StopRotatingComponent))]
partial struct DestroyStoppedJob : IJobEntity
{
    public float ElapsedTime;
    public EntityCommandBuffer.ParallelWriter ECB;
    
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in StopRotatingComponent stopRotating, in RandomDataComponent randomData)
    {
        if (ElapsedTime <= stopRotating.TimeToStop + randomData.Duration)
        {
            return;
        }
        
        ECB.DestroyEntity(chunkIndex, entity);
    }
}
