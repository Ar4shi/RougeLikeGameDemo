using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    // ��������Ͷ���ʱ��ָ����LayerMask��
    public LayerMask blockingLayer;

    // ÿ���ƶ���ʱ (s)
    public float moveTime = 0.1f;

    // �ű������ص���Ϸ�����ϵ���ײ�����
    private BoxCollider2D boxCollider2D;
    Rigidbody2D rb2D;
    float inverseMoveTime;

    /**
     * ��ʼ��
     */
    protected virtual void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    /**
     * ����Э��ʵ��ƽ���ƶ�
     */
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainDistance = (transform.position - end).sqrMagnitude;
            // ʣ����뽫����һ֡����ִ��
            yield return null;
        }
        GameController.instance.playerTurn = true;
    }

    /**
     * ����Ͷ������ƶ�
     */
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider2D.enabled = false;
        // ��������Ͷ����ǰ���Ƿ����ϰ���
        // ����Ͷ�䣬��Unity�Դ�����������ӿ�ʼλ�õ�����λ����һ������Ͷ�䣬�����ָ����Layer mask�����ײ�彻�����ͻ᷵�����һ��RaycastHit2D�ṹ����Ϣ
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider2D.enabled = true;

        if (hit.transform == null)
        {
            // ����ƽ���ƶ�
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    /**
     * ���ݴ���ķ����������move() �����ƶ��жϺͲ���
     * �����ƶ�������¸��ݷ��ص�hit�ṹ���ж��Ƿ����onCantMove ���д���
     */
    protected virtual void AttempMove<T>(int xDir, int yDir) where T : Component
    {
        // ����Ͷ���⵽�Ľṹ����Ϣ
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
