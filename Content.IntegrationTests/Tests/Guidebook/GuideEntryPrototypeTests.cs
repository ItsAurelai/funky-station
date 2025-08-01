// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Guidebook;
using Content.Client.Guidebook.Richtext;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;
using System.Linq;
using Content.Shared.Guidebook;

namespace Content.IntegrationTests.Tests.Guidebook;

[TestFixture]
[TestOf(typeof(GuidebookSystem))]
[TestOf(typeof(GuideEntryPrototype))]
[TestOf(typeof(DocumentParsingManager))]
public sealed class GuideEntryPrototypeTests
{
    [Test]
    public async Task ValidatePrototypeContents()
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings { Connected = true });
        var client = pair.Client;
        await client.WaitIdleAsync();
        var protoMan = client.ResolveDependency<IPrototypeManager>();
        var resMan = client.ResolveDependency<IResourceManager>();
        var parser = client.ResolveDependency<DocumentParsingManager>();
        var prototypes = protoMan.EnumeratePrototypes<GuideEntryPrototype>().ToList();

        foreach (var proto in prototypes)
        {
            await client.WaitAssertion(() =>
            {
                using var reader = resMan.ContentFileReadText(proto.Text);
                var text = reader.ReadToEnd();
                Assert.That(parser.TryAddMarkup(new Document(), text), $"Failed to parse guidebook: {proto.Id}");
            });

            // Avoid styleguide update limit
            await client.WaitRunTicks(1);
        }

        await pair.CleanReturnAsync();
    }
}
