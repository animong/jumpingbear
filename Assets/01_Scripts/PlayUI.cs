using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
	public ButtonObject btnHitArea;
	public UnityEngine.UI.Text txtDistance;
	public UnityEngine.UI.Text txtCoin;

	public ButtonObject btnPause;

	private int num;
	[SerializeField]
	private int distance;
	[SerializeField]
	private int coin;

	public void Init()
	{
		btnHitArea.fncPress = PressHit;
		btnPause.btn.onClick.AddListener(ClickPause);
	}

	public void InitData()
	{
		coin = 0;
		distance = -1;
		UpdateDistance();
		UpdateCoin();
	}

	private void PressHit(bool value)
	{
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

	public void UpdateDistance()
	{
		num = Mathf.FloorToInt(PlayManager.ins.player.tran.localPosition.y);
		if (distance < num)
		{
			distance = num;
			txtDistance.text = string.Format("{0:000}m", num);
		}
	}

	private void ClickPause()
	{
		SceneManager.ins.LoadScene(SceneManager.SCENE.LOBBY);
	}
}
