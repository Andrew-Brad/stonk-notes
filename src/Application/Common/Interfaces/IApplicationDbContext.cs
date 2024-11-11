using StonkNotes.Domain.Entities;

namespace StonkNotes.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<DayNote> DayNotes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
