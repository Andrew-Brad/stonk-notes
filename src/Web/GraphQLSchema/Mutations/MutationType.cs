namespace StonkNotes.Web.GraphQLSchema;

public class MutationType : ObjectType<Mutation>
{
    protected override void Configure(IObjectTypeDescriptor<Mutation> descriptor)
    {
        descriptor.Authorize();
        descriptor.Field(f => f.CreateDayNote(default!, default!, default!));
        descriptor.Field(f => f.UpdateDayNote(default!, default!, default!));
    }
}
