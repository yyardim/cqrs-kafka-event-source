using System;
using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class EditCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public required string Comment { get; set; }
    public required string Username { get; set; }
}