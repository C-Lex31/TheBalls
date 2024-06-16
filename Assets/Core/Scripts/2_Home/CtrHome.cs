using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.iOS;
#endif



public class CtrHome : CtrBase
{
    //IOS Haptic Button
    public GameObject settingObjHaptic;
    
    
    public RectTransform panelSetting;
    public Image settingDim;
    bool isOpenSetting = false;
    
    public CanvasGroup canvasGroupHome;
    public MyInfo _MyInfo;
    float posOpenY = -180f;
    float posCloseY = -930f;
    bool isPlay = false;
    public GameObject lockScreen;

    //Panels
    public PanelStart panelStart;
    public PanelMyPage panelMyPage;
    public PanelRanking panelRanking;
    public PanelBallSkin panelBallSkin;
    public PanelShop panelShop;
    public PopupBuy _PopupBuy;

    public void Start()
    {
        SetData();
    }

    void SetData()
    {
        SetHomeUI(false, null, 0f);

        //Currency settings
        PlayManager.Instance.commonUI._CoinGem.SetCoin();
        PlayManager.Instance.commonUI._CoinGem.SetGem();
        
        if (GameData.NickName == "")
        {
            Instantiate(Resources.Load("Popup/Popup_InputName"));
        }
        else
        {
            //todo Import ranking data
            // RankingManager.Instance.GetRankingData();
        }

        //Ranking Panel Reset
        panelRanking.UIReset();

        //Start Panel Reset
        panelStart.UIReset();

        //Reset my page
        panelMyPage.UIReset();

        //Ball panel initialization
        panelBallSkin.UIReset();

        //Reset panel store
        panelShop.UIReset();

        _PopupBuy.UIReset();


#if UNITY_IOS
        //Check review
        if (!GameData.ReviewSuccess) {
            if (GameData.ReviewCount != 0 && GameData.ReviewCount % 10 == 0) {
                GameData.ReviewSuccess = true;
                Device.RequestStoreReview();
            } else {
                GameData.ReviewCount += 1;
            }
        } else {
            if (GameData.ReviewCount != 0 && GameData.ReviewCount % 1000 == 0) {
                Device.RequestStoreReview();
            } else {
                GameData.ReviewCount += 1;
            }
        }
#endif


#if UNITY_ANDROID
        // Hide the haptic function for Android
        settingObjHaptic.gameObject.SetActive(false);
#endif

        //Play background music
         SoundManager.Instance.PlayBGM(SoundList.sound_home_bgm);

        if (GameData.NickName != "")
        {
            //My information settings
            SetMyInfoMyPage();
        }
    }


    public void SetMyInfoMyPage()
    {
        StartCoroutine(SetMyInfoMyPageCo());
    }

    //My information settings
    IEnumerator SetMyInfoMyPageCo()
    {
        // while (!RankingManager.Instance.isDataLoadComplete) {
        //     yield return null;
        // }

        yield return null;
        _MyInfo.SetMyInfo();
        panelMyPage.SetMyPage();

        SetHomeUI(true, null);
    }

    //todo Click My Page
    public void Click_MyPage()
    {
        PlayManager.Instance.commonUI._CoinGem.Hide();
        panelMyPage.Open();
    }


    // public TextMeshProUGUI textBestScore;
    //
    // //Animation showing best score
    // IEnumerator ShowBestScoreCo()
    // {
    //
    //     int score = 0;
    //     float time = 0.5f;
    //
    //     DOTween.To(() => score, x => score = x, GameData.BestScore, time).SetEase(Ease.Linear);
    //
    //     while (time > 0)
    //     {
    //         time -= Time.deltaTime;
    //         textBestScore.text = Data.ChangeCountFormat(score);
    //         yield return null;
    //     }
    //
    //     textBestScore.text = Data.ChangeCountFormat(GameData.BestScore);
    // }



    #region 

    public void ClickPlay()
    {
        panelStart.SetPanelStart();
    }

    #endregion


    #region 

    public void Click_Ball()
    {
        panelBallSkin.SetData();
        panelBallSkin.Open();
    }

    #endregion


    #region 

    public void Click_Ranking()
    {
        PlayManager.Instance.commonUI._CoinGem.Hide();
        panelRanking.Open();
    }

    #endregion

    #region 

    public void Click_Settings()
    {
        if (!isOpenSetting)
        {
            settingDim.gameObject.SetActive(true);
            settingDim.DOFade(1f, 0.2f);


            isOpenSetting = true;
            panelSetting.DOAnchorPosY(posOpenY, 0.2f).SetEase(Ease.InOutCubic);
            SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);

            SetHomeUI(false);
        }
        else
        {
            isOpenSetting = false;
            panelSetting.DOAnchorPosY(posCloseY, 0.2f).SetEase(Ease.OutCubic);
            SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_close);

            settingDim.DOFade(0f, 0.2f).OnComplete(() => { settingDim.gameObject.SetActive(false); });

            SetHomeUI(true);
        }

        lockScreen.SetActive(isOpenSetting);
    }

    #endregion



    public void Click_Restore () {
        // IAPManager.instance.RestorePurchases();
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }


    #region 

    public void Click_YouTube()
    {
         Application.OpenURL(Data.url_portfolio);
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }

    #endregion


    #region 

    public void Click_Source()
    {
        Application.OpenURL(Data.url_srccode);
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }

    #endregion

    #region

    public void Click_Instagram()
    {
        Application.OpenURL(Data.url_facebook);
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }

    #endregion


    #region 

    public void Click_Shop()
    {
        panelShop.Open();
    }

    #endregion


    public void SetHomeUI(bool value, CanvasGroup targetGroup, float t = 0.25f)
    {
        float time = t;
        if (!value)
        {
            //Hide the Home panel
            canvasGroupHome.DOKill();

            canvasGroupHome.DOFade(0f, time).SetEase(Ease.OutSine);
            canvasGroupHome.transform.DOScale(1.15f, time).SetEase(Ease.OutSine).OnComplete(() =>
            {
                canvasGroupHome.gameObject.SetActive(false);

                if (targetGroup != null)
                {
                    targetGroup.gameObject.SetActive(true);
                    targetGroup.DOKill();
                    targetGroup.DOFade(1f, time).SetEase(Ease.OutCubic);
                    targetGroup.transform.DOScale(1f, time).SetEase(Ease.OutCubic);


                    if (targetGroup.GetComponent<PanelRanking>() != null)
                    {
                        targetGroup.GetComponent<PanelRanking>().ShowRanking();
                    }
                }
            });
        }
        else
        {
            PlayManager.Instance.commonUI._CoinGem.Show();

            if (targetGroup != null)
            {
                targetGroup.DOKill();
                targetGroup.DOFade(0f, time).SetEase(Ease.OutCubic);
                targetGroup.transform.DOScale(0.95f, time).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    targetGroup.gameObject.SetActive(false);
                    //Show home panel
                    canvasGroupHome.DOKill();
                    canvasGroupHome.gameObject.SetActive(true);
                    canvasGroupHome.DOFade(1f, time).SetEase(Ease.OutCubic);
                    canvasGroupHome.transform.DOScale(1f, time).SetEase(Ease.OutBack);
                });
            }
            else
            {
                //Show home panel
                canvasGroupHome.DOKill();
                canvasGroupHome.gameObject.SetActive(true);
                canvasGroupHome.DOFade(1f, time).SetEase(Ease.OutCubic);
                canvasGroupHome.transform.DOScale(1f, time).SetEase(Ease.OutBack);
            }
        }
    }


    public void SetHomeUI(bool value)
    {
        if (!value)
        {
            //Hide the Home panel
            canvasGroupHome.DOKill();
            canvasGroupHome.transform.DOScale(0.9f, 0.25f).SetEase(Ease.OutCubic);
            PlayManager.Instance.commonUI._CoinGem.SetSmallSize();


        }
        else
        {
            canvasGroupHome.DOKill();
            canvasGroupHome.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutCubic);

            PlayManager.Instance.commonUI._CoinGem.SetLargeSize();
        }
    }

   


#if UNITY_ANDROID
    private void Update () {
        if (Input.GetKey(KeyCode.Escape)) {
            if (PlayManager.Instance.panelBase != null) return;

            if (PlayManager.Instance.isPopupOn) return;
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Instantiate(Resources.Load("Popup/Popup_Exit"));
            }
        }
    }
#endif

}
