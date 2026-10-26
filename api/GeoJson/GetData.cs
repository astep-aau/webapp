using Npgsql;

public static class GetData
{
    public static void Map(WebApplication app)
    {
        var configuration = app.Configuration;
        var connectionString = configuration.GetConnectionString("Postgres");

        if (configuration["POSTGRES_HOST"] is { Length: > 0 } host)
        {
            connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Database = configuration["POSTGRES_DB"],
                Username = configuration["POSTGRES_USER"],
                Password = configuration["POSTGRES_PASSWORD"]
            }.ConnectionString;
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Postgres connection settings are required.");
        }

        app.MapGet(
            "/api/edges",
            async () =>
            {
                await using var connection = new NpgsqlConnection(connectionString);

                await connection.OpenAsync();

                await using var command = new NpgsqlCommand(
                    """
                    SELECT ST_AsGeoJSON(ST_Collect(geometry))
                    FROM public.osm_edges;
                    """,
                    connection
                );

                var geoJson = (string?)await command.ExecuteScalarAsync();

                return Results.Content(geoJson ?? "No GeoJson...");
            }
        );
    }
}
