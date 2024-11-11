using Snapshooter.NUnit;
using StonkNotes.Application.TodoLists.Queries.GetTodos;
using StonkNotes.Domain.Entities;
using StonkNotes.Domain.ValueObjects;

namespace StonkNotes.Application.FunctionalTests.TodoLists.Queries;

using static Testing;

public class GetTodosTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnPriorityLevels()
    {
        await RunAsDefaultUserAsync();

        var query = new GetTodosQuery();

        var result = await SendAsync(query);

        result.PriorityLevels.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldReturnAllListsAndItems()
    {
        await RunAsDefaultUserAsync();

        await AddAsync(new TodoList
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            Items =
                    {
                        new TodoItem { Title = "Apples", Done = true },
                        new TodoItem { Title = "Milk", Done = true },
                        new TodoItem { Title = "Bread", Done = true },
                        new TodoItem { Title = "Toilet paper" },
                        new TodoItem { Title = "Pasta" },
                        new TodoItem { Title = "Tissues" },
                        new TodoItem { Title = "Tuna" }
                    }
        });

        var query = new GetTodosQuery();

        var result = await SendAsync(query);

        result.Lists.Should().HaveCount(1);
        result.Lists.First().Items.Should().HaveCount(7);
    }

    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetTodosQuery();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // testing only to ensure the simplest possible thing works
    [Test]
    public async Task CreateSampleSnapshotTest()
    {
        // arrange
        await Task.CompletedTask;
        //var serviceClient = new ServiceClient();

        // act
        ShortText text = ShortText.From(Guid.Parse("2292F21C-8501-4771-A070-C79C7C7EF451").ToString());

        // assert
        Snapshot.Match(text);
    }
}
