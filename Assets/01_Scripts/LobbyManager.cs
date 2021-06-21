using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
	public ButtonObject btnPlay;
	public PopupLoaing loading;

	private void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		Init();
		LevelData.ins.loading = loading;
		LevelData.ins.LoadFirstLoad();
	}

	private void Init()
	{
		btnPlay.btn.onClick.AddListener(ClickPlay);
	}

	private void ClickPlay()
	{
		SceneManager.ins.LoadScene(SceneManager.SCENE.PLAY);
	}
	
}
