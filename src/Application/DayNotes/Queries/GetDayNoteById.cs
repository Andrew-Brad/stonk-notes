using StonkNotes.Application.Common.Interfaces;
using StonkNotes.Application.Common.Models;
using StonkNotes.Application.Common.Security;

namespace StonkNotes.Application.DayNotes.Queries.GetDayNoteById;

//[Authorize]
public record GetDayNoteByIdQuery(int Id) : IRequest<DayNoteDto>;

public class GetDayNoteByIdQueryHandler : IRequestHandler<GetDayNoteByIdQuery, DayNoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDayNoteByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DayNoteDto> Handle(GetDayNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.DayNotes
            .SingleAsync(x => x.Id == request.Id, cancellationToken);
        return _mapper.Map<DayNoteDto>(entity);
    }
}
