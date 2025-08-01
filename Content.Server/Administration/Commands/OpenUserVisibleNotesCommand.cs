// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Riggle <27156122+RigglePrime@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Brandon Hu <103440971+Brandon-Huu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Notes;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AnyCommand]
public sealed class OpenUserVisibleNotesCommand : IConsoleCommand
{
    [Dependency] private readonly IConfigurationManager _configuration = default!;
    [Dependency] private readonly IAdminNotesManager _notes = default!;

    public const string CommandName = "adminremarks";

    public string Command => CommandName;
    public string Description => Loc.GetString("admin-remarks-command-description");
    public string Help => $"Usage: {Command}";

    public async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (!_configuration.GetCVar(CCVars.SeeOwnNotes))
        {
            shell.WriteError(Loc.GetString("admin-remarks-command-error"));
            return;
        }

        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        await _notes.OpenUserNotesEui(player);
    }
}
