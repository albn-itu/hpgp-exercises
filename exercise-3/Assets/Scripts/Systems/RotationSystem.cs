using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RotationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        state.Dependency = new RotateCubeJob
        {
            DeltaTime = deltaTime
        }.Schedule(state.Dependency);
    }
}

[BurstCompile]
internal partial struct RotateCubeJob : IJobEntity
{
    public float DeltaTime;

    private void Execute(ref LocalTransform transform, in RotationDataComponent rotationData)
    {
        var newRotation = quaternion.RotateZ(rotationData.Rotation * math.TORADIANS * DeltaTime);
        transform.Rotation = math.mul(transform.Rotation, newRotation);
    }
}
