using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLine : MonoBehaviour
{
	public GameObject obj;
	public Transform tran;

	public LineRenderer render;
	public BoxCollider col;

	public PlayCoin coin;
	
	private bool isDrag;
	private bool isReturn;
	private bool isTake;

	private Vector3[] arrPos;

	public LineData data;
	
	private float TakeTime;

	private Vector3 vec3;
	private bool move_up;
	private bool move_left;

	private float move_x_min;
	private float move_x_max;
	private float move_y_min;
	private float move_y_max;

	public int pos_index;

	public void Init()
	{
		isDrag = false;
		isReturn = false;
		isTake = false;

		arrPos = new Vector3[render.positionCount];
	}

	public void InitPos()
	{
		TakeTime = 0;

		for (int i = 0; i < arrPos.Length; i++)
		{
			arrPos[i] = render.GetPosition(i);
		}

		move_x_min = data.move_X_min + tran.localPosition.x;
		move_x_max = data.move_X_max + tran.localPosition.x;
		move_y_min = data.move_y_min + tran.localPosition.y;
		move_y_max = data.move_y_max + tran.localPosition.y;
	}

	public void DragLine(Vector3 hitPoint)
	{
		isDrag = true;
		isReturn = false;
		StartCoroutine(DoDragLine(hitPoint));
	}
	
	private IEnumerator DoDragLine(Vector3 hitPoint)
	{
		if (this == PlayManager.ins.stage.linePool.actLine) PlayManager.ins.player.isFly = false;
		
		int center = 1;
		/*
		float dis = -1;
		float center_dis = -1;
		int i;
		
		for (i = 1; i < render.positionCount - 1; i++)
		{
			dis = Vector3.Distance(render.GetPosition(i) + tran.position, hitPoint);
			if (center_dis == -1 || dis < center_dis)
			{
				center = i;
				center_dis = dis;
			}
		}
		*/
		//Vector3 startL = render.GetPosition(center);
		Vector3 dragL = Vector3.zero;

		//Vector3 startM = PlayManager.ins.stage.MouseHitVec();
		//Vector3 vec = Vector3.zero;
		Vector3 vecLook = Vector3.zero;
		hitPoint = PlayManager.ins.stage.MouseHitVec();

		while (isDrag)
		{
			yield return new WaitForEndOfFrame();
			//yield return new WaitForSeconds(0.01f);

			//dragL = startL + (PlayManager.ins.stage.MouseHitVec() - startM);
			
			if (LevelData.ins.alltouch)
			{
				dragL = (PlayManager.ins.stage.MouseHitVec() - hitPoint + arrPos[1]);// - tran.position;
			}
			else
			{
				dragL = PlayManager.ins.stage.MouseHitVec() - tran.position;
			}

			dragL.z = -2f;
			if (dragL.y > 0f) dragL.y = 0;
			if (dragL.y < -PlayManager.ins.data.lineDragMax) dragL.y = -PlayManager.ins.data.lineDragMax;
		
			render.SetPosition(center, dragL);
			/*
			//vecLook = tran.position;
			vecLook = Vector3.zero;
			vecLook.x = render.GetPosition(0).x;
			vecLook.x += (Vector3.Distance(render.GetPosition(0), dragL) / Vector3.Distance(render.GetPosition(2), dragL)) * (render.GetPosition(0).x * -2f);
			vecLook += tran.position;
			vecLook.y += 10;
			PlayManager.ins.player.tranMesh.LookAt(vecLook);
			PlayManager.ins.player.tranMesh.Rotate(Vector3.right, 90);
			*/

			if (this == PlayManager.ins.stage.linePool.actLine)
			{	//유저가 올라가 있는 라인 드래그시에만 플레이어 위치 이동
				
				if (Vector3.Distance(Vector3.zero, dragL) < 2f
					|| dragL.y > -1f)
				{
					PlayManager.ins.player.tranMesh.localRotation = Quaternion.identity;
				}
				else
				{
					dragL += tran.position;
					dragL.z = 0;
					PlayManager.ins.player.tran.position = dragL;

					vecLook = tran.position;
					vecLook.y += 10f;
					PlayManager.ins.player.tranMesh.LookAt(vecLook);
					PlayManager.ins.player.tranMesh.Rotate(Vector3.right, 90);
					PlayManager.ins.player.tranMesh.Rotate(Vector3.down, 160);
				}
			}

			PlayManager.ins.player.guide.UpdateGuideLine(render.GetPosition(1));

			/*
			for (i = 1; i < render.positionCount - 1; i++)
			{
				if (i == center) continue;
				
				//if (i < center)
				//{
					//vec = Vector3.Lerp(arrPos[0], dragL, 0.01f * (center - i));
				//	vec.y = dragL.y * ((float)i / (float)(center - 1));
				//}
				//else
				//{
					//vec = Vector3.Lerp(arrPos[arrPos.Length - 1], dragL, 0.01f * (i + center));
				//	vec.y = dragL.y * ((float)(center - 1) / (float)i);//((i - center) * 0.01f);
				//}

				//vec.x = arrPos[i].x;
				//render.SetPosition(i, vec);
				
				render.SetPosition(i, dragL);
			}
			*/
			//Debug.Log(Input.mousePosition);
		}

		yield break;
	}

	public void UpdateLine()
	{
		if (isTake) return;

		vec3 = tran.localPosition;

		if (data.move_X_min != 0)
		{
			vec3.x += ((float)data.move_speed * Time.smoothDeltaTime * 1f) * (move_left ? -1 : 1);
			tran.localPosition = vec3;
			if (move_left && vec3.x < move_x_min) move_left = false;
			if (!move_left && vec3.x > move_x_max) move_left = true;
		}

		if (data.move_y_min != 0)
		{
			vec3.y += ((float)data.move_speed * Time.smoothDeltaTime * 1f) * (move_up ? -1 : 1);
			tran.localPosition = vec3;
			if (move_up && vec3.y < move_y_min) move_up = false;
			if (!move_up && vec3.y > move_y_max) move_up = true;
		}
	}
	
	public void ReturnLine()
	{
		isDrag = false;
		isReturn = true;
		isTake = false;

		StartCoroutine(DoReturnLine());
	}

	private IEnumerator DoReturnLine()
	{
		float time = Time.time;
		int i;
		Vector3 vec;
		while (isReturn)
		{
			yield return new WaitForEndOfFrame();
			for (i = 0; i < arrPos.Length; i++)
			{
				vec = Vector3.Lerp(arrPos[i], render.GetPosition(i), Time.smoothDeltaTime * 20f);
				render.SetPosition(i, vec);
			}

			if (time + 0.2f < Time.time)
			{   //0.2초 동안 모션
				//isReturn = false;
				PlayManager.ins.stage.linePool.ReturnLine(this);
				PlayManager.ins.stage.linePool.CreateLine();
			}
		}

		yield break;
	}

	public void FirstTake() { isTake = true;	}

	public void TakePlayer()
	{
		TakeTime = Time.time;
		isTake = true;

		if (data.time == 0) return;

		//Debug.Log("TakePlayer");
		StartCoroutine(DoTake());
	}

	private IEnumerator DoTake()
	{
		float rate;
		Vector3 vec;

		while (isTake)
		{
			//Debug.Log("Time.time:" + Time.time + ",data.time:" + (TakeTime + data.time));
			if (Time.time > TakeTime + data.time)
			{
				OutLine();
				yield break;
			}
			
			//시간에의한 선 길이 점점 좁아 지도록
			rate = 1f - ((Time.time - TakeTime) / data.time);

			for (int i = 0; i < arrPos.Length; i++)
			{
				vec = arrPos[i];
				vec.x *= rate;
				render.SetPosition(i, vec);


				if (isDrag) continue;

				//Debug.Log(PlayManager.ins.player.tran.localPosition.x + "::"+ (vec.x + tran.localPosition.x));

				if (i == 0 && PlayManager.ins.player.tran.localPosition.x < vec.x + tran.localPosition.x)
				{
					OutLine();
					yield break;
				}
				if (i == 2 && PlayManager.ins.player.tran.localPosition.x > vec.x + tran.localPosition.x)
				{
					OutLine();
					yield break;
				}
			}
			

			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void OutLine()
	{
		isDrag = false;
		isReturn = false;
		isTake = false;

		for (int i = 0; i < arrPos.Length; i++)
		{
			render.SetPosition(i, arrPos[i]);
		}
		PlayManager.ins.stage.linePool.ReturnLine(this);
		PlayManager.ins.player.isFly = true;
		PlayManager.ins.player.guide.obj.SetActive(false);

	}

}
