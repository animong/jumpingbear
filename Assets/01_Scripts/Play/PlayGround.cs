using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGround : MonoBehaviour
{
	public Transform tran;
	public Transform tranScroll;

	private Vector3 vec;

	public float MIN_STAGE;



	public void Init(bool isStartY = false)
	{
		MIN_STAGE = PlayManager.ins.data.playY;
		vec = Vector3.zero;
		vec.y = MIN_STAGE;
		
		tranScroll.localPosition = vec;

		if (isStartY == true)
		{
			vec = tran.localPosition;
			vec.y = PlayManager.ins.data.startY;
			tran.localPosition = vec;
			UpdateMinPos();
		}
	}

	public void UpdateMinPos()
	{
		vec = tranScroll.localPosition;
		vec.y = (MIN_STAGE - tran.localPosition.y) - PlayManager.ins.player.tran.localPosition.y;
		tranScroll.localPosition = vec;
	}

	public void UpdateMoveTween()
	{
		vec = tranScroll.localPosition;
		vec.y = (MIN_STAGE - tran.localPosition.y) - PlayManager.ins.player.tran.localPosition.y;
		LeanTween.moveLocal(tranScroll.gameObject, vec, 0.5f).setEaseOutCirc();
	}
	
}
