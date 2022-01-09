using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    // 进行线性投射的时候指定的LayerMask层
    public LayerMask blockingLayer;

    // 每次移动耗时 (s)
    public float moveTime = 0.1f;

    // 脚本所挂载的游戏对象上的碰撞器组件
    private BoxCollider2D boxCollider2D;
    Rigidbody2D rb2D;
    float inverseMoveTime;

    /**
     * 初始化
     */
    protected virtual void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    /**
     * 利用协程实现平滑移动
     */
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainDistance = (transform.position - end).sqrMagnitude;
            // 剩余代码将在下一帧继续执行
            yield return null;
        }
        GameController.instance.playerTurn = true;
    }

    /**
     * 线性投射检测和移动
     */
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider2D.enabled = false;
        // 调用线性投射检测前方是否有障碍物
        // 线性投射，是Unity自带方法。它会从开始位置到结束位置做一个光线投射，如果与指定的Layer mask层的碰撞体交互，就会返回真和一个RaycastHit2D结构体信息
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider2D.enabled = true;

        if (hit.transform == null)
        {
            // 进行平滑移动
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    /**
     * 根据传入的方向参数调用move() 进行移动判断和操作
     * 不能移动的情况下根据返回的hit结构体判断是否调用onCantMove 进行处理
     */
    protected virtual void AttempMove<T>(int xDir, int yDir) where T : Component
    {
        // 线性投射检测到的结构体信息
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if (hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
        GameController.instance.playerTurn = true;
    }

    protected abstract void OnCantMove<T>(T component) where T : Component;
}
