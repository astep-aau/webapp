using Npgsql;

public static class GetData
{
    public static void Map(WebApplication app)
    {
        var configuration = app.Configuration;
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = configuration["POSTGRES_HOST"] ?? throw new InvalidOperationException("POSTGRES_HOST is required."),
            Port = int.Parse(configuration["POSTGRES_PORT"] ?? throw new InvalidOperationException("POSTGRES_PORT is required.")),
            Database = configuration["POSTGRES_DB"] ?? throw new InvalidOperationException("POSTGRES_DB is required."),
            Username = configuration["POSTGRES_USER"] ?? throw new InvalidOperationException("POSTGRES_USER is required."),
            Password = configuration["POSTGRES_PASSWORD"] ?? throw new InvalidOperationException("POSTGRES_PASSWORD is required.")
        }.ConnectionString;

        app.MapGet(
            "/api/edges",
            async (int page) =>
            {
                if (page < 0) return Results.BadRequest();

                await using var connection = new NpgsqlConnection(connectionString);

                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(
                    """
                    SELECT ST_AsGeoJSON(ST_Collect(geometry))
                    FROM (
                        SELECT geometry FROM public.osm_edges
                        ORDER BY fid
                        LIMIT 5000 OFFSET @offset
                    ) AS edges;
                    """,
                    connection
                );

                command.Parameters.AddWithValue("offset", (long)page * 5000);
                var geoJson = await command.ExecuteScalarAsync() as string;

                if (geoJson is null) return Results.NoContent();
                return Results.Content(geoJson, "application/geo+json");
            }
        );
    }
}
