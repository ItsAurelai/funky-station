// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Gameplay;
using Content.Client.Mapping;
using Robust.Client.State;

namespace Content.IntegrationTests.Tests;

[TestFixture]
public sealed class MappingEditorTest
{
    [Test]
    public async Task StopHardCodingWidgetsJesusChristTest()
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings
        {
            Connected = true
        });
        var client = pair.Client;
        var state = client.ResolveDependency<IStateManager>();

        await client.WaitPost(() =>
        {
            Assert.DoesNotThrow(() =>
            {
                state.RequestStateChange<MappingState>();
            });
        });

        // arbitrary short time
        await client.WaitRunTicks(30);

        await client.WaitPost(() =>
        {
            Assert.DoesNotThrow(() =>
            {
                state.RequestStateChange<GameplayState>();
            });
        });

        await pair.CleanReturnAsync();
    }
}
