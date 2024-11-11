using System.Text.Json.Nodes;
using Snapshooter.NUnit;
using StonkNotes.Application.Common.Exceptions;
using StonkNotes.Application.DayNotes.Commands.CreateDayNote;
using StonkNotes.Application.FunctionalTests.WebIntegration;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.FunctionalTests.DayNotes.Commands;

using static Testing;

public class CreateDayNoteCommandTests : BaseTestFixture
{
    // A test philosophy is implicit here: since this is a functional test, the test case
    // is named after the business behavior, but in terms of code execution, we are standing up
    // a full integration-style test when a CommandValidator unit test would do. TBD if this changes.
    // notable scenario is EntryDate which would theoretically require a db roundtrip
    [Test]
    public async Task ShouldRequireKnownEnums()
    {
        // Arrange
        var command = new CreateDayNoteCommand(
            DateOnly.MinValue,
            "foo",
            "bar",
            MarketCondition.Unknown,
            Mood.Unknown,
            MarketVolatility.Unknown);

        // Act
        var task = SendAsync(command);

        // Assert
        await FluentActions.Invoking(async () => await task)
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldRequireValidSummaryText()
    {
        // Arrange
        var command = new CreateDayNoteCommand(
            DateOnly.MinValue,
            string.Empty,
            "bar",
            MarketCondition.Bearish,
            Mood.Neutral,
            MarketVolatility.High);

        // Act
        var task = SendAsync(command);

        // Assert
        await FluentActions.Invoking(async () => await task)
            .Should()
            .ThrowAsync<ValidationException>();
        // Structures Error format for client consumption not yet supported:
        //.Exception
        //.WithMessage($"Text summaries must be 1,024 characters or less.");
    }

    [Test]
    public async Task ShouldCreateDayNote()
    {
        // Arrange
        var userId = await RunAsUserAsync("foo20", "P@ssword!1", []);

        // lang=gql
        const string document =
            """
            mutation CreateDayNote($date: Date!, $noteText: String!, $summaryText: String!, $marketCondition: MarketCondition!, $marketVolatility: MarketVolatility!, $mood: Mood!) {
              createDayNote(
                input: {
                  dayNoteInput: {
                    entryDate: $date
                    noteText: $noteText
                    summaryText: $summaryText
                    marketCondition: $marketCondition
                    marketVolatility: $marketVolatility
                    mood: $mood
                  }
                }
              ) {
                addDayNotePayload {
                  id
                }
              }
            }
            """;
        var variables = new Dictionary<string, object>
        {
            ["date"] = "11/1/2024",
            ["noteText"] = "foo",
            ["summaryText"] = "bar",
            ["marketCondition"] = MarketCondition.Bearish.ToString().ToUpper(),
            ["marketVolatility"] = MarketVolatility.High.ToString().ToUpper(),
            ["mood"] = Mood.Neutral.ToString().ToUpper(),
        };
        var httpClient = await GetAuthenticatedGraphQLClient("foo20", "P@ssword!1");
        var request = new GraphQLRequest(document, variables);

        // Act
        var response = await httpClient.PostGraphQLRequest<Dictionary<string, object>>(request);

        // Assert
        response.HasErrors.Should().BeFalse();
        response.Errors.Should().BeNullOrEmpty();

        response.Data!["createDayNote"].Should().NotBeNull();
        JsonNode payload = JsonNode.Parse(response.Data["createDayNote"].ToString()!)!;
        payload!["addDayNotePayload"]!["id"]!.GetValue<int>().Should()
            .BePositive(because: " 4 entities are pre-seeded when the app runs");
        //response.Data.MatchSnapshot();
    }
}
