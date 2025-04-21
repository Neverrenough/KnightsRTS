using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapMinWidth;
    [SerializeField] private int mapMinHeight;
    [SerializeField] private int mapIntWidth;
    [SerializeField] private int mapIntHeight;

    [SerializeField] private int countLayers;

    [SerializeField] private Tilemap waterTilemap;

    [SerializeField] private GameObject grid;
    [SerializeField] private Tilemap tileMap;

    [SerializeField] private LayerGenerator layerGenerator;

    private int sortingOrderInt = 0;

    private void Start()
    {
        layerGenerator.SetLayerSize(mapMinWidth, mapMinHeight);
        GenerateLayers();         
    }
    private void GenerateLayers()
    {
        for(int l = 0; l < countLayers; l++)
        {
            for (int i = 0; i < 3; i++)
            {
                sortingOrderInt -= 1;

                GameObject currentTileMap = Instantiate(tileMap.gameObject, grid.transform);
                currentTileMap.GetComponent<TilemapRenderer>().sortingOrder = sortingOrderInt;
                currentTileMap.transform.position = new Vector3(-(mapMinWidth / 2), -(mapMinHeight / 2), 0);

                if (i == 0)
                    layerGenerator.tilemapGroundLevel = currentTileMap.GetComponent<Tilemap>();
                if (i == 1)
                    layerGenerator.tilemapStoneLevel = currentTileMap.GetComponent<Tilemap>();
                if (i == 2)
                {  
                    layerGenerator.tilemapShadowsOrWaterFoamLevel = currentTileMap.GetComponent<Tilemap>();
                    if(l == countLayers - 1)
                    {
                        layerGenerator.isWaterLayer = true;
                        currentTileMap.transform.position = new Vector3(currentTileMap.transform.position.x, currentTileMap.transform.position.y - 0.5f, 0);
                    }
                             
                }
            }
            mapMinWidth += mapIntWidth;
            mapMinHeight += mapIntHeight;
            layerGenerator.SetLayerSize(mapMinWidth, mapMinHeight);
            layerGenerator.GenerateLayerSquare();
            //layerGenerator.GenerateLayerMarchingSquares();
        }
        waterTilemap.GetComponent<TilemapRenderer>().sortingOrder = sortingOrderInt - 1;
    }
}
