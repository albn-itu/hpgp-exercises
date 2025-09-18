using System.Diagnostics.CodeAnalysis;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

partial struct StopRotatingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
    }

    [ExecuteAlways]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();
        var elapsedTime = (float)SystemAPI.Time.ElapsedTime;
        
        JobHandle initRandomDataHandle = new InitRandomDataJob
        {
            ElapsedTime = elapsedTime,
        }.ScheduleParallel(state.Dependency);
        
        JobHandle addStopRotationComponentHandle = new AddStopRotatingComponentJob
        {
            ElapsedTime = elapsedTime,
            ECB = ecb,
        }.ScheduleParallel(initRandomDataHandle);
        
        state.Dependency = new StopRotatingJob
        {
            ElapsedTime = elapsedTime,
            ECB = ecb,
        }.ScheduleParallel(addStopRotationComponentHandle);
    }
}

[BurstCompile]
partial struct InitRandomDataJob : IJobEntity
{
    public float ElapsedTime;
    public void Execute([EntityIndexInQuery] int entityIndex, ref RandomDataComponent randomData)
    {
        if (randomData.Initialized) return;
        
        randomData.Generator.InitState((uint)entityIndex + 42 + (uint)ElapsedTime);
        randomData.Initialized = true;
        randomData.InitializedTime += ElapsedTime;
    }
}

[BurstCompile]
[WithNone(typeof(StopRotatingComponent))]
partial struct AddStopRotatingComponentJob : IJobEntity
{
    public float ElapsedTime;
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute(Entity entity, [ChunkIndexInQuery]int chunkIndex, ref RandomDataComponent randomData, ref URPMaterialPropertyBaseColor baseColor)
    {
        var red = new float4(1, 0, 0, 1);
        var green = new float4(0, 1, 0, 1);
        var blue = new float4(0, 0, 1, 1);
        
        float time = ElapsedTime - randomData.InitializedTime;
        
        baseColor.Value = math.lerp(blue, green, math.saturate(time / randomData.Duration));
        
        if (time < randomData.Duration) return;

        if (randomData.Generator.NextInt(0, 100) % 2 == 0)
        {
            ECB.AddComponent(chunkIndex, entity, new StopRotatingComponent
            {
                TimeToStop = ElapsedTime + randomData.Generator.NextInt(1, 5)
            });
            
            baseColor.Value = red;
        }
        else
        {
            randomData.InitializedTime = ElapsedTime;
            baseColor.Value = blue;
        }
    }
}

[BurstCompile]
[WithAll(typeof(StopRotatingComponent))]
partial struct StopRotatingJob : IJobEntity
{
    public float ElapsedTime;
    public EntityCommandBuffer.ParallelWriter ECB;
    
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in StopRotatingComponent stopRotating, ref URPMaterialPropertyBaseColor baseColor)
    {
        var purple = new float4(1, 0, 1, 1);
        var yellow = new float4(1, 1, 0, 1);

        baseColor.Value = new float4(1, 1, 1, 1);

        if (ElapsedTime <= stopRotating.TimeToStop) // not yet time to stop
        {
            baseColor.Value = yellow;
            return;
        }
        
        ECB.SetComponentEnabled<RotationDataComponent>(chunkIndex, entity, false);
        baseColor.Value = purple;
    }
}
