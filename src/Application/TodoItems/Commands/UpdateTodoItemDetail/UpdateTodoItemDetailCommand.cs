using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;

namespace Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;

public record UpdateTodoItemDetailCommand : IRequest
{
    public int Id { get; init; }

    public int ListId { get; init; }

    public PriorityLevel Priority { get; init; }

    public string? Note { get; init; }

    public List<string>? Tags { get; init; }
}

public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoItemDetailCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(e => e.Id == request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        if (request.Tags != null && request.Tags.Any())
        {
            entity.Tags ??= new List<Tag>();

            var newTags = request.Tags.Where(tag => !entity.Tags.Any(et => et.Title == tag));

            foreach (var newTag in newTags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Title == newTag);
                if (tag == null)
                    tag = new Tag { Title = newTag };

                entity.Tags.Add(tag);
            }

            var removedTagIds = entity.Tags.Where(et => !request.Tags.Any(tag => tag == et.Title)).Select(t => t.Id);
            if (removedTagIds != null && removedTagIds.Any())
            {
                var removedTags = await _context.Tags.Where(t => removedTagIds.Contains(t.Id)).ToListAsync();
                foreach (var removedTag in removedTags)
                {
                    entity.Tags.Remove(removedTag);
                }
            }
        }
        else
        {
            entity.Tags = new List<Tag>();
        }

        entity.ListId = request.ListId;
        entity.Priority = request.Priority;
        entity.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
