using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameObject obj;
	public Transform tran;
	public Transform tranMesh;

	public GameObject objGuide;
	public PlayerGuide guide;
	
	public Transform tranPoint0;
	public Transform tranPoint1;
	public Transform tranPoint2;
	
	public bool isFly;

	private Vector3 vec;

	private Vector3 force;

	private Vector3 vecMove;

	private Collider[] colHit;
	private int num;

	private PlayLine line;
	private PlayCoin coin;
	private ObjectBird bird;

	private Vector3 prevPos;
	private float ShotPos;

	public void Init(bool isRestart = false)
	{
		LineTake(true, isRestart);
		
		prevPos = vec;
		guide.Init();
	}
	
	/// <summary>
	/// 라인 착지
	/// </summary>
	private void LineTake(bool setX = true, bool isRestart = false)
	{
		force = Vector3.zero;
		isFly = false;
		//obj.SetActive(false);
		objGuide.SetActive(false);
		guide.obj.SetActive(false);

		tranMesh.localRotation = Quaternion.identity;

		vec = PlayManager.ins.stage.linePool.actLine.tran.position;
		if (setX == false) vec.x = tran.position.x;
		vec.y += PlayManager.ins.stage.linePool.actLine.render.startWidth * 0.5f;
		tran.position = vec;

		ShotPos = tran.localPosition.y;

		if(isRestart) PlayManager.ins.stage.ground.UpdateMoveTween();
		else UpdateCamPos();
	}

	public void SetForceStart(ref Vector2 vecOrg, ref Vector3 vecSet)
	{
		vecSet.y = vecOrg.y * -PlayManager.ins.data.lineForce * PlayManager.ins.data.lineForceRate;
		vecSet.x = vecOrg.x * -PlayManager.ins.data.lineForce * 0.3f * PlayManager.ins.data.lineForceRate;
	}

	public void Shot(Vector2 vForce)
	{
		SetForceStart(ref vForce, ref force);
		ShotPos = tran.localPosition.y;
		isFly = true;
	}

	public void UpdatePlayer()
	{
		if (isFly == false) return;
		
		//날아감
		force.y -= PlayManager.ins.data.gravity * Time.smoothDeltaTime;
		vecMove = tran.position;
		vecMove += force * Time.smoothDeltaTime;

		//바람에 의한 이동
		vecMove.x += PlayManager.ins.stage.windPool.CheckWind(tran.position.y);

		tran.position = vecMove;

		if (force.y < 0f) tranMesh.localRotation = Quaternion.identity; 

		colHit = Physics.OverlapCapsule(tranPoint1.position, tranPoint2.position, 1f, PlayManager.ins.stage.layerCoin);
		if (colHit != null)
		{
			for (num = 0; num < colHit.Length; num++)
			{
				if (colHit[num] == null || colHit[num].gameObject == null) continue;
				coin = colHit[num].GetComponent<PlayCoin>();
				if (coin == null) continue;
				coin.obj.SetActive(false);
				PlayManager.ins.ui.AddCoin();
			}
		}
		//새 충돌
		colHit = Physics.OverlapCapsule(tranPoint1.position, tranPoint2.position, 1f, PlayManager.ins.stage.layerBird);
		if (colHit != null)
		{
			for (num = 0; num < colHit.Length; num++)
			{
				if (colHit[num] == null || colHit[num].gameObject == null) continue;
				bird = colHit[num].transform.GetComponent<ObjectBird>();
				if (bird == null) continue;

				if (force.y > 0) force.y *= -1;
				force.y -= 5f;
				force.x *= 0.5f;
			}
		}

		//충돌 체크
		colHit = Physics.OverlapCapsule(tranPoint1.position, tranPoint2.position, 1f, PlayManager.ins.stage.layerLine);
		if (colHit != null)
		{
			for (num = 0; num < colHit.Length; num++)
			{
				if (colHit[num] == null || colHit[num].gameObject == null) continue;
				line = colHit[num].transform.parent.GetComponent<PlayLine>();
				if (line == null || line == PlayManager.ins.stage.linePool.actLine) continue;
				
				if(prevPos.y > tran.position.y 
					&& line.tran.position.y < tranPoint0.position.y
					&& tran.position.x > line.tran.position.x + line.render.GetPosition(0).x
					&& tran.position.x < line.tran.position.x + line.render.GetPosition(2).x
					)
				{   //착지
					force = Vector3.zero;
					//vec = tran.position;
					PlayManager.ins.stage.linePool.actLine = line;
					line.TakePlayer();
					LineTake(false);

					PlayManager.ins.stage.linePool.jump_index = line.pos_index;
					PlayManager.ins.stage.linePool.jump_x = line.tran.position.x;
					return;
				}
				else 
				{   //윗발판
					if (force.y > 0) force.y *= -1;
					force.y -= 5f;
					force.x *= 0.5f;
					break;
				}
				//Debug.Log(colHit[num].gameObject.name);
			}
		}
	
		prevPos = tran.position;
		//
		if (tran.position.y < -100
			|| PlayManager.MAX_W * -0.59f > tran.position.x
			|| PlayManager.MAX_W * 0.59f < tran.position.x)
		{   //게임 오버 초기화
			PlayManager.ins.GameOver();
			return;	
		}

		UpdateCamPos();
		PlayManager.ins.ui.UpdateDistance();
	}

	/// <summary>
	/// 화면 움직임
	/// </summary>
	private void UpdateCamPos()
	{
		if (ShotPos > tran.localPosition.y) return;
		PlayManager.ins.stage.ground.UpdateMinPos();
	}

}
