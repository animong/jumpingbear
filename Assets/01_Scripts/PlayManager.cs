using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
	public static PlayManager ins;

	public Camera cam3d;
	public Camera cam2d;
	public Canvas canvas;

	public PlayUI ui;
	public PlayStage stage;
	public Player player;
	public LevelData data;
	
	public const float MAX_W = 70f;

	private void Awake()
	{
		if (PlayManager.ins != null)
		{
			Destroy(this);
			return;
		}
		PlayManager.ins = this;
		data = LevelData.ins;
	}

	void Start()
    {
#if UNITY_EDITOR
		if (SceneManager.ins == null)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("001_Lobby");

			//Debug.Log("Move Lobby Scene");
			return;
		}
#endif
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		
		Init();
	}

	private void Init(bool isFirst = true)
	{
		if (isFirst)
		{
			stage.Init();
			ui.Init();
		}
		else
		{
			stage.linePool.InitLine();
			stage.ground.Init();
		}
		player.Init();
		ui.InitData();

		//player.obj.SetActive(true);
		//stage.linePool.line.obj.SetActive(true);
	}
	
    void Update()
    {
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

		Init(false);
		//SceneManager.ins.LoadScene(SceneManager.SCENE.LOBBY);
	}

}
