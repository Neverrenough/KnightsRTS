using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LayerGenerator : MonoBehaviour
{
    [SerializeField] private TileBase tileWater;
    [SerializeField] private Tilemap tileMapWater;

    [SerializeField] public bool isWaterLayer;

    private int layerWidth;
    private int layerHeight; 
    [SerializeField] private float noiseScale = 0.1f; // Масштаб шума для неровных краев

    [SerializeField] public Tilemap tilemapGroundLevel;
    [SerializeField] public Tilemap tilemapStoneLevel;
    [SerializeField] public Tilemap tilemapShadowsOrWaterFoamLevel;

    [SerializeField] private TileBase tileGroundLastOrFirst; // Тайл который странно генерируется я бы назвал это shitCode но меня это устраивает
    [SerializeField] private TileBase tileStoneLastOrFirst; // Тайл который странно генерируется я бы назвал это shitCode но меня это устраивает

    [SerializeField] private TileBase[] tilesEdge; // 9 тайлов для обводки (0-верх, 1-низ, 2-лево, 3-право, 4-лево верх, 5-право верх, 6-лево низ, 7-право низ, 8-центр)
    [SerializeField] private TileBase[] tilesStoneEdge; // 3 тайла краев для нижнего слоя
    [SerializeField] private TileBase[] tilesStoneWalls; // 3 тайла стен для нижнего слоя
    [SerializeField] private TileBase tileShadow;
    [SerializeField] private AnimatedTile tileWaterFoam;

    private int[,] mapData;
    
    public void SetLayerSize(int _layerWidth, int _layerHeight)
    {
        layerWidth = _layerWidth;
        layerHeight = _layerHeight;
    }
    public void GenerateLayerSquare()
    {
        
        mapData = new int[layerWidth, layerHeight];

        // Генерация внутренностей
        for (int x = 0; x < layerWidth; x++)
        {
            for (int y = 0; y < layerHeight; y++)
            {
                float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                if (noiseValue > 0.3f || (x > 5 && x < layerWidth - 5 && y > 5 && y < layerHeight - 5))
                {
                    mapData[x, y] = 1; // Внутренний тайл
                }
            }
        }
        for (int x = 0; x < layerWidth * 2; x++)
        {
            for(int y = 0; y < layerHeight * 2; y++)
            {
                tileMapWater.SetTile(new Vector3Int(x - (layerWidth/2), y - (layerHeight / 2), 0), tileWater);
            }
        }

        // Генерация обводки
        for (int x = 0; x < layerWidth; x++)
        {
            for (int y = 0; y < layerHeight; y++)
            {
                if (mapData[x, y] == 1)
                {
                    PlaceTileSquare(x, y);
                }
            }
        }
    }

    void PlaceTileSquare(int x, int y)
    {
        bool top = (y < layerHeight - 1 && mapData[x, y + 1] == 1);
        bool bottom = (y > 0 && mapData[x, y - 1] == 1);
        bool left = (x > 0 && mapData[x - 1, y] == 1);
        bool right = (x < layerWidth - 1 && mapData[x + 1, y] == 1);

        if (top && bottom && left && right)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[8]); // Центр
        }
        else if (top && bottom && left)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[3]); // Правый край
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[6]);

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else if (top && bottom && right)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[2]); // Левый край
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[5]);

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y  - 3, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else if (left && right && bottom)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[0]); // Верхний край
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[7]); // Камень в левом нижнем углу

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else if (left && right && top)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[1]); // Нижний край
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[1]); // Камень по нижнему краю
            tilemapStoneLevel.SetTile(new Vector3Int(x, y - 1, 0), tilesStoneWalls[1]); // Камень по нижнему краю

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else if (top && left)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[5]); // Левый верхний угол
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[2]); // Камень в правом нижнем углу
            tilemapStoneLevel.SetTile(new Vector3Int(x, y - 1, 0), tilesStoneWalls[2]); // Камень в правом нижнем углу

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else if (top && right)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[4]); // Правый верхний угол
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[0]); // Камень в левом нижнем углу
            tilemapStoneLevel.SetTile(new Vector3Int(x, y - 1, 0), tilesStoneWalls[0]); // Камень в левом нижнем углу

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else if (bottom && left)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[7]); // Левый нижний угол
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[4]); // Камень в левом нижнем углу

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else if (bottom && right)
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[6]); // Правый нижний угол
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tilesStoneEdge[3]); // Камень в левом нижнем углу
            
            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
        else
        {
            tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tileGroundLastOrFirst); // Запасной вариант
            tilemapStoneLevel.SetTile(new Vector3Int(x, y, 0), tileStoneLastOrFirst);
            tilemapStoneLevel.SetTile(new Vector3Int(x, y - 1, 0), tilesStoneWalls[0]);

            if (isWaterLayer == true)
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 2, 0), tileWaterFoam);
            else
                tilemapShadowsOrWaterFoamLevel.SetTile(new Vector3Int(x, y - 1, 0), tileShadow);
        }
    }
    /*public void GenerateLayerMarchingSquares()
    {
        mapData = new int[layerWidth, layerHeight];

        for (int x = 0; x < layerWidth; x++)
        {
            for (int y = 0; y < layerHeight; y++)
            {
                float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                if (noiseValue > 0.3f || (x > 5 && x < layerWidth - 5 && y > 5 && y < layerHeight - 5))
                {
                    mapData[x, y] = 1;
                }
            }
        }

        GenerateMarchingSquares();
    }

    void GenerateMarchingSquares()
    {
        for (int x = 0; x < layerWidth - 1; x++)
        {
            for (int y = 0; y < layerHeight - 1; y++)
            {
                int squareValue = GetMarchingSquareValue(x, y);
                PlaceMarchingSquareTile(x, y, squareValue);
            }
        }
    }

    int GetMarchingSquareValue(int x, int y)
    {
        int value = 0;
        if (mapData[x, y] == 1) value += 1;
        if (mapData[x + 1, y] == 1) value += 2;
        if (mapData[x, y + 1] == 1) value += 4;
        if (mapData[x + 1, y + 1] == 1) value += 8;
        return value;
    }

    void PlaceMarchingSquareTile(int x, int y, int value)
    {
        if (value == 0 || value == 15) return;
        tilemapGroundLevel.SetTile(new Vector3Int(x, y, 0), tilesEdge[value % tilesEdge.Length]);
    }*/
}
