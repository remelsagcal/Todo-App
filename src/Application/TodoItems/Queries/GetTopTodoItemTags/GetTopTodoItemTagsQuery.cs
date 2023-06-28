using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTopTodoItemTagsQuery : IRequest<List<string>>
{
    public int ResultSize { get; init; } = 5;
}

public class GetTopTodoItemTagsQueryHandler : IRequestHandler<GetTopTodoItemTagsQuery, List<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTopTodoItemTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<string>> Handle(GetTopTodoItemTagsQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.Tags
            .OrderByDescending(t => t.Items.Count)
            .Take(request.ResultSize)
            .Select(x => x.Title)
            .ToListAsync();

        return result;
    }
}
