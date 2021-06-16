using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
	public ButtonObject btnPlay;


	private void Start()
	{
		Init();
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
