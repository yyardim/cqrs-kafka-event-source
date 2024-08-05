using System;
using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class RemoveCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public required string Username { get; set; }
}