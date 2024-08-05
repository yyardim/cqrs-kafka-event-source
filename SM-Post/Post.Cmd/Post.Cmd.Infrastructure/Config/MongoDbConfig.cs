namespace Post.Cmd.Infrastructure.Config;

public class MongoDbConfig
{
    public required string ConnectionString { get; set; }
    public required string Database { get; set; }
    public required string Collection { get; set; }
}