using Unity.Mathematics;
using Unity.Transforms;

namespace TeaFramework {
    internal static partial class MathCollection {
        public static float3 GetEuler(this quaternion q) => math.Euler(q);

        public static quaternion SetEulerYaw(this float yaw) => quaternion.EulerXYZ(0f, yaw, 0f);

        public static float SmoothFactor(this float deltaTime, float timeConstant = 0.02f) =>
            1f - math.exp(-deltaTime / math.max(1e-4f, timeConstant));

        public static float3 GetDir(this LocalTransform start, LocalTransform target) =>
            start.Position - target.Position;

        public static float3 GetDir(this LocalToWorld start, LocalToWorld target) => start.Position - target.Position;

        public static float3 GetDir(this LocalToWorld start, LocalTransform target) => start.Position - target.Position;

        public static float3 GetDir(this LocalTransform start, LocalToWorld target) => start.Position - target.Position;

        public static bool GetDistanceAndDot(LocalTransform player,
            LocalTransform target,
            out float distanceSqr,
            out float dot) {
            var dir = player.GetDir(target);
            distanceSqr = math.lengthsq(dir);
            dot = math.dot(player.Forward(), math.normalize(dir));

            return dot >= 0f;
        }

        public static bool GetDistanceAndDot(LocalToWorld player,
            LocalToWorld target,
            out float distanceSqr,
            out float dot) {
            var dir = player.GetDir(target);
            distanceSqr = math.lengthsq(dir);
            dot = math.dot(player.Forward, math.normalize(dir));

            return dot >= 0f;
        }
    }
}