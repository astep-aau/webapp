from sqlalchemy import create_engine
import osmnx as ox

# Sources:
# https://github.com/gboeing/osmnx-examples/blob/main/notebooks/03-graph-place-queries.ipynb
# https://osmnx.readthedocs.io/en/stable/user-reference.html
# https://geopandas.org/en/stable/docs/reference/api/geopandas.GeoDataFrame.to_postgis.html

ox.settings.log_console = True

G = ox.graph.graph_from_place("Chengdu, China", network_type="drive")

gdf_nodes, gdf_edges = ox.convert.graph_to_gdfs(G)

engine = create_engine("postgresql+psycopg2://postgres:postgres@localhost:5432/astep")

gdf_nodes.to_postgis("osm_nodes", engine, if_exists="replace")
gdf_edges.to_postgis("osm_edges", engine, if_exists="replace")
