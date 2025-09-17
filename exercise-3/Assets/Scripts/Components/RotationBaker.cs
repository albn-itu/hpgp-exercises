using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class RotationBaker : MonoBehaviour
{
    public float rotation;
    
    class Baker : Baker<RotationBaker>
    {
        public override void Bake(RotationBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RotationDataComponent
            {
                Rotation = authoring.rotation
            });
        }
    }
}

struct RotationDataComponent : IComponentData, IEnableableComponent
{
    public float Rotation;
}
