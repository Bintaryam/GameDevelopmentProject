using UnityEngine;

[ExecuteAlways]
public class DesertTerrainGenerator : MonoBehaviour
{
    public Terrain terrain;

    [Header("Terrain Size")]
    public int heightmapResolution = 513;
    public int terrainWidth = 600;
    public int terrainLength = 600;
    public int terrainHeight = 55;

    [Header("Base Shape")]
    public float macroNoiseScale = 0.0045f;
    public float macroStrength = 0.18f;
    public float detailNoiseScale = 0.02f;
    public float detailStrength = 0.035f;

    [Header("Dunes")]
    public float duneScale = 0.018f;
    public float duneHeight = 0.12f;
    public float duneWarpScale = 0.008f;
    public float duneWarpStrength = 18f;

    [Header("Wind Direction")]
    public float windDirX = 1f;
    public float windDirZ = 0.35f;

    [Header("Playable Area")]
    public bool flattenSpawnArea = true;
    public Vector2 spawnCenterNormalized = new Vector2(0.5f, 0.5f);
    public float spawnRadius = 0.12f;
    public float spawnFlatHeight = 0.08f;

    [Header("Offsets")]
    public float offsetX = 0f;
    public float offsetZ = 0f;

    [ContextMenu("Generate Desert Terrain")]
    public void Generate()
    {
        if (terrain == null)
        {
            Debug.LogError("No Terrain assigned.");
            return;
        }

        TerrainData data = terrain.terrainData;
        data.heightmapResolution = heightmapResolution;
        data.size = new Vector3(terrainWidth, terrainHeight, terrainLength);

        float[,] heights = new float[heightmapResolution, heightmapResolution];

        Vector2 wind = new Vector2(windDirX, windDirZ).normalized;
        Vector2 cross = new Vector2(-wind.y, wind.x);

        for (int z = 0; z < heightmapResolution; z++)
        {
            for (int x = 0; x < heightmapResolution; x++)
            {
                float nx = (float)x / (heightmapResolution - 1);
                float nz = (float)z / (heightmapResolution - 1);

                float worldX = nx * terrainWidth + offsetX;
                float worldZ = nz * terrainLength + offsetZ;

                // Large rolling desert shape
                float macro = FractalNoise(worldX, worldZ, macroNoiseScale, 4, 0.5f, 2f);
                float detail = FractalNoise(worldX, worldZ, detailNoiseScale, 3, 0.5f, 2f);

                // Warp the dune coordinates so ridges do not look too perfect
                float warpX = (Mathf.PerlinNoise(worldX * duneWarpScale, worldZ * duneWarpScale) - 0.5f) * 2f;
                float warpZ = (Mathf.PerlinNoise((worldX + 1000f) * duneWarpScale, (worldZ + 1000f) * duneWarpScale) - 0.5f) * 2f;

                float warpedX = worldX + warpX * duneWarpStrength;
                float warpedZ = worldZ + warpZ * duneWarpStrength;

                // Project along wind/crosswind for dune shaping
                float alongWind = warpedX * wind.x + warpedZ * wind.y;
                float acrossWind = warpedX * cross.x + warpedZ * cross.y;

                // Dune lines
                float dunePatternA = Mathf.PerlinNoise(alongWind * duneScale, acrossWind * duneScale * 0.22f);
                float dunePatternB = Mathf.PerlinNoise(
                    alongWind * duneScale * 0.55f + 200f,
                    acrossWind * duneScale * 0.12f + 200f
                );

                // Sharpen some ridges but keep them soft
                float dunes = Mathf.Pow(dunePatternA, 2.2f) * 0.75f + dunePatternB * 0.25f;

                float height = 0f;
                height += macro * macroStrength;
                height += detail * detailStrength;
                height += dunes * duneHeight;

                // Slight basin shaping so edges feel more natural
                float edgeFadeX = Mathf.SmoothStep(0f, 1f, nx) * Mathf.SmoothStep(0f, 1f, 1f - nx);
                float edgeFadeZ = Mathf.SmoothStep(0f, 1f, nz) * Mathf.SmoothStep(0f, 1f, 1f - nz);
                float edgeFade = edgeFadeX * edgeFadeZ;
                height *= Mathf.Lerp(0.7f, 1f, edgeFade);

                // Optional flatter spawn/play area
                if (flattenSpawnArea)
                {
                    float dx = nx - spawnCenterNormalized.x;
                    float dz = nz - spawnCenterNormalized.y;
                    float dist = Mathf.Sqrt(dx * dx + dz * dz);

                    if (dist < spawnRadius)
                    {
                        float t = Mathf.InverseLerp(spawnRadius, 0f, dist);
                        t = Mathf.SmoothStep(0f, 1f, t);
                        height = Mathf.Lerp(height, spawnFlatHeight, t * 0.9f);
                    }
                }

                heights[z, x] = Mathf.Clamp01(height);
            }
        }

        data.SetHeights(0, 0, heights);
    }

    float FractalNoise(float x, float z, float scale, int octaves, float persistence, float lacunarity)
    {
        float amplitude = 1f;
        float frequency = 1f;
        float total = 0f;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = x * scale * frequency;
            float sampleZ = z * scale * frequency;

            float n = Mathf.PerlinNoise(sampleX, sampleZ);
            total += n * amplitude;
            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue;
    }
}
