using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

public class RandomDataBaker : MonoBehaviour
{
    public float duration = 2f;
    
    class Baker : Baker<RandomDataBaker>
    {
        public override void Bake(RandomDataBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomDataComponent
            {
                Generator = new Random(1),
                Initialized = false,
                Duration = authoring.duration,
            });
        }
    }
}

struct RandomDataComponent : IComponentData
{
    public Random Generator;
    public bool Initialized;
    public float InitializedTime;
    public float Duration;
}