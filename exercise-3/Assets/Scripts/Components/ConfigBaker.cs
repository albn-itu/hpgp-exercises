using Unity.Entities;
using UnityEngine;

class ConfigBaker : MonoBehaviour
{
    public int spawnCount = 10;
    public GameObject prefab;
    
    class Baker : Baker<ConfigBaker>
    {
        public override void Bake(ConfigBaker authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ConfigComponent
            {
                spawnCount = authoring.spawnCount,
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

struct ConfigComponent : IComponentData
{
    public int spawnCount;
    public Entity prefab;
}