using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
	public ButtonObject btnHitArea;

	public UnityEngine.UI.Text txtDistance;
	public UnityEngine.UI.Text txtDistanceTop;

	public UnityEngine.UI.Text txtCoin;
	public UnityEngine.UI.Text txtTopDistance;
	
	public GameObject objIngame;
	public GameObject objMain;

	public GameObject objTopDistance;
	public GameObject objCoin;

	public GameObject objTitle;
	public ButtonObject btnRetry;
	public ButtonObject btnRetryNow;

	public ButtonObject btnChar;
	public ButtonObject btnShop;
	public ButtonObject btnRanking;
	public ButtonObject btnOption;

	public ButtonObject btnPause;

	public GameObject objGuide;

	//
	private int num;
	[SerializeField]
	private int distance;
	//[SerializeField]
	//private int coin;
	
	private CloudOnce.CloudPrefs.CloudInt cloudCoin;

	private int coin
	{
		get { return cloudCoin.Value; }
		set { cloudCoin.Value = value; }
	}

	private CloudOnce.CloudPrefs.CloudInt cloudScore;

	public int topScore
	{
		get { return cloudScore.Value; }
		set { cloudScore.Value = value; }
	}
	
	public void Init()
	{
		btnHitArea.fncPress = PressHit;
		btnPause.btn.onClick.AddListener(ClickPause);
		btnRetry.btn.onClick.AddListener(ClickRetry);
		btnRetryNow.btn.onClick.AddListener(ClickRetryNow);
		btnRanking.btn.onClick.AddListener(ClickRanking);

		objMain.SetActive(true);
		objIngame.SetActive(false);

		btnRetry.obj.SetActive(false);
		btnRetryNow.obj.SetActive(false);
	}
	
	public void InitVariables()
	{
		objGuide.SetActive(true);

		cloudCoin = new CloudOnce.CloudPrefs.CloudInt("Coin", CloudOnce.PersistenceType.Latest);//CloudOnce.CloudVariables.Coin;
		UpdateCoin();

		cloudScore = new CloudOnce.CloudPrefs.CloudInt("Score", CloudOnce.PersistenceType.Latest);
		topScore = topScore;
		txtTopDistance.text = string.Format(PlayManager.STR_DISTANCE, topScore);
		objTopDistance.SetActive(true);
	}

	public int GetDistance() { return distance; }
	public int GetCoin() { return coin; }

	public void InitData()
	{
		distance = -1;
		UpdateDistance();
		//UpdateCoin();
	}

	private void PressHit(bool value)
	{
		if (btnRetry.obj.activeSelf == true) return;
		if (PlayManager.ins.is_play == false) return;

		if (value == true && objMain.activeSelf)
		{
			objMain.SetActive(false);
			objIngame.SetActive(true);
			btnRetry.obj.SetActive(false);
			btnRetryNow.obj.SetActive(false);

			UpdateDistance(0);
		}
		
		PlayManager.ins.stage.CheckLine(value);
	}

	public void AddCoin()
	{
		coin++;
		UpdateCoin();
	}

	private void UpdateCoin()
	{
		txtCoin.text = string.Format("{0:000}", coin);
	}

	public void UpdateDistance(int value = 0)
	{
		if(distance < 0) txtDistance.text = string.Format(PlayManager.STR_DISTANCE, 0);
		if (PlayManager.IS_CHEK_LINE)
		{
			if (distance < value)
			{
				distance = value;
				txtDistance.text = string.Format(PlayManager.STR_DISTANCE, distance);
			}
			return;
		}
		num = Mathf.FloorToInt(PlayManager.ins.player.tran.localPosition.y);
		if (distance < num)
		{
			distance = num;
			txtDistance.text = string.Format(PlayManager.STR_DISTANCE, distance);
		}
	}
	
	private void ClickPause()
	{
		SceneManager.ins.LoadScene(SceneManager.SCENE.LOBBY);
	}

	private void ClickRetry()
	{
		PlayManager.ins.Init(false);
		btnRetry.obj.SetActive(false);
		btnRetryNow.obj.SetActive(false);

		txtDistance.text = string.Empty;
		//objMain.SetActive(false);
		btnChar.obj.SetActive(true);
		btnRanking.obj.SetActive(true);
		objTitle.SetActive(true);
	}

	private void ClickRetryNow()
	{
		//ca-app-pub-3940256099942544/5224354917
#if UNITY_EDITOR
		RetryNow();
#else
		PlayManager.ins.mgrAD.ShowAd();
#endif
	}

	public void RetryNow()
	{
		PlayManager.ins.stage.linePool.CreateLine(true);
		PlayManager.ins.player.Init(true);
		PlayManager.ins.stage.linePool.actLine.FirstTake();

		btnRetry.obj.SetActive(false);
		btnRetryNow.obj.SetActive(false);
		objMain.SetActive(false);
	}

	private void ClickRanking()
	{
		CloudOnce.Cloud.Leaderboards.ShowOverlay();
		//CloudOnce.Leaderboards.HigeScore.ShowOverlay();
	}
}
