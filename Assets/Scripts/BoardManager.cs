using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int rows = 8;
    public int columns = 8;
    public GameObject[] floorFiles;
    public GameObject[] outerWallTiles;
    public GameObject exitTile;

    Transform boardHolder;

    /*
     * 生成关卡
     */
    public void SetupScene()
    {
        this.BoardSetUp();
        this.ExitTileSetUp();
    }

    /*
     * 铺设地板
     * 如果是地图的四条边则铺设outwall外墙
     * 地图内部则铺设Floor地板
     */
    private void BoardSetUp()
    {
        boardHolder = new GameObject("board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // 随机铺设
                GameObject toInstantiate = floorFiles[Random.Range(0, floorFiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    /**
     * 生成出口
     */
    private void ExitTileSetUp()
    {
        Instantiate(exitTile, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
