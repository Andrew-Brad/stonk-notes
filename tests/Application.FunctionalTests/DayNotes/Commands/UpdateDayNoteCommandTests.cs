using System.Text.Json.Nodes;
using StonkNotes.Application.Common.Exceptions;
using StonkNotes.Application.DayNotes.Commands.UpdateDayNote;
using StonkNotes.Application.FunctionalTests.WebIntegration;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.FunctionalTests.DayNotes.Commands;

using static Testing;

public class UpdateDayNoteCommandTests : BaseTestFixture
{
    // A test philosophy is implicit here: since this is a functional test, the test case
    // is named after the business behavior, but in terms of code execution, we are standing up
    // a full integration-style test when a CommandValidator unit test would do. TBD if this changes.
    // notable scenario is EntryDate which would theoretically require a db roundtrip
    [Test]
    public async Task ShouldRequireKnownEnums()
    {
        // Arrange
        var command = new UpdateDayNoteCommand(
            1,
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
        var command = new UpdateDayNoteCommand(
            1,
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
    public async Task ShouldUpdateDayNote()
    {
        // Arrange
        var userId = await RunAsUserAsync(nameof(ShouldUpdateDayNote), "P@ssw0rd1!", []);
        var httpClient = await GetAuthenticatedGraphQLClient(nameof(ShouldUpdateDayNote), "P@ssw0rd1!");

        // lang=gql
        const string createDocument =
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
        var gqlVariables = new Dictionary<string, object>
        {
            ["date"] = "11/1/2024",
            ["noteText"] = "foo",
            ["summaryText"] = "bar",
            ["marketCondition"] = MarketCondition.Bearish.ToString().ToUpper(),
            ["marketVolatility"] = MarketVolatility.High.ToString().ToUpper(),
            ["mood"] = Mood.Neutral.ToString().ToUpper(),
        };

        var request = new GraphQLRequest(createDocument, gqlVariables);
        var response = await httpClient.PostGraphQLRequest<Dictionary<string, object>>(request);
        response.HasErrors.Should().BeFalse();
        response.Errors.Should().BeNullOrEmpty();
        JsonNode payload = JsonNode.Parse(response.Data!["createDayNote"].ToString()!)!;
        var id = payload!["addDayNotePayload"]!["id"]!.GetValue<int>();
        // lang=gql
        const string updateDocument =
            """
            mutation UpdateDayNote($id: Int!, $date: Date!, $noteText: String!, $summaryText: String!, $marketCondition: MarketCondition!, $marketVolatility: MarketVolatility!, $mood: Mood!) {
              updateDayNote(
                  dayNoteInput: {
                    id: $id
                    entryDate: $date
                    noteText: $noteText
                    summaryText: $summaryText
                    marketCondition: $marketCondition
                    marketVolatility: $marketVolatility
                    mood: $mood
                  }
                
              ) {
                  id
                }
              }
            """;

        // Act
        gqlVariables["id"] = id;
        gqlVariables["noteText"] = "note2";
        gqlVariables["marketCondition"] = MarketCondition.Bullish.ToString().ToUpper();
        var updateRequest = new GraphQLRequest(updateDocument, gqlVariables);
        var updateResponse = await httpClient.PostGraphQLRequest<Dictionary<string, object>>(updateRequest);

        // Assert
        updateResponse.HasErrors.Should().BeFalse();
        updateResponse.Errors.Should().BeNullOrEmpty();
        updateResponse.Data!["updateDayNote"].Should().NotBeNull();
        JsonNode updatePayload = JsonNode.Parse(updateResponse.Data["updateDayNote"].ToString()!)!;
        updatePayload["id"]!.GetValue<int>().Should().Be(id);
        //response.Data.MatchSnapshot();
    }
}
