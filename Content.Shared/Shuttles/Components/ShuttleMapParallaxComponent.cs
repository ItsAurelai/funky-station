// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Shuttles.Components;

/// <summary>
/// Shows a parallax background on the shuttle map console.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ShuttleMapParallaxComponent : Component
{
    public static readonly ResPath FallbackTexture = new ResPath("/Textures/Parallaxes/space_map2.png");

    // TODO: This should ideally be shared with parallax stuff to avoid duplication, for now it's just a texture
    [DataField, AutoNetworkedField]
    public ResPath TexturePath;
}
