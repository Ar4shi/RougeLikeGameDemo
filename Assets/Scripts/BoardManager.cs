
using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // 序列化Count类，使Count类的实例在Inspector面板上显示, 并可以赋予相应的值
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            this.minimum = min;
            this.maximum = max;
        }
    }

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public int rows = 8;
    public int columns = 8;
    public GameObject[] floorFiles;
    public GameObject[] outerWallTiles;
    public GameObject exitTile;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;

    private Transform boardHolder;
    private List<Vector3> gridPosition = new List<Vector3>();

    /*
     * 生成关卡
     */
    public void SetupScene(int level)
    {
        this.BoardSetUp();
        this.ExitTileSetUp();
        this.ItemSetUp(level);
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
                GameObject toInstantiate = floorFiles[UnityEngine.Random.Range(0, floorFiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[UnityEngine.Random.Range(0, outerWallTiles.Length)];
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

    /**
     * 生成随机物品
     */
    private void ItemSetUp(int level)
    {
        this.InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
    }

    // 初始化随机区域坐标
    private void InitialiseList()
    {
        gridPosition.Clear();
        for (int i = 1; i < columns - 1; i++)
        {
            for (int j = 1; j < rows - 1; j++)
            {
                gridPosition.Add(new Vector3(i, j, 0f));
            }
        }
    }

    /**
     * 返回一个随机坐标 且不重复
     */
    private Vector3 RandomPosition()
    {
        int randomIndex = UnityEngine.Random.Range(0, gridPosition.Count);
        Vector3 randomPosition = gridPosition[randomIndex];
        gridPosition.RemoveAt(randomIndex);
        return randomPosition;
    }

    /**
     * 随机生成资源在指定位置
     */
    private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = UnityEngine.Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = this.RandomPosition();
            GameObject tileChoice = tileArray[UnityEngine.Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
}
