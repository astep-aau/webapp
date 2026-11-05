<script lang="ts">
  import { onMount } from "svelte";
  import "leaflet/dist/leaflet.css";

  let mapElement!: HTMLDivElement;

  onMount(async () => {
    const leaflet = await import("leaflet");

    const map = leaflet.map(mapElement).setView([39.9042, 116.4074], 10);

    leaflet
      .tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png")
      .addTo(map);

    for (let page = 0; ; page++) {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/edges?page=${page}`);
      if (!response.ok) {
        throw new Error(`API error: ${response.status}`);
      }
      if (response.status === 204) break;

      const edges = await response.json();
      leaflet.geoJSON(edges).addTo(map);
    }
  });
</script>

<h1>Beijing China</h1>

<div class="map" bind:this={mapElement}></div>

<style>
  .map {
    width: 100%;
    height: 700px;
  }
</style>
