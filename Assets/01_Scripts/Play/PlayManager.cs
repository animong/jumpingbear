using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
	public static PlayManager ins;

	public Camera cam3d;
	public Camera cam2d;
	public Canvas canvas;

	public ADManager mgrAD;
	public PlayUI ui;
	public PlayStage stage;
	public Player player;
	public PopupLoaing loading;

	public LevelData data;
	
	public const float MAX_W = 70f;
	private bool is_init;

	public const string STR_LEADER = "CgkI4ZSUsegQEAIQBA";
	public bool is_play;

	//public const string STR_DISTANCE = "{0:000}m";
	public const string STR_DISTANCE = "{0:000}";

	/// <summary> 점수 체크를 라인 기준으로 할지 여부 </summary>
	public static bool IS_CHEK_LINE = true;

	private void Awake()
	{
		/*
#if UNITY_EDITOR
		if (SceneManager.ins == null)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("001_Lobby");

			//Debug.Log("Move Lobby Scene");
			return;
		}
#endif
		*/
		if (PlayManager.ins != null)
		{
			Destroy(this);
			return;
		}
		is_init = false;
		is_play = false;
		PlayManager.ins = this;
		//data = LevelData.ins;

		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		//UnityEngine.Caching.ClearCache();
	}

	void Start()
    {
		mgrAD.Init();
		
		ui.btnRetry.obj.SetActive(false);
		ui.btnRetryNow.obj.SetActive(false);

		ui.btnRanking.obj.SetActive(false);
		ui.btnShop.obj.SetActive(false);
		ui.btnChar.obj.SetActive(false);
		ui.btnOption.obj.SetActive(false);

		player.obj.SetActive(false);
		stage.birdPool.prefab.obj.SetActive(false);
		stage.linePool.prefab.obj.SetActive(false);
		stage.windPool.prefab.obj.SetActive(false);

		ui.txtCoin.text = string.Empty;
		ui.txtDistance.text = string.Empty;
		ui.txtTopDistance.text = string.Empty;
		ui.txtDistanceTop.text = string.Empty;

		ui.objTopDistance.SetActive(false);
		ui.objCoin.SetActive(false);
		ui.objGuide.SetActive(false);

		//CloudOnce.Cloud.OnCloudLoadComplete += LoadCloud;
		CloudOnce.Cloud.OnInitializeComplete += CloudInitComplete;
		
		CloudOnce.Cloud.Initialize(true, true);
		CloudOnce.Cloud.CloudSaveEnabled = true;
	}
	
	private void CloudInitComplete()
	{
		Debug.Log("CloudOnce Init");
		CloudOnce.Cloud.OnInitializeComplete -= CloudInitComplete;
		
		//CloudOnce.Cloud.OnCloudLoadComplete += LoadCloud;
		//CloudOnce.Cloud.Storage.Load();

		ui.btnRanking.obj.SetActive(true);
		ui.btnChar.obj.SetActive(true);
		//ui.btnOption.obj.SetActive(true);

		player.obj.SetActive(true);

		stage.birdPool.prefab.obj.SetActive(true);
		stage.linePool.prefab.obj.SetActive(true);
		stage.windPool.prefab.obj.SetActive(true);

		ui.objGuide.SetActive(true);

		InitPlay(true);
		is_play = true;
		//CloudOnce.Cloud.OnSignedInChanged += InitPlay;
		//CloudOnce.Cloud.SignIn();
	}
	
	private void InitPlay(bool value)
	{
		Debug.Log("Google Play Login");
		CloudOnce.Cloud.OnSignedInChanged -= InitPlay;
		if (value)
		{
			//ui.InitCoin();

			Init();
			LevelData.ins.loading = loading;
			LevelData.ins.LoadFirstLoad();
			
			is_init = true;
			
			//코인 정보 불러옴
			CloudOnce.Cloud.OnCloudLoadComplete += LoadCloud;
			CloudOnce.Cloud.Storage.Load();
			//ui.InitCoin();
		}
	}

	private void LoadCloud(bool value)
	{
		CloudOnce.Cloud.OnCloudLoadComplete -= LoadCloud;
		ui.InitVariables();
		ui.objCoin.SetActive(true);

		//리더보드에서 정보 불러옴
		CloudOnce.Cloud.Leaderboards.LoadScores(STR_LEADER, LoadScores);
	}

	private void LoadScores(UnityEngine.SocialPlatforms.IScore[] values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i].userID == CloudOnce.Cloud.PlayerID)
			{
				if (ui.topScore < (int)values[i].value)
				{
					ui.topScore = (int)values[i].value;
					ui.txtTopDistance.text = string.Format(STR_DISTANCE, ui.topScore);
					ui.objTopDistance.SetActive(true);
				}
				ui.txtDistanceTop.text = "TOP" + values[i].rank;
				break;
			}
		}
	}

	public void Init(bool isFirst = true, bool isStartY = true)
	{
		if (isFirst)
		{
			stage.Init();
			ui.Init();
		}
		else
		{
			stage.birdPool.ReturnViewAll();
			stage.windPool.ReturnViewAll();
			stage.linePool.InitLine(isStartY);
			stage.ground.Init(isStartY);
		}

		player.Init();
		ui.InitData();
		stage.linePool.actLine.FirstTake();
		//player.obj.SetActive(true);
		//stage.linePool.line.obj.SetActive(true);
	}
	
    void Update()
    {
		if (is_init == false) return;

		stage.linePool.UpdateLine();
		stage.birdPool.UpdateBird();
		stage.windPool.UpdateWind();

		if (ui.btnRetry.obj.activeSelf == true)return;

		player.UpdatePlayer();

		/*
		 	deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			nFPS = Mathf.CeilToInt(1.0f / deltaTime);
			if (backFPS != nFPS)
			{
				//str = string.Format(STR_NUM, Mathf.CeilToInt(1.0f / deltaTime));
				txtFPS.text = nFPS.ToString();
				backFPS = nFPS;
			}
		 */
	}

	public void GameOver()
	{
		Debug.Log("GAME OVER");
		is_play = false;

		if (ui.GetDistance() > ui.topScore)
		{
			ui.topScore = ui.GetDistance();
			ui.txtTopDistance.text = string.Format(STR_DISTANCE, ui.topScore);
		}

		//CloudOnce.Leaderboards.HigeScore.SubmitScore(ui.GetDistance());
		CloudOnce.Cloud.Leaderboards.SubmitScore(STR_LEADER, ui.GetDistance());
		//CloudOnce.CloudVariables.Coin = ui.GetCoin();

		//CloudOnce.Cloud.OnCloudSaveComplete += ShowRetryUI;
		CloudOnce.Cloud.Storage.Save();
		ShowRetryUI(true);
		//Init(false);
		//SceneManager.ins.LoadScene(SceneManager.SCENE.LOBBY);
	}

	private void ShowRetryUI(bool value)
	{
		CloudOnce.Cloud.OnCloudSaveComplete -= ShowRetryUI;

		is_play = true;
		ui.objIngame.SetActive(true);
		ui.objMain.SetActive(true);
		ui.objTitle.SetActive(false);
		ui.btnRetry.obj.SetActive(true);
		
		ui.btnChar.obj.SetActive(false);
		ui.btnRanking.obj.SetActive(false);
		ui.objGuide.SetActive(false);

		if (stage.linePool.jump_index != -1) ui.btnRetryNow.obj.SetActive(true);
	}

	public void RestartY()
	{
		Init(false, true);
	}
	/*
	public void OnApplicationPause(bool pause)
	{
		if (pause == true)
		{
			//CloudOnce.Cloud.OnCloudSaveComplete += ShowRetryUI;
			CloudOnce.Cloud.Storage.Save();
		}
	}
	public void OnApplicationQuit()
	{
		//CloudOnce.Cloud.OnCloudSaveComplete += ShowRetryUI;
		CloudOnce.Cloud.Storage.Save();
	}
	*/
}
