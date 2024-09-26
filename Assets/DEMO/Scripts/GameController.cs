using DVAH;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : DVAH.Singleton<GameController>
{
    [SerializeField] Text text;
    private void Start()
    {
        
    }
    public void ShowInter()
    {
        if (!AdManagerDemo.Instant.IntersIsLoaded(0))
            text.text = "inter not loaded yet!";
        AdManagerDemo.Instant.ShowInterstitial(ID: 0);
    }

    public void ShowReward()
    {
        if (!AdManagerDemo.Instant.RewardIsLoaded(0))
            text.text = "Reward not loaded yet!";
        AdManagerDemo.Instant.ShowRewardVideo(ID: 0, showNoAds: true);
    }

    public void ShowAoa()
    {
        if (!AdManagerDemo.Instant.AoaIsLoaded(0))
            text.text = "Aoa not loaded yet!";
        AdManagerDemo.Instant.ShowAoa(ID: 0);
    }


    public void ShowBanner()
    { 
        AdManagerDemo.Instant.ShowBanner(ID: 0);
    }
    public void ShowMrecs()
    { 
        AdManagerDemo.Instant.ShowMRECs(ID: 0);
    }


    public void hidebanner()
    {
        AdManagerDemo.Instant.HideBanner(ID: 0);
    }
    public void HideMreacs()
    {
        AdManagerDemo.Instant.HideMRECs(ID: 0);
    }
}
