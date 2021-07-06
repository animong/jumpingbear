using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBird : ObjectBase
{
	public Transform tranBody;

	private float speed;
	private float delayTime;

	private float showTime;
	private Vector3 vec;

	public void Init()
	{
		
	}

	public void SetData(float speed, float delayTime, float y)
	{
		this.speed = speed;
		this.delayTime = delayTime;

		vec = tran.position;
		vec.y = y + PlayManager.ins.data.lineGap - 10;// 10;
		tran.position = vec;

		showTime = Time.time;
		Show();
	}

	private void Show()
	{
		obj.SetActive(true);

		vec = tran.position;
		vec.x = PlayManager.MAX_W * (0.7f* (speed > 0 ? -1 : 1));
		tran.position = vec;

		tranBody.localRotation = Quaternion.identity;
		if(speed > 0) tranBody.Rotate(Vector3.up, 180);

	}

	public void UpdateBird()
	{
		if (Time.time < showTime) return;

		vec = tran.position;
		vec.x += (speed * Time.smoothDeltaTime);

		tran.position = vec;
		
		if (vec.x < PlayManager.MAX_W * -0.75f 
			|| vec.x > PlayManager.MAX_W * 0.75f)
		{	//화면 밖으로 도착
			showTime = Time.time + delayTime;
			Show();
		}

		if (tran.position.y < -100)
		{	//지나가서 초기화
			Return();
			return;
		}
	}

	public void Return()
	{
		PlayManager.ins.stage.birdPool.ReturnBird(this);
	}
}
