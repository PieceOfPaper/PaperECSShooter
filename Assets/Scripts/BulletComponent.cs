using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BulletData : IComponentData
{
    public float m_Speed;
}

public class BulletComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] float m_Speed = 1.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new BulletData
        {
            m_Speed = m_Speed,
        };
        dstManager.AddComponentData(entity, data);
    }
}
