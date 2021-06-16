using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
	public static SceneManager ins;

	public enum SCENE
	{
		TITLE,
		LOBBY,
		PLAY,
	}

	public void Awake()
	{
		if (SceneManager.ins != null)
		{
			Destroy(gameObject);
			return;
		}
		SceneManager.ins = this;
		DontDestroyOnLoad(this);
	}

	public void LoadScene(SCENE value)
	{
		switch (value)
		{
			case SCENE.TITLE:
				break;
			case SCENE.LOBBY:
				UnityEngine.SceneManagement.SceneManager.LoadScene("001_Lobby");
				break;
			case SCENE.PLAY:
				UnityEngine.SceneManagement.SceneManager.LoadScene("002_Play");
				break;
		}
	}

	private IEnumerator DoLoadScene()
	{


		yield break;
	}


}
