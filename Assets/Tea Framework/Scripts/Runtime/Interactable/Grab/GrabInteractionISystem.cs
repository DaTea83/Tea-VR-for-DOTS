using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TeaFramework {
    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PostTransformSystemGroup))]
    [UpdateAfter(typeof(OverlapGrabInteractISystem))]
    public partial struct GrabInteractionISystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (buffer, data, active, ltw, entity)
                     in SystemAPI.Query<DynamicBuffer<GrabberEntitiesIBuffer>, RefRO<GrabberTriggerIData>,
                         RefRW<GrabberActiveIData>, RefRO<LocalToWorld>>().WithEntityAccess()) {
                if (data.ValueRO.Player == Entity.Null) continue;
                var lInput = SystemAPI.GetComponentRO<LHandInputIData>(data.ValueRO.Player);
                var rInput = SystemAPI.GetComponentRO<RHandInputIData>(data.ValueRO.Player);
                if (lInput.ValueRO.IsTracked == 0 && rInput.ValueRO.IsTracked == 0) continue;

                active.ValueRW.CurrentInput = data.ValueRO.GrabButton.GetButtonValues(lInput, rInput);
                active.ValueRW.PreviousInput = active.ValueRW.CurrentInput;

                if (buffer.Length == 0) continue;
                GetClosestInteractable(buffer, out var interact, out _);

                var iBuffer = SystemAPI.GetBuffer<InteractGrabIBuffer>(interact);
                var iLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(interact);
                var offset = ltw.ValueRO.GetDir(iLocalToWorld.ValueRO);

                if (!SystemAPI.HasComponent<InteractableNonPhysicsITag>(interact)) {
                    var iData = SystemAPI.GetComponentRO<PhysicsMassIData>(interact);
                    var iMass = SystemAPI.GetComponentRW<PhysicsMass>(interact);
                    var iVel = SystemAPI.GetComponentRW<PhysicsVelocity>(interact);
                    var iGravity = SystemAPI.GetComponentRW<PhysicsGravityFactor>(interact);

                    switch (data.ValueRO.InteractType) {
                        case EButtonInteract.Hold:

                            if (active.ValueRO.CurrentInput > data.ValueRO.Threshold && interact != Entity.Null) {
                                SetInteractable(active, iVel, iMass, iGravity, iBuffer, interact, entity, offset);
                                ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(interact, true);
                            }
                            else if (active.ValueRO.CurrentInput < data.ValueRO.Threshold) {
                                ReleaseInteractable(active, iData, iMass, iGravity, iBuffer, entity,
                                    out var currentInteract);
                                if (iBuffer.Length <= 0) {
                                    if (currentInteract == Entity.Null) return;
                                    ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(currentInteract, false);
                                }

                                active.ValueRW.InteractEntity = Entity.Null;
                            }

                            break;
                        case EButtonInteract.Toggle:

                            if (!(active.ValueRO.CurrentInput > 0.1f) || !(active.ValueRO.PreviousInput < 0.1f)) return;
                            if (active.ValueRO.InteractEntity == Entity.Null) {
                                SetInteractable(active, iVel, iMass, iGravity, iBuffer, interact, entity, offset);
                                ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(interact, true);
                            }
                            else {
                                ReleaseInteractable(active, iData, iMass, iGravity, iBuffer, entity,
                                    out var currentInteract);
                                if (iBuffer.Length <= 0) {
                                    if (currentInteract == Entity.Null) return;
                                    ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(currentInteract, false);
                                }

                                active.ValueRW.InteractEntity = Entity.Null;
                            }

                            break;
                        default:
                            break;
                    }
                }
                else {
                    switch (data.ValueRO.InteractType) {
                        case EButtonInteract.Hold:
                            if (active.ValueRO.CurrentInput > data.ValueRO.Threshold && interact != Entity.Null) {
                                if (active.ValueRO.InteractEntity != Entity.Null) return;
                                iBuffer.Add(new InteractGrabIBuffer {
                                    GrabberEntity = entity,
                                    Offset = offset
                                });
                                active.ValueRW.InteractEntity = interact;
                                ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(interact, true);
                            }
                            else {
                                var currentInteract = active.ValueRW.InteractEntity;
                                if (currentInteract == Entity.Null) return;
                                var cBuffer = SystemAPI.GetBuffer<InteractGrabIBuffer>(currentInteract);
                                for (var i = 0; i < cBuffer.Length; i++) {
                                    if (cBuffer[i].GrabberEntity != entity) continue;
                                    cBuffer.RemoveAt(i);
                                }

                                if (iBuffer.Length <= 0) {
                                    if (currentInteract == Entity.Null) return;
                                    ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(currentInteract, false);
                                }

                                active.ValueRW.InteractEntity = Entity.Null;
                            }

                            break;
                        case EButtonInteract.Toggle:
                            if (!(active.ValueRO.CurrentInput > 0.1f) || !(active.ValueRO.PreviousInput < 0.1f)) return;
                            if (active.ValueRO.InteractEntity == Entity.Null) {
                                if (active.ValueRO.InteractEntity != Entity.Null) return;
                                iBuffer.Add(new InteractGrabIBuffer {
                                    GrabberEntity = entity,
                                    Offset = offset
                                });
                                active.ValueRW.InteractEntity = interact;
                                ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(interact, true);
                            }
                            else {
                                var currentInteract = active.ValueRW.InteractEntity;
                                if (currentInteract == Entity.Null) return;
                                var cBuffer = SystemAPI.GetBuffer<InteractGrabIBuffer>(currentInteract);
                                for (var i = 0; i < cBuffer.Length; i++) {
                                    if (cBuffer[i].GrabberEntity != entity) continue;
                                    cBuffer.RemoveAt(i);
                                }

                                if (iBuffer.Length <= 0) {
                                    if (currentInteract == Entity.Null) return;
                                    ecb.SetComponentEnabled<InteractGrabFollowIEnableableTag>(currentInteract, false);
                                }

                                active.ValueRW.InteractEntity = Entity.Null;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        private static void GetClosestInteractable(DynamicBuffer<GrabberEntitiesIBuffer> buffer,
            out Entity entity,
            out float disSqr) {
            if (buffer.Length == 0) {
                entity = Entity.Null;
                disSqr = -1;
                return;
            }

            var best = buffer[0].Interactable;
            var bestDis = buffer[0].DisSqr;
            for (var i = 1; i < buffer.Length; i++) {
                // If DOT = 1, the object is right in front you
                // If DOT = -1, the object is behind you
                // Slight behind is fine, value too high is hard to register
                if (!(buffer[i].DisSqr < bestDis) || (buffer[i].Dot < -0.5f)) continue;
                best = buffer[i].Interactable;
                bestDis = buffer[i].DisSqr;
            }

            entity = best;
            disSqr = bestDis;
        }

        private static bool SetInteractable(RefRW<GrabberActiveIData> active,
            RefRW<PhysicsVelocity> vel,
            RefRW<PhysicsMass> mass,
            RefRW<PhysicsGravityFactor> gravity,
            DynamicBuffer<InteractGrabIBuffer> iBuffer,
            Entity interact,
            Entity entity,
            in float3 offset) {
            // If already holding return false
            if (active.ValueRO.InteractEntity != Entity.Null) return false;

            // Interact buffer add new grabber
            iBuffer.Add(new InteractGrabIBuffer {
                GrabberEntity = entity,
                Offset = offset
            });
            // Grabber add interactable
            active.ValueRW.InteractEntity = interact;

            // No need to set physics stuff if already set one time
            if (iBuffer.Length > 1) return true;
            gravity.ValueRW.Value = 0;
            mass.ValueRW.InverseMass = 0;
            mass.ValueRW.InverseInertia = float3.zero;
            vel.ValueRW.Linear = vel.ValueRW.Angular = float3.zero;

            return true;
        }

        private static bool ReleaseInteractable(RefRW<GrabberActiveIData> active,
            RefRO<PhysicsMassIData> data,
            RefRW<PhysicsMass> mass,
            RefRW<PhysicsGravityFactor> gravity,
            DynamicBuffer<InteractGrabIBuffer> iBuffer,
            Entity entity,
            out Entity currentInteract) {
            // Return if there are no interactable
            var interact = active.ValueRW.InteractEntity;
            currentInteract = interact;
            if (interact == Entity.Null) return false;

            // Loop through the interact buffer and remove the grabber
            for (var i = 0; i < iBuffer.Length; i++) {
                if (iBuffer[i].GrabberEntity != entity) continue;
                iBuffer.RemoveAt(i);
            }

            //If there are still got grabber no need to set physics back
            if (iBuffer.Length > 0) return true;
            gravity.ValueRW.Value = 1;
            mass.ValueRW.InverseMass = data.ValueRO.InverseMass;
            mass.ValueRW.InverseInertia = data.ValueRO.InverseInertia;

            return true;
        }
    }
}