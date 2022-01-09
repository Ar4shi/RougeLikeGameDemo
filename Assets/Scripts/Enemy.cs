using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    public AudioClip attackSound1;
    public AudioClip attackSound2;

    Transform target;
    bool skipMove;
    Animator animator;

    protected override void Start()
    {
        GameController.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // 当Player和Enemy在同一个X坐标，则对两个物体的Y坐标进行高低判断，如果target（Player）的y值高，则移动方向是向上移动，yDir为1，否则为-1向下移动。
        // 当Player和Enemy不在同一个X坐标，则直接判断X高低，target高则xDir为1向右移动，否则为 - 1向左移动。
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttempMove<Player>(xDir, yDir);
    }

    // Player走两回合，怪物才能走一回合
    protected override void AttempMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttempMove<T>(xDir, yDir);
        skipMove = true;
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(attackSound1,attackSound2);
        hitPlayer.LostFood(playerDamage);
    }
}
