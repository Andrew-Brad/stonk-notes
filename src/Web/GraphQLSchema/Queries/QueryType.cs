namespace StonkNotes.Web.GraphQLSchema;

public class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Authorize();
        descriptor
            .Field(f => f.GetDayNote(default!, default!))
            .Type<DayNoteType>()
            .Argument("id", a => a.Type<IntType>());
    }
}
