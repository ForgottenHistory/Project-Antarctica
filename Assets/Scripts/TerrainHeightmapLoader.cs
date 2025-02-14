using UnityEngine;

public class TerrainHeightmapLoader : MonoBehaviour 
{
    [SerializeField] private Texture2D heightmapTexture;
    [SerializeField] private float heightMultiplier = 1f;
    private Terrain terrain;
    private TerrainData terrainData;

    // Remove Awake() and initialize components when needed
    private void InitializeComponents()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();
        if (terrainData == null && terrain != null)
            terrainData = terrain.terrainData;
    }

    public void ApplyHeightmap()
    {
        if (heightmapTexture == null) return;

        // Initialize components before using them
        InitializeComponents();
        
        // Check if components are properly initialized
        if (terrain == null || terrainData == null)
        {
            Debug.LogError("Terrain or TerrainData is missing!");
            return;
        }

        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                float xCoord = (float)x / terrainData.heightmapResolution;
                float yCoord = (float)y / terrainData.heightmapResolution;
                float height = heightmapTexture.GetPixelBilinear(xCoord, yCoord).grayscale;
                heights[x, y] = height * heightMultiplier;
            }
        }
        
        terrainData.SetHeights(0, 0, heights);
    }

    public void ResetTerrain()
    {
        InitializeComponents();
        
        if (terrain == null || terrainData == null)
        {
            Debug.LogError("Terrain or TerrainData is missing!");
            return;
        }

        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        terrainData.SetHeights(0, 0, heights);
    }
}