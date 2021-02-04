using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PlayerData : IComponentData
{
    public Entity BulletPrefab;
}

public class PlayerComponent : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject BulletPrefab;


    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BulletPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new PlayerData
        {
            BulletPrefab = conversionSystem.GetPrimaryEntity(BulletPrefab),
        };
        dstManager.AddComponentData(entity, data);
    }
}
