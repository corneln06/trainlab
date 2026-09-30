import * as maplibregl from "maplibre-gl";
import "maplibre-gl/dist/maplibre-gl.css";
import "./style.css";

type TrackFeature = {
    type: "Feature";
    properties: Record<string, unknown>;
    geometry: {
        type: "LineString";
        coordinates: [number, number][];
    };
};

const map = new maplibregl.Map({
    container: "app",
    style: "https://tiles.openfreemap.org/styles/liberty",
    center: [9.85, 55.86],
    zoom: 10,
});

map.on("load", async () => {
    const response = await fetch("/track.geojson");
    const track: TrackFeature = await response.json();

    map.addSource("track", { type: "geojson", data: track });

    map.addLayer({
        id: "track-line",
        type: "line",
        source: "track",
        layout: { "line-join": "round", "line-cap": "round" },
        paint: { "line-color": "#e11d48", "line-width": 4 },
    });
    
    let minLon = Infinity;
    let minLat = Infinity;
    let maxLon = -Infinity;
    let maxLat = -Infinity;

    for (const [lon, lat] of track.geometry.coordinates) {
        minLon = Math.min(minLon, lon);
        minLat = Math.min(minLat, lat);
        maxLon = Math.max(maxLon, lon);
        maxLat = Math.max(maxLat, lat);
    }

    map.fitBounds(
        [
            [minLon, minLat],
            [maxLon, maxLat],
        ],
        { padding: 40 },
    );
});