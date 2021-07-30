using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBirdPool : ObjectBasePool
{
	public ObjectBird prefab;

	private List<ObjectBird> view;
	private List<ObjectBird> list;

	private ObjectBird bird;

	private int n;
	private Vector3 vec;

	public void Init()
	{
		list = new List<ObjectBird>();
		view = new List<ObjectBird>();

		InitBird();
	}

	public void InitBird(bool isStartY = false)
	{
		prefab.obj.SetActive(false);
		ReturnViewAll();
	}

	public void UpdateBird()
	{
		for (n = 0; n < view.Count; n++)
		{
			view[n].UpdateBird();
		}
	}

	public void CreateBird(float y, int speed, int showTime)
	{
		bird = GetBird();
		bird.tran.SetParent(PlayManager.ins.stage.ground.tranScroll);
		bird.tran.localScale = Vector3.one;
		bird.tran.localRotation = Quaternion.identity;

		bird.SetData(speed, showTime, y);
	}

	private ObjectBird GetBird()
	{
		if (list.Count > 0)
		{
			bird = list[0];
			list.RemoveAt(0);
			view.Add(bird);

			return bird;
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
			ReturnBird(view[0]);
		}
	}

	public void ReturnBird(ObjectBird value)
	{
		list.Add(value);
		list[list.Count - 1].obj.SetActive(false);
		list[list.Count - 1].Init();
		value.tran.SetParent(tran);

		view.Remove(value);
	}
	
}
