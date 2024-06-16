using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BlockBase : MonoBehaviour {
    [HideInInspector] public BlockGroup blockGroup;
    public SpriteRenderer mySprite;
    public TextMeshPro textHealht;
    [HideInInspector] public bool isBall = false;
    [HideInInspector] public int blockHealth;
    [HideInInspector] public bool isDestory = false;

    public virtual void SetData (int health) {
        isDestory = false;
        if (textHealht != null) {
            textHealht.text = Utility.ChangeThousandsSeparator(health);
            this.blockHealth = health;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        if (isDestory) return;

        if (collision.gameObject.CompareTag("Ball")) {
            InDamage(collision);
        }
    }

    public virtual void InDamage (Collision2D collision) {
        HitFx();

        //Reduce block health by the amount of ball damage
        blockHealth -= collision.gameObject.GetComponent<Ball>().damage;
        textHealht.text = Utility.ChangeThousandsSeparator(blockHealth);

        if (blockHealth <= 0) {
            if (CtrGame.instance.comboCount < 10) {
                CtrGame.instance.comboCount += 1;
            }

            ComboEffect comboEffect = PoolManager.Spawn(CtrPool.instance.pComboEffect, transform.position, Quaternion.identity).GetComponent<ComboEffect>();
            comboEffect.SetCombo();


            //Block is destroyed when its health becomes 0.
            blockHealth = 0;
            Destroy();
            CtrBlock.instance.CheckAllClear();
        }
    }

    //When it touches the floor line
    //public void OnTriggerEnter2D (Collider2D collision) {
    //    if (collision.gameObject.CompareTag("InTrigger")) {
    //        if (!CtrGame.instance.isGameOver) {
    //            CtrGame.instance.isGameOver = true;
    //            Debug.Log("InTrigger");
    //            CtrGame.instance.GameOver();
    //        }

    //        Destory();
    //    }
    //}

    //block destruction
    public virtual void Destroy () {
        if (isDestory) return;
        isDestory = true;

        if (!isBall) {
            //If it is not a ball, block effect and block destruction sound are played.
            PoolManager.Despawn(PoolManager.Spawn(CtrPool.instance.pFxBlockBoom, transform.position, Quaternion.identity), 1f);

            //todo savedata
            PlayManager.Instance.countBreakeBrick++;

#if UNITY_IOS
            if (PlayManager.Instance.isHaptic) {
                //iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
                //0 : small 1 : light 2 : midium 3 : heavy 4 : success 5 : warring 6 : falure 7 : onoff 
            }
#endif
        } 
        
        Reset();
        blockGroup.blockBases.Remove(this);
        PoolManager.Despawn(this.gameObject);
    }

    public virtual void Reset () { }
    public virtual void HitFx () { }
}
