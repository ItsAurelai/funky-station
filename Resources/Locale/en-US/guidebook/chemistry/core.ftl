# SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
# SPDX-FileCopyrightText: 2023 TomaszKawalec <40093912+TK-A369@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 Flesh <62557990+PolterTzi@users.noreply.github.com>
# SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
# SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
#
# SPDX-License-Identifier: MIT

guidebook-reagent-effect-description =
    {$chance ->
        [1] { $effect }
        *[other] Has a { NATURALPERCENT($chance, 2) } chance to { $effect }
    }{ $conditionCount ->
        [0] .
        *[other] {" "}when { $conditions }.
    }

guidebook-reagent-name = [bold][color={$color}]{CAPITALIZE($name)}[/color][/bold]
guidebook-reagent-recipes-header = Recipe
guidebook-reagent-recipes-reagent-display = [bold]{$reagent}[/bold] \[{$ratio}\]
guidebook-reagent-sources-header = Sources
guidebook-reagent-sources-ent-wrapper = [bold]{$name}[/bold] \[1\]
guidebook-reagent-sources-gas-wrapper = [bold]{$name} (gas)[/bold] \[1\]
guidebook-reagent-effects-header = Effects
guidebook-reagent-effects-metabolism-group-rate = [bold]{$group}[/bold] [color=gray]({$rate} units per second)[/color]
guidebook-reagent-plant-metabolisms-header = Plant Metabolism
guidebook-reagent-plant-metabolisms-rate = [bold]Plant Metabolism[/bold] [color=gray](1 unit every 3 seconds as base)[/color]
guidebook-reagent-physical-description = [italic]Seems to be {$description}.[/italic]
guidebook-reagent-recipes-mix-info = {$minTemp ->
    [0] {$hasMax ->
            [true] {CAPITALIZE($verb)} below {NATURALFIXED($maxTemp, 2)}K
            *[false] {CAPITALIZE($verb)}
        }
    *[other] {CAPITALIZE($verb)} {$hasMax ->
            [true] between {NATURALFIXED($minTemp, 2)}K and {NATURALFIXED($maxTemp, 2)}K
            *[false] above {NATURALFIXED($minTemp, 2)}K
        }
}
