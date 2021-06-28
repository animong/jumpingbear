using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWindPool : ObjectBasePool
{
	private List<ObjectWind> list;
	private List<ObjectWind> view;

	public ObjectWind prefab;

	private ObjectWind wind;
	
	private int n;
	private int l;

	public void Init()
	{
		list = new List<ObjectWind>();
		view = new List<ObjectWind>();

		InitWind();
	}

	public void InitWind(bool isStartY = false)
	{
		prefab.obj.SetActive(false);
		ReturnViewAll();
	}

	public void UpdateWind()
	{
		for (l = 0; l < view.Count; l++)
		{
			if (view[l].tran.position.y < -100)
			{   //지나가서 초기화
				ReturnWind(view[l]);
				return;
			}
		}
	}

	public ObjectWind CreateWind(float y, int speed)
	{
		wind = GetWind();
		wind.tran.SetParent(PlayManager.ins.stage.ground.tranScroll);
		wind.tran.localScale = Vector3.one;
		wind.tran.localRotation = Quaternion.identity;

		wind.SetData(speed, y);
		return wind;
	}

	public float CheckWind(float y)
	{
		for (n = 0; n < view.Count; n++)
		{
			if (view[n].tran.position.y > y
				|| view[n].tran.position.y + PlayManager.ins.data.lineGap < y) continue;
			
			//Debug.Log(view[n].speed * Time.smoothDeltaTime);
			return view[n].speed * Time.smoothDeltaTime;
		}
		return 0f;
	}

	private ObjectWind GetWind()
	{
		if (list.Count > 0)
		{
			wind = list[0];
			list.RemoveAt(0);
			view.Add(wind);

			return wind;
		}

		view.Add(GameObject.Instantiate(prefab));
		view[view.Count - 1].Init();
		return view[view.Count - 1];
	}

	public void ReturnViewAll()
	{
		int cnt = view.Count;
		for (int i = 0; i < cnt; i++)
		{
			ReturnWind(view[0]);
		}
	}

	public void ReturnWind(ObjectWind value)
	{
		list.Add(value);
		list[list.Count - 1].obj.SetActive(false);
		list[list.Count - 1].Init();
		value.tran.SetParent(tran);

		view.Remove(value);
	}
}
