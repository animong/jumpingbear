using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWind : ObjectBase
{
	public float speed;
	private Vector3 vec;

	public void Init()
	{
		
	}

	public void SetData(float speed, float y)
	{
		this.speed = speed;
		
		vec = tran.position;
		vec.y = y;
		tran.position = vec;

		Show();
	}

	private void Show()
	{
		obj.SetActive(true);

		vec = tran.position;
		vec.x = PlayManager.MAX_W * (0.55f * (speed > 0 ? -1 : 1));
		tran.position = vec;

		tran.localRotation = Quaternion.identity;
		tran.Rotate(Vector3.up, (speed > 0) ? 90 : -90);
	}

}
