using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

public class MainSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    float m_CurrentTime;
    float m_LastSpawnedTime;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<PlayerData>()
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((int entityInQueryIndex, ref Rotation rotation, in PlayerData player, in LocalToWorld location) =>
            {
                var instance = commandBuffer.Instantiate(entityInQueryIndex, player.BulletPrefab);

                var position = math.transform(location.Value, (float3)Vector3.forward);
                commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = position });
                commandBuffer.SetComponent(entityInQueryIndex, instance, new Rotation { Value = rotation.Value });
            })
            .ScheduleParallel();

        Entities
            .WithAll<PlayerData>()
            .ForEach((ref Rotation rotation, in PlayerData player) =>
            {
                rotation.Value = math.mul(
                    math.normalize(rotation.Value),
                    quaternion.AxisAngle(math.up(), 2.0f * deltaTime));
            })
            .ScheduleParallel();

        Entities
            .WithAll<BulletData>()
            .ForEach((ref Translation trans, ref Rotation rot, in BulletData bullet) =>
            {
                trans.Value += math.rotate(rot.Value, (float3)Vector3.forward * bullet.m_Speed * deltaTime);
            })
            .ScheduleParallel();


        m_CurrentTime += deltaTime;
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
