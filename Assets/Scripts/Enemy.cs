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

        // ��Player��Enemy��ͬһ��X���꣬������������Y������иߵ��жϣ����target��Player����yֵ�ߣ����ƶ������������ƶ���yDirΪ1������Ϊ-1�����ƶ���
        // ��Player��Enemy����ͬһ��X���꣬��ֱ���ж�X�ߵͣ�target����xDirΪ1�����ƶ�������Ϊ - 1�����ƶ���
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

    // Player�����غϣ����������һ�غ�
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
