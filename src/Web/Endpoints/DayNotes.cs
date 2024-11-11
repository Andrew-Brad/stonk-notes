using StonkNotes.Application.DayNotes.Commands.CreateDayNote;
//using StonkNotes.Application.TodoLists.Commands.DeleteTodoList;
//using StonkNotes.Application.TodoLists.Commands.UpdateTodoList;
//using StonkNotes.Application.TodoLists.Queries.GetTodos;

namespace StonkNotes.Web.Endpoints;

public class DayNotes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            //.RequireAuthorization()
            //.MapGet(GetTodoLists)
            .MapPost(CreateDayNote);
        //.MapPut(UpdateTodoList, "{id}")
        //.MapDelete(DeleteTodoList, "{id}");
    }

    // public Task<TodosVm> GetTodoLists(ISender sender)
    // {
    //     return  sender.Send(new GetTodosQuery());
    // }

    public Task<int> CreateDayNote(ISender sender, CreateDayNoteCommand command)
    {
        return sender.Send(command);
    }

    // public async Task<IResult> UpdateTodoList(ISender sender, int id, UpdateTodoListCommand command)
    // {
    //     if (id != command.Id) return Results.BadRequest();
    //     await sender.Send(command);
    //     return Results.NoContent();
    // }
    //
    // public async Task<IResult> DeleteTodoList(ISender sender, int id)
    // {
    //     await sender.Send(new DeleteTodoListCommand(id));
    //     return Results.NoContent();
    // }
}
