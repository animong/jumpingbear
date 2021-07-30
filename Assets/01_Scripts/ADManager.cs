using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class ADManager : MonoBehaviour
{
	private RewardedAd rewardedAd;

	private const string AD_KEY = "ca-app-pub-3940256099942544/5224354917"; //test
	//private const string AD_KEY = "ca-app-pub-5514917895492992/1775773339"; //real

	public void Init()
	{
		MobileAds.Initialize((a)=> { CreateAndLoadRewardedAd(); });
		//CreateAndLoadRewardedAd();
	}

	public void ShowAd()
	{
		if (rewardedAd.IsLoaded()) // 광고가 로드 되었을 때
		{
			rewardedAd.Show(); // 광고 보여주기
		}
	}

	public void CreateAndLoadRewardedAd() // 광고 다시 로드하는 함수
	{
		rewardedAd = new RewardedAd(AD_KEY);

		rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		AdRequest request = new AdRequest.Builder().Build();
		rewardedAd.LoadAd(request);
	}

	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{  // 사용자가 광고를 닫았을 때
		CreateAndLoadRewardedAd();  // 광고 다시 로드
	}

	private void HandleUserEarnedReward(object sender, Reward e)
	{   // 광고를 다 봤을 때
		PlayManager.ins.ui.RetryNow();
	}

}
