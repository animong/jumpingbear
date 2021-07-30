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
		if (rewardedAd.IsLoaded()) // ���� �ε� �Ǿ��� ��
		{
			rewardedAd.Show(); // ���� �����ֱ�
		}
	}

	public void CreateAndLoadRewardedAd() // ���� �ٽ� �ε��ϴ� �Լ�
	{
		rewardedAd = new RewardedAd(AD_KEY);

		rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		rewardedAd.OnAdClosed += HandleRewardedAdClosed;

		AdRequest request = new AdRequest.Builder().Build();
		rewardedAd.LoadAd(request);
	}

	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{  // ����ڰ� ���� �ݾ��� ��
		CreateAndLoadRewardedAd();  // ���� �ٽ� �ε�
	}

	private void HandleUserEarnedReward(object sender, Reward e)
	{   // ���� �� ���� ��
		PlayManager.ins.ui.RetryNow();
	}

}
