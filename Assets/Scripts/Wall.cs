using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // 被攻击一次之后的Wall图片
    public Sprite dmgSprite;
    // wall 血量
    public int hp = 4;
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        SoundManager.instance.RandomizeSfx(chopSound1,chopSound2);
        if (hp<=0) {
            gameObject.SetActive(false);
        }
    }
}
