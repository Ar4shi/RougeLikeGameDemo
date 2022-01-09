using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;
    // 回合等待时间，单位为s
    public float turnDelay = 0.1f;
    public float levelStartDelay = 2f;

    BoardManager boardManager;
    int level = 1;
    List<Enemy> enemies;
    // 怪物们是不是正在移动中，正在移动则为true，其他情况则为false
    bool enemyMoving;
    bool doingSetUp;
    GameObject levelImage;
    Text levelText;

    void Awake()
    {
        // 控制器使用单例模式
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardManager = GetComponent<BoardManager>();
        this.InitGame();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += levelWasLoaded;
    }

    private void levelWasLoaded(Scene s, LoadSceneMode mode)
    {
        level++;
        InitGame();
    }

    // 判断当playerTrun和enemyMoving均为false的情况下（是怪物回合并且怪物没有在移动中），
    // 使用StartCoroutine函数开启协同程序MoveEnemys()指挥怪物开始一个个进行移动；
    private void Update()
    {
        if (playerTurn || enemyMoving)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
    }

    // 游戏结束的时候禁用该组件
    public void GameOver()
    {
        levelText.text = "After " + level + " days,you dead.";
        levelImage.SetActive(true);
        enabled = false;
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    private void InitGame()
    {
        doingSetUp = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        // Invoke()可以实现根据时间调用指定的方法，延时加载界面，这样视觉上过渡更加平滑，不突兀
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardManager.SetupScene(level);
    }

    // 协程是分步骤执行代码的程序，遇到条件（yield return语句）会挂起暂停退出，直到条件满足才会被唤醒继续执行后面的代码。
    private IEnumerator MoveEnemies()
    {
        enemyMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        enemyMoving = false;
        playerTurn = true;
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetUp = false;
    }
}
