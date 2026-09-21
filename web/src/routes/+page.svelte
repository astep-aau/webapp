<script lang="ts">
  import { onMount } from "svelte";
  import "leaflet/dist/leaflet.css";

  let mapElement!: HTMLDivElement;

  onMount(async () => {
    const leaflet = await import("leaflet");

    const map = leaflet.map(mapElement).setView([30.5, 104], 10);

    leaflet
      .tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png")
      .addTo(map);

    const response = await fetch("http://localhost:5260/api/edges");
    if (!response.ok) {
      throw new Error(`API error {response.status}: {response.Error}`);
    }

    const edges = await response.json();

    leaflet.geoJSON(edges).addTo(map);
  });
</script>

<h1>Chengdu China</h1>

<div class="map" bind:this={mapElement}></div>

<style>
  .map {
    width: 100%;
    height: 700px;
  }
</style>
