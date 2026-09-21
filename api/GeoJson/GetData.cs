using Npgsql;

public static class GetData
{
    public static void Map(WebApplication app)
    {
        var connectionString = app.Configuration.GetConnectionString("Postgres")!;

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
