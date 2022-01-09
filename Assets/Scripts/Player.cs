using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{

    // 每次劈砍对障碍墙造成的伤害值
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    Animator animator;
    int food;

    public void LostFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food:" + food;
        this.CheckIfGameOver();
    }

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameController.instance.playerFoodPoints;
        foodText.text = "Food:" + food;
        base.Start();
    }

    // 获取用户键盘输入
    private void Update()
    {
        if (!GameController.instance.playerTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        // 当horizontal不为0的时候，vertical强制性为0，也就是说不能斜着走，只能上下左右移动
        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            GameController.instance.playerTurn = false;
            AttempMove<Wall>(horizontal, vertical);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitwall = component as Wall;
        hitwall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    // 每次移动扣血 没有血结束游戏
    protected override void AttempMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food:" + food;
        base.AttempMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        this.CheckIfGameOver();
        GameController.instance.playerTurn = false;
    }

    private void OnDisable()
    {
        GameController.instance.playerFoodPoints = food;
    }

    // 检测碰撞到的对象的tag 如果是food或者soda 则增加生命值并隐藏
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food:" + food;
            SoundManager.instance.RandomizeSfx(eatSound1,eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food:" + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
    }

    // 重新加载场景
    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    // 检测生命小于0 则结束游戏
    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.playSingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameController.instance.GameOver();
        }
    }
}
