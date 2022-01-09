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
    // �غϵȴ�ʱ�䣬��λΪs
    public float turnDelay = 0.1f;
    public float levelStartDelay = 2f;

    BoardManager boardManager;
    int level = 1;
    List<Enemy> enemies;
    // �������ǲ��������ƶ��У������ƶ���Ϊtrue�����������Ϊfalse
    bool enemyMoving;
    bool doingSetUp;
    GameObject levelImage;
    Text levelText;

    void Awake()
    {
        // ������ʹ�õ���ģʽ
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

    // �жϵ�playerTrun��enemyMoving��Ϊfalse������£��ǹ���غϲ��ҹ���û�����ƶ��У���
    // ʹ��StartCoroutine��������Эͬ����MoveEnemys()ָ�ӹ��￪ʼһ���������ƶ���
    private void Update()
    {
        if (playerTurn || enemyMoving)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
    }

    // ��Ϸ������ʱ����ø����
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
        // Invoke()����ʵ�ָ���ʱ�����ָ���ķ�������ʱ���ؽ��棬�����Ӿ��Ϲ��ɸ���ƽ������ͻأ
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardManager.SetupScene(level);
    }

    // Э���Ƿֲ���ִ�д���ĳ�������������yield return��䣩�������ͣ�˳���ֱ����������Żᱻ���Ѽ���ִ�к���Ĵ��롣
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
