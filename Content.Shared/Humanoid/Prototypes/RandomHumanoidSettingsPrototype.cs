// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Moony <moony@hellomouse.net>
// SPDX-FileCopyrightText: 2024 Verm <32827189+Vermidia@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Humanoid.Prototypes;

/// <summary>
///     This is what is used to change a humanoid spawned by RandomHumanoidSystem in Content.Server.
/// </summary>
[Prototype]
public sealed partial class RandomHumanoidSettingsPrototype : IPrototype, IInheritingPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;

    [ParentDataField(typeof(PrototypeIdArraySerializer<RandomHumanoidSettingsPrototype>))]
    public string[]? Parents { get; private set; }

    [AbstractDataField]
    [NeverPushInheritance]
    public bool Abstract { get; private set; }

    /// <summary>
    ///     Whether the humanoid's name should take from the randomized profile or not.
    /// </summary>
    [DataField("randomizeName")]
    public bool RandomizeName { get; private set; } = true;

    /// <summary>
    ///     Species that will be ignored by the randomizer.
    /// </summary>
    [DataField("speciesBlacklist")]
    public HashSet<string> SpeciesBlacklist { get; private set; } = new();

    /// <summary>
    ///     Extra components to add to this entity.
    /// </summary>
    [DataField]
    [AlwaysPushInheritance]
    public ComponentRegistry? Components { get; private set; }
}
