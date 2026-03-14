using System;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace TeaFramework {
    public static partial class HelperCollection {
        internal static async Task<bool> FadeScreenAsync(this Image fadeImage,
            EFadeType fadeType,
            float loadDuration,
            float dt,
            Action onDone = null) {
            try {
                if (fadeImage is null) return false;

                var targetAlpha = fadeType switch {
                    EFadeType.FadeOut => 0,
                    EFadeType.FadeIn => 1,
                    _ => fadeImage.color.a
                };

                var currentAlpha = fadeImage.color.a;
                float time = 0;

                while (time <= loadDuration) {
                    time += dt;
                    var alpha = math.lerp(currentAlpha, targetAlpha, time / loadDuration);
                    if (fadeImage != null)
                        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
                    await Awaitable.EndOfFrameAsync();
                }

                if (fadeImage != null)
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
                onDone?.Invoke();
                return true;
            }
            catch (Exception e) {
                Debug.LogException(e);
            }

            return false;
        }
    }
}