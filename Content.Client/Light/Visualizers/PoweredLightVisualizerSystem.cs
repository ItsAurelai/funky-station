// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 deathride58 <deathride58@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Light;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;

namespace Content.Client.Light.Visualizers;

public sealed class PoweredLightVisualizerSystem : VisualizerSystem<PoweredLightVisualsComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PoweredLightVisualsComponent, AnimationCompletedEvent>(OnAnimationCompleted);
    }

    protected override void OnAppearanceChange(EntityUid uid, PoweredLightVisualsComponent comp, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!AppearanceSystem.TryGetData<PoweredLightState>(uid, PoweredLightVisuals.BulbState, out var state, args.Component))
            return;

        if (comp.SpriteStateMap.TryGetValue(state, out var spriteState))
            args.Sprite.LayerSetState(PoweredLightLayers.Base, spriteState);

        if (args.Sprite.LayerExists(PoweredLightLayers.Glow))
        {
            if (TryComp<PointLightComponent>(uid, out var light))
            {
                args.Sprite.LayerSetColor(PoweredLightLayers.Glow, light.Color);
            }

            args.Sprite.LayerSetVisible(PoweredLightLayers.Glow, state == PoweredLightState.On);
        }

        SetBlinkingAnimation(
            uid,
            state == PoweredLightState.On
            && (AppearanceSystem.TryGetData<bool>(uid, PoweredLightVisuals.Blinking, out var isBlinking, args.Component) && isBlinking),
            comp
        );
    }

    /// <summary>
    /// Loops the blinking animation until the light should stop blinking.
    /// </summary>
    private void OnAnimationCompleted(EntityUid uid, PoweredLightVisualsComponent comp, AnimationCompletedEvent args)
    {
        if (!TryComp<AnimationPlayerComponent>(uid, out var animationPlayer))
            return;
        if (args.Key != PoweredLightVisualsComponent.BlinkingAnimationKey)
            return;
        if(!comp.IsBlinking)
            return;

        AnimationSystem.Play((uid, animationPlayer), BlinkingAnimation(comp), PoweredLightVisualsComponent.BlinkingAnimationKey);
    }

    /// <summary>
    /// Sets whether or not the given light should be blinking.
    /// Triggers or clears the blinking animation of the state changes.
    /// </summary>
    private void SetBlinkingAnimation(EntityUid uid, bool shouldBeBlinking, PoweredLightVisualsComponent comp)
    {
        if (shouldBeBlinking == comp.IsBlinking)
            return;

        comp.IsBlinking = shouldBeBlinking;

        var animationPlayer = EnsureComp<AnimationPlayerComponent>(uid);
        if (shouldBeBlinking)
        {
            AnimationSystem.Play((uid, animationPlayer), BlinkingAnimation(comp), PoweredLightVisualsComponent.BlinkingAnimationKey);
        }
        else if (AnimationSystem.HasRunningAnimation(uid, animationPlayer, PoweredLightVisualsComponent.BlinkingAnimationKey))
        {
            AnimationSystem.Stop(uid, animationPlayer, PoweredLightVisualsComponent.BlinkingAnimationKey);
        }
    }

    /// <summary>
    /// Generates a blinking animation.
    /// Essentially just flashes the light off and on over a random time interval.
    /// The resulting animation is looped indefinitely until the comp is set to stop blinking.
    /// </summary>
    private Animation BlinkingAnimation(PoweredLightVisualsComponent comp)
    {
        var randomTime = MathHelper.Lerp(comp.MinBlinkingAnimationCycleTime, comp.MaxBlinkingAnimationCycleTime, _random.NextFloat());
        var blinkingAnim = new Animation()
        {
            Length = TimeSpan.FromSeconds(randomTime),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
                {
                    ComponentType = typeof(PointLightComponent),
                    InterpolationMode = AnimationInterpolationMode.Nearest,
                    Property = nameof(PointLightComponent.AnimatedEnable),
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(false, 0),
                        new AnimationTrackProperty.KeyFrame(true, 1)
                    }
                },
                new AnimationTrackSpriteFlick()
                {
                    LayerKey = PoweredLightLayers.Base,
                    KeyFrames =
                    {
                        new AnimationTrackSpriteFlick.KeyFrame(comp.SpriteStateMap[PoweredLightState.Off], 0),
                        new AnimationTrackSpriteFlick.KeyFrame(comp.SpriteStateMap[PoweredLightState.On], 0.5f)
                    }
                }
            }
        };

        if (comp.BlinkingSound != null)
        {
            var sound = _audio.ResolveSound(comp.BlinkingSound);
            blinkingAnim.AnimationTracks.Add(new AnimationTrackPlaySound()
            {
                KeyFrames =
                {
                    new AnimationTrackPlaySound.KeyFrame(sound, 0.5f)
                }
            });
        }

        return blinkingAnim;
    }
}
