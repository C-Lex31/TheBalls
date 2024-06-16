using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CtrSplash : CtrBase {
    public Image imageSplash;

    void Awake () {
        imageSplash.DOFade(0f, 0f);
    }


    protected override void Start()
    {
        StartCoroutine(StartCo());
        base.Start();
    }

    IEnumerator StartCo () {
        
        imageSplash.DOFade(1, 0.25f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(2.0f);

        imageSplash.DOFade(0, 0.2f).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(0.2f);
        PlayManager.Instance.isFade = true;

        //Waiting for Update Check
        yield return StartCoroutine(CheckUpdateCo());

        PlayManager.Instance.LoadScene(Data.scene_title);
    }

    IEnumerator CheckUpdateCo () {
        bool isCheckUpdate = true;

        while (isCheckUpdate) {
            yield return new WaitForSeconds(1f);
            isCheckUpdate = false;
        }
    }
}
