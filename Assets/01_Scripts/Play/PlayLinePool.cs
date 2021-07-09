using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLinePool : MonoBehaviour
{
	public Transform tran;
	public PlayLine prefab;

	private List<PlayLine> list;
	private List<PlayLine> view;
	
	[HideInInspector]
	public PlayLine actLine;

	private PlayLine line;
	private Vector3 vec;

	private int idxCreate;
	private int startRan;

	private int n;

	public void Init()
	{
		list = new List<PlayLine>();
		view = new List<PlayLine>();

		InitLine();
	}

	public void ReturnViewAll()
	{
		int cnt = view.Count;
		for (int i = 0; i < cnt; i++)
		{
			ReturnLine(view[0]);
		}
		idxCreate = 0;
	}

	public void UpdateLine()
	{
		for (n = 0; n < view.Count; n++)
		{
			view[n].UpdateLine();
		}
	}

	public PlayLine GetCurrentLine()
	{
		line = null;
		for (int i = 0; i < view.Count; i++)
		{
			if (view[i].tran.position.y > PlayManager.ins.player.tran.position.y) continue;
			if (line != null && line.tran.position.y > view[i].tran.position.y) continue;
			line = view[i];
		}
		return line;
	}

	public void ReturnLine(PlayLine line)
	{
		list.Add(line);
		list[list.Count - 1].obj.SetActive(false);
		list[list.Count - 1].Init();
		line.tran.SetParent(tran);

		view.Remove(line);
	}

	public void InitLine(bool isStartY = false)
	{
		actLine = null;
		prefab.obj.SetActive(false);

		ReturnViewAll();

		vec = Vector3.zero;
		startRan = Random.Range(0, 2);

		if (isStartY) idxCreate = Mathf.FloorToInt(PlayManager.ins.data.startY / PlayManager.ins.data.lineGap);

		for (int i = 0; i < 5; i++)
		{
			CreateLine();
		}

		actLine = view[0];
	}
	
	private PlayLine GetLine()
	{
		if (list.Count > 0)
		{	//대기중 라인 있는 경우 생성된 리스트 전달
			line = list[0];
			list.RemoveAt(0);
			view.Add(line);

			return line;
		}

		//대기 라인이 없는 경우 생성해서 전달
		view.Add(GameObject.Instantiate<PlayLine>(prefab));
		return view[view.Count - 1];
	}

	public void CreateLine()
	{
		line = GetLine();

		line.data = PlayManager.ins.data.lines[PlayManager.ins.data.GetLineIdx(idxCreate)];
		
		line.Init();
		line.tran.SetParent(PlayManager.ins.stage.ground.tranScroll);

		line.tran.localScale = Vector3.one;
		line.tran.localRotation = Quaternion.identity;

		//위치 설정
		vec.y = idxCreate * PlayManager.ins.data.lineGap;
		vec.x = PlayManager.MAX_W * 0.25f;
		if (idxCreate % 2 == startRan) vec.x *= -1f;
		line.tran.localPosition = vec;

		//넓이 설정
		vec = line.render.GetPosition(0);
		float line_width;
		if (line.data.width_min != 0)
		{
			line_width = Random.Range(line.data.width_min, line.data.width_max);
		}
		else
		{
			line_width = PlayManager.ins.data.lineWidth;
		}
		vec.x = line_width * -0.5f;
		line.render.SetPosition(0, vec);

		vec = line.render.GetPosition(2);
		vec.x = line_width * 0.5f;//PlayManager.MAX_W * 0.25f;
		line.render.SetPosition(2, vec);

		line.render.startWidth = PlayManager.ins.data.lineHeight;

		//충돌영역 설정
		vec = line.col.size;
		vec.x = PlayManager.ins.data.lineWidth;// PlayManager.MAX_W * 0.5f;
		line.col.size = vec;

		//동전 등장 여부
		if (idxCreate % PlayManager.ins.data.coinCheck == 0
			&& PlayManager.ins.data.coinRate <= Random.Range(0, 101))
		{
			line.coin.obj.SetActive(true);	
		}
		else line.coin.obj.SetActive(false);

		line.InitPos();
		line.obj.SetActive(true);

		if (line.data.birdSpeed != 0)
		{	//새 설정
			PlayManager.ins.stage.birdPool.CreateBird(line.tran.position.y, line.data.birdSpeed, line.data.birdShowTime);
		}

		if (line.data.windSpeed != 0)
		{   //바람 설정
			PlayManager.ins.stage.windPool.CreateWind(line.tran.position.y, line.data.windSpeed);
		}

		idxCreate++;
	}
}
