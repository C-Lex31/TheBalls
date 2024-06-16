using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private static Player _instance;

    public static Player instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Player>();
            }

            return _instance;
        }
    }

    public GuideLine guideLine;
    [SerializeField] private GameObject center;
    [HideInInspector] public Vector3 nextPosition;
    
    [HideInInspector] public List<Ball> activeBall = new List<Ball>();
    [HideInInspector] public List<Ball> inBall = new List<Ball>();
    [HideInInspector] public List<BlockBase> addBallBlock = new List<BlockBase>();
    
    [HideInInspector] public bool isFirst = false;
    [HideInInspector] public GameObject shotRot;
    [HideInInspector] public bool isReturnBall = false;
    [SerializeField] private ParticleSystem fxGet;
    [SerializeField] private TextMeshProUGUI textGetBallCount;

    [HideInInspector] public GameObject[] balls;
    [HideInInspector] public GameObject selectBall;

    public int ballMaxCount = 1;
    public int ballCount = 1;

    private int num = 0;

    /// <summary>
    /// Player Initial Settings
    /// </summary>
    public void SetData()
    {
        selectBall = balls[GameData.SelectBallNum];
        center.GetComponent<SpriteRenderer>().sprite = selectBall.GetComponent<Ball>().spriteBall.sprite; //Safe to remove
        PoolManager.CreatePool(selectBall, 50, false, 0);

        shotRot = new GameObject();
        shotRot.name = "ShotRot";
        nextPosition = guideLine.transform.position;
        nextPosition.y = transform.position.y;
        CtrUI.instance.SetBallCount(ballCount);
    }



    #region //Ball Launch

    public void ShotBall()
    {
        CtrUI.instance.SetReturnBallButton(true);


        isReturnBall = false;
        shotRot.transform.position = guideLine.transform.position;
        shotRot.transform.rotation = guideLine.transform.rotation;

        CtrGame.instance.IsLock = true;

        StartCoroutine(ShotBallCo());
        guideLine.GuidelineOff();
    }

    #endregion



    
    IEnumerator ShotBallCo()
    {
        center.SetActive(false);

        Vector3 shotpos = guideLine.transform.position;
        
        for (int i = 0; i < ballMaxCount; i++)
        {
            //SoundManager.Instance.PlayEffect(Sound.sound_play_sfx_ball_launch);
            CtrGame.instance.ShotSound();
            
            Ball ball = PoolManager.Spawn(selectBall, shotpos, Quaternion.identity).GetComponent<Ball>();
            ballCount -= 1;
            CtrUI.instance.SetBallCount(ballCount);
        
            if (i == 0)
            {
                ball.isFirst = true;
            }
        
            activeBall.Add(ball);
            ball.SetData(1); //damage = 1
            yield return new WaitForSeconds(0.035f);
        
            if (ballCount < 0)
            {
                ballCount = 0;
            }
        }

        yield return new WaitForSeconds(0.035f);
        CtrUI.instance.textBallCount.DOFade(0f, 0.1f).SetEase(Ease.OutCubic);
        StartCoroutine(CheckTurnCo());
    }

    IEnumerator CheckTurnCo()
    {
        while (activeBall.Count > 0)
        {
            yield return null;
        }

        StartCoroutine(ReadyPlayerCo());
    }

    public void SetNextPositionX(float posX)
    {
        nextPosition.x = posX;
        Transform guideLineTransform;
        (guideLineTransform = guideLine.transform).DOMoveX(posX, 0f);
        guideLineTransform.rotation = Quaternion.identity;
        center.gameObject.SetActive(true);

        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_ball_comback);
    }

    IEnumerator ReadyPlayerCo()
    {
        CtrUI.instance.SetReturnBallButton(false);
        SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_ball_comback);
        CtrUI.instance.textBallCount.DOFade(1f, 0.1f).SetEase(Ease.OutCubic);
        CtrUI.instance.textBallCount.transform.DOMoveX(nextPosition.x, 0f);

        //Initialize the number of balls
        ballCount = ballMaxCount;
        //Ball count UI applied
        CtrUI.instance.SetBallCount(ballMaxCount);

        //Additional Ball Animation
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].transform.DOKill();
            addBallBlock[i].transform.DOMove(nextPosition, 0.1f);
            SoundManager.Instance.PlayEffect(SoundList.sound_play_sfx_ball_comback);
            fxGet.Play();
        }

        yield return new WaitForSeconds(0.15f);
        if (addBallBlock.Count > 0)
        {
            textGetBallCount.text = $"+{addBallBlock.Count}";
            textGetBallCount.transform.DOMove(nextPosition, 0f);
            textGetBallCount.DOFade(1f, 0f);
            textGetBallCount.transform.DOMoveY(0.5f, 0.2f).SetEase(Ease.OutCubic).SetRelative(true);
            textGetBallCount.DOFade(0f, 1f).SetEase(Ease.Linear).SetDelay(0.2f);
        }

        //Delete added ball blocks
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].Destroy();
        }

        //Existing Ball += Added Ball
        ballCount = ballMaxCount += addBallBlock.Count;
        CtrUI.instance.SetBallCount(ballMaxCount);

        //Initialize the list of added balls
        addBallBlock.Clear();

        inBall.Clear();
        isFirst = false;
        CtrGame.instance.NextTurn();
    }

    //Player settings if continued
    public void ContinuePlayer()
    {
        StartCoroutine(ContinuePlayerCo());
    }

    IEnumerator ContinuePlayerCo()
    {
        CtrUI.instance.SetReturnBallButton(false);
        CtrUI.instance.textBallCount.DOFade(1f, 0.1f).SetEase(Ease.OutCubic);

        //Initialize the number of balls
        ballCount = ballMaxCount;
        
        //Ball count UI applied
        CtrUI.instance.SetBallCount(ballMaxCount);

        //Additional ball animation
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].transform.DOKill();
            addBallBlock[i].transform.DOMove(nextPosition, 0.15f);
            fxGet.Play();
        }

        yield return new WaitForSeconds(0.15f);

        if (addBallBlock.Count > 0)
        {
            textGetBallCount.text = $"+{addBallBlock.Count}";
            textGetBallCount.transform.DOMove(nextPosition, 0f);
            textGetBallCount.DOFade(1f, 0f);
            textGetBallCount.transform.DOMoveY(0.5f, 0.2f).SetEase(Ease.OutCubic).SetRelative(true);
            textGetBallCount.DOFade(0f, 1f).SetEase(Ease.Linear).SetDelay(0.2f);
        }

        //Delete added ball blocks
        for (int i = 0; i < addBallBlock.Count; i++)
        {
            addBallBlock[i].Destroy();
        }

        //Existing Ball += Added Ball
        ballCount = ballMaxCount += addBallBlock.Count;
        CtrUI.instance.SetBallCount(ballMaxCount);

        //Initialize the list of added balls
        addBallBlock.Clear();

        //Initialize according to the number of balls
        //CtrBallSlot.instance.ResetBallCount();

        inBall.Clear();
        isFirst = false;

        CtrGame.instance.isGameOver = false;
        CtrGame.instance.NextTurnMoveEnd();
    }


    //After the ball is fired, the recovery button is activated.
    //When the button is clicked, the currently active ball is immediately recovered.
    //The player is ready to fire.
    
    #region //Recover the ball.
    public void ReturnBall()
    {
        StopAllCoroutines();
        CtrUI.instance.SetReturnBallButton(false);
        StartCoroutine(ReturnBallCo());
    }

    IEnumerator ReturnBallCo()
    {
        for (int i = 0; i < activeBall.Count; i++)
        {
            activeBall[i].ReturnBall();
        }

        yield return new WaitForSeconds(0.25f);
        activeBall.Clear();
        SetNextPositionX(nextPosition.x);

        StartCoroutine(ReadyPlayerCo());
    }
    #endregion
}

