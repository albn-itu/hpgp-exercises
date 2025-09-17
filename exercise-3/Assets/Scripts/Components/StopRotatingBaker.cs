using Unity.Entities;
using UnityEngine;

class StopRotatingBaker : MonoBehaviour
{
    public float timeToStop = 2f;
    
    class Baker : Baker<StopRotatingBaker>
    {
        public override void Bake(StopRotatingBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new StopRotatingComponent
            {
                TimeToStop = authoring.timeToStop
            });
        }
    }
}

internal struct StopRotatingComponent : IComponentData
{
    public float TimeToStop;
}