using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LineData
{
	/// <summary> 체크순서 </summary>
	public int order;
	/// <summary> 등장 높이 </summary>
	public int startY;
	/// <summary> 체크단계</summary>
	public int checkGap;
	/// <summary> 설정 확률</summary>
	public int rate;
	/// <summary> 화면표시시간 </summary>
	public int time;

	/// <summary> 이동 속도 </summary>
	public int move_speed;
	/// <summary> 좌우 이동 최소값 </summary>
	public int move_X_min;
	/// <summary> 좌우 이동 최대값 </summary>
	public int move_X_max;
	/// <summary> 위아래 이동 최소 값 </summary>
	public int move_y_min;
	/// <summary> 위아래 이동 최대 값 </summary>
	public int move_y_max;

	/// <summary> 수정 넓이 최소값 </summary>
	public int width_min;
	/// <summary> 수정 넓이 최대값  </summary>
	public int width_max;

	/// <summary> 바람 속도 </summary>
	public int windSpeed;

	/// <summary> 새 속도 </summary>
	public int birdSpeed;
	/// <summary> 새 등장 타이밍 </summary>
	public int birdShowTime;

}

public class LevelData : MonoBehaviour
{
	public static LevelData ins;
	
	[Header("시작높이 (소수점)"), Space(-10)]
	public float startY;
	[Header("플레이 높이 : 중심 기준 (소수점)"), Space(-10)]
	public float playY;
	[Header("중력 (소수점)"), Space(-10)]
	public float gravity;

	[Header("선 생성 간격 (소수점)")]
	public float lineGap;
	[Header("선 넓이 (소수점)"), Space(-10)]
	public float lineWidth;
	[Header("선 굵기 (소수점)"), Space(-10)]
	public float lineHeight;

	[Header("당기는 힘 전달되는 비율 (소수점)")]
	public float lineForceRate;
	[Header("당기는 최대 거리 (소수점)"), Space(-10)]
	public float lineDragMax;
	[Header("밀어내는 힘 (소수점)"), Space(-10)]
	public float lineForce;

	[Header("가이드 선 길이 (정수)")]
	public int guideLength;
	[Header("가이드 선 촘촘한 정도 (정수)"), Space(-10)]
	public int guideDetail;

	[Header("동전 등장 간격 (정수)")]
	public int coinCheck;
	[Header("동전 등장 확률 100% (정수)")]
	public int coinRate;
	[Header("아무곳이나 드래그해도 동작하도록")]
	public bool alltouch;

	[Header("라인 타입 정보")]
	public List<LineData> lines;

	public PopupLoaing loading;

	private bool is_load;
	private int load_cnt;
	private bool is_first_load;
	private bool is_restart;

	public void Awake()
	{
		if (LevelData.ins != null && LevelData.ins != this)
		{
			Destroy(gameObject);
			return;
		}

		LevelData.ins = this;
		DontDestroyOnLoad(gameObject);

		load_cnt = 0;
		is_load = false;
		is_first_load = false;

		DataInit();

		//LoadData();
	}

	private void DataInit()
	{
		startY = 0;

		playY = -20;
		gravity = 80f;

		lineGap = 30;
		lineWidth = 25;
		lineHeight = 1f;

		lineForceRate = 0.3f;
		lineDragMax = 10;
		lineForce = 50f;

		guideLength = 15;
		guideDetail = 5;

		coinCheck = 2;
		coinRate = 50;

		alltouch = true;

		lines = new List<LineData>();

		lines.Add(new LineData());
		lines[lines.Count - 1].order = 100;
		lines[lines.Count - 1].startY = 50;
		lines[lines.Count - 1].checkGap = 5;
		lines[lines.Count - 1].time = 5;


		lines.Add(new LineData());
		lines[lines.Count - 1].order = 150;
		lines[lines.Count - 1].startY = 10;
		lines[lines.Count - 1].checkGap = 2;
	}

	public void LoadFirstLoad()
	{
		if (is_first_load == true) return;
		LoadData();
	}

	public void LoadData(bool restart = false)
	{
		if (is_load == true) return;
		
		is_restart = restart;

		loading.obj.SetActive(true);
		UnityWebRequest.ClearCookieCache();
		is_load = true;
		load_cnt = 0;
		StartCoroutine(LoadLevel());
		StartCoroutine(LoadLevelLine());
	}

	private IEnumerator LoadLevel()
	{
		UnityWebRequest www;

		//https://docs.google.com/spreadsheets/d/1sXlKMiNriLeDi1XR86dRdM2upowCTBepGoVmjAPs1VY/edit?usp=sharing
		//https://docs.google.com/spreadsheets/d/1sXlKMiNriLeDi1XR86dRdM2upowCTBepGoVmjAPs1VY/edit#gid=0

		//www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vR_AEgug0cCtyoSuG4Qe-3RdGI3PdogQzsA1n-XgzPmde730iGQLQBXVB_50PbR0YYuZpXI7u3IqIzl/pub?output=csv&gid=0");
		www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/1sXlKMiNriLeDi1XR86dRdM2upowCTBepGoVmjAPs1VY/export?format=csv&gid=0");
		
		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log("시트 불러오는 과정에서 에러");
			is_load = false;
			yield break;
		}
		
		Debug.Log(www.downloadHandler.text);

		string[] cell = www.downloadHandler.text.Split('\n');
		string[] row;
		int i, j;
		
		int key_idx = 0;
		int value_idx = 0;

		for (i = 0; i < cell.Length; i++)
		{
			row = cell[i].Split(',');
			
			if (i == 0)
			{
				for (j = 0; j < row.Length; j++)
				{
					if (row[j].Contains("#")) continue;
					if (row[j].Contains("key")) key_idx = j;
					if (row[j].Contains("value")) value_idx = j;
				}
				continue;
			}

			switch (row[key_idx])
			{
				case "startY": startY = float.Parse(row[value_idx]); break;
				case "playY": playY = float.Parse(row[value_idx]); break;
				case "gravity": gravity = float.Parse(row[value_idx]); break;
				case "lineGap": lineGap = float.Parse(row[value_idx]); break;
				case "lineWidth": lineWidth = float.Parse(row[value_idx]); break;
				case "lineHeight": lineHeight = float.Parse(row[value_idx]); break;
				case "lineForceRate": lineForceRate = float.Parse(row[value_idx]); break;
				case "lineDragMax": lineDragMax = float.Parse(row[value_idx]); break;
				case "lineForce": lineForce = float.Parse(row[value_idx]); break;
				case "guideLength": guideLength = int.Parse(row[value_idx]); break;
				case "guideDetail": guideDetail = int.Parse(row[value_idx]); break;
				case "coinCheck": coinCheck = int.Parse(row[value_idx]); break;
				case "coinRate": coinRate = int.Parse(row[value_idx]); break;
				case "alltouch": alltouch = int.Parse(row[value_idx]) == 1; break;
			}
		}
		
		www.Dispose();
		www = null;
		
		CheckLoad();
		yield break;
	}

	private IEnumerator LoadLevelLine()
	{
		UnityWebRequest www;
		string[] cell;
		string[] row;
		int i, j;
		int key_idx = 0;

		//www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vR_AEgug0cCtyoSuG4Qe-3RdGI3PdogQzsA1n-XgzPmde730iGQLQBXVB_50PbR0YYuZpXI7u3IqIzl/pub?output=csv
		//www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vR_AEgug0cCtyoSuG4Qe-3RdGI3PdogQzsA1n-XgzPmde730iGQLQBXVB_50PbR0YYuZpXI7u3IqIzl/pub?output=csv&gid=1422264379");
		www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/1sXlKMiNriLeDi1XR86dRdM2upowCTBepGoVmjAPs1VY/export?format=csv&gid=1829669208");

		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log("시트 불러오는 과정에서 에러");
			is_load = false;
			yield break;
		}

		Debug.Log(www.downloadHandler.text);

		cell = www.downloadHandler.text.Split('\n');
		string[] cell_menu = new string[] { };

		for (i = 0; i < cell.Length; i++)
		{
			row = cell[i].Split(',');

			if (i == 0)
			{
				cell_menu = cell[i].Split(',');
				lines.Clear();
				for (j = 0; j < row.Length; j++)
				{
					if (row[j].Contains("#")) continue;
					if (row[j].Contains("key")) { key_idx = j; continue; }

					lines.Add(new LineData());
				}
				continue;
			}

			for (j = 0; j < row.Length; j++)
			{
				if (cell_menu[j].Contains("#")) continue;
				if (cell_menu[j].Contains("key")) continue;

				switch (row[key_idx])
				{
					case "order": lines[j - 2].order = int.Parse(row[j]); break;
					case "startY": lines[j - 2].startY = int.Parse(row[j]); break;
					case "rate": lines[j - 2].rate = int.Parse(row[j]); break;
					case "checkGap": lines[j - 2].checkGap = int.Parse(row[j]); break;
					case "time": lines[j - 2].time = int.Parse(row[j]); break;
					case "move_speed": lines[j - 2].move_speed = int.Parse(row[j]); break;
					case "move_X_min": lines[j - 2].move_X_min = int.Parse(row[j]); break;
					case "move_X_max": lines[j - 2].move_X_max = int.Parse(row[j]); break;
					case "move_y_min": lines[j - 2].move_y_min = int.Parse(row[j]); break;
					case "move_y_max": lines[j - 2].move_y_max = int.Parse(row[j]); break;
					case "width_min": lines[j - 2].width_min = int.Parse(row[j]); break;
					case "width_max": lines[j - 2].width_max = int.Parse(row[j]); break;
					case "windSpeed": lines[j - 2].windSpeed = int.Parse(row[j]); break;
					case "birdSpeed": lines[j - 2].birdSpeed = int.Parse(row[j]); break;
					case "birdShowTime": lines[j - 2].birdShowTime = int.Parse(row[j]); break;
				}
			}
		}

		www.Dispose();
		www = null;
		
		CheckLoad();
		yield break;
	}

	private void CheckLoad()
	{
		load_cnt++;

		if (load_cnt != 2) return;

		lines.Sort(AlignLine);
		
		loading.obj.SetActive(false);
		is_load = false;
		is_first_load = true;

		if (PlayManager.ins != null /*&& is_restart*/) PlayManager.ins.GameOver();
	}
	/// <summary>
	/// 라인 생성 설정 내용
	/// </summary>
	/// <param name="idxCreate"></param>
	/// <returns></returns>
	public int GetLineIdx(int idxCreate)
	{
		for (int i = 0; i < lines.Count; i++)
		{
			if (idxCreate * lineGap > lines[i].startY
				&& idxCreate % lines[i].checkGap == 0
				&& Random.Range(0,100) < lines[i].rate)
			{
				return i;
			}
		}
		return lines.Count - 1;
	}

	private int AlignLine(LineData a, LineData b)
	{
		return b.order.CompareTo(a.order);
	}

}
