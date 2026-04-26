namespace fitnes.Domain.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string PostgresConnection { get; set; }
    public required string RedisConnection { get; set; }
    public required string MongoConnection { get; set; }
    public required string MongoDatabaseName { get; set; }
}
