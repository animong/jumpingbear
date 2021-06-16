using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStage : MonoBehaviour
{
	public PlayLine line;

	private Vector3 vec3;
	private Ray ray;
	private RaycastHit hit;

	public int layerLine;
	private int layerHit;
	public int layerCoin;

	public PlayLinePool linePool;
	public PlayGround ground;

	private PlayLine selLine;
	
	public void Init()
	{
		layerLine = 1 << LayerMask.NameToLayer("Line");
		layerHit = 1 << LayerMask.NameToLayer("Hit");
		layerCoin = 1 << LayerMask.NameToLayer("Coin");

		linePool.Init();
		ground.Init();
	}

	public void CheckLine(bool value)
	{
		if (PlayManager.ins.player.isFly) return; //유저 나는 중에는 드레그 안되도록

		if (selLine != null)
		{
			if (selLine == linePool.actLine)
			{
				vec3.x = selLine.render.GetPosition(1).x;
				vec3.y = selLine.render.GetPosition(1).y;
				PlayManager.ins.player.Shot(vec3);
				PlayManager.ins.player.guide.obj.SetActive(false);
			}

			selLine.ReturnLine();
			selLine = null;
		}

		if (value == false) return;
		
		//Debug.Log("CheckLine");

		ray = PlayManager.ins.cam3d.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 10000, layerLine) == false) return;
		//Debug.Log("PressLine");

		selLine = hit.transform.parent.GetComponent<PlayLine>();
		//if (selLine != linePool.actLine) selLine = null;
		if (selLine == null) return;
		//Debug.Log("SelectLine:" + tmpLine.obj.name);

		//PlayManager.ins.player.objGuide.SetActive(true);
		selLine.DragLine(hit.point);

		PlayManager.ins.player.guide.Show();
	}

	public Vector3 MouseHitVec()
	{
		ray = PlayManager.ins.cam3d.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit, 10000, layerHit) == false) return Vector3.zero;

		return hit.point;
	}

}
