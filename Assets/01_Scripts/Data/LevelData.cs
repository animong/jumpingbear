using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LineData
{
	/// <summary> üũ���� </summary>
	public int order;
	/// <summary> ���� ���� </summary>
	public int startY;
	/// <summary> üũ�ܰ�</summary>
	public int checkGap;
	/// <summary> ���� Ȯ��</summary>
	public int rate;
	/// <summary> ȭ��ǥ�ýð� </summary>
	public int time;

	/// <summary> �̵� �ӵ� </summary>
	public int move_speed;
	/// <summary> �¿� �̵� �ּҰ� </summary>
	public int move_X_min;
	/// <summary> �¿� �̵� �ִ밪 </summary>
	public int move_X_max;
	/// <summary> ���Ʒ� �̵� �ּ� �� </summary>
	public int move_y_min;
	/// <summary> ���Ʒ� �̵� �ִ� �� </summary>
	public int move_y_max;

	/// <summary> ���� ���� �ּҰ� </summary>
	public int width_min;
	/// <summary> ���� ���� �ִ밪  </summary>
	public int width_max;

	/// <summary> �ٶ� �ӵ� </summary>
	public int windSpeed;

	/// <summary> �� �ӵ� </summary>
	public int birdSpeed;
	/// <summary> �� ���� Ÿ�̹� </summary>
	public int birdShowTime;

}

public class LevelData : MonoBehaviour
{
	public static LevelData ins;
	
	[Header("���۳��� (�Ҽ���)"), Space(-10)]
	public float startY;
	[Header("�÷��� ���� : �߽� ���� (�Ҽ���)"), Space(-10)]
	public float playY;
	[Header("�߷� (�Ҽ���)"), Space(-10)]
	public float gravity;

	[Header("�� ���� ���� (�Ҽ���)")]
	public float lineGap;
	[Header("�� ���� (�Ҽ���)"), Space(-10)]
	public float lineWidth;
	[Header("�� ���� (�Ҽ���)"), Space(-10)]
	public float lineHeight;

	[Header("���� �� ���޵Ǵ� ���� (�Ҽ���)")]
	public float lineForceRate;
	[Header("���� �ִ� �Ÿ� (�Ҽ���)"), Space(-10)]
	public float lineDragMax;
	[Header("�о�� �� (�Ҽ���)"), Space(-10)]
	public float lineForce;

	[Header("���̵� �� ���� (����)")]
	public int guideLength;
	[Header("���̵� �� ������ ���� (����)"), Space(-10)]
	public int guideDetail;

	[Header("���� ���� ���� (����)")]
	public int coinCheck;
	[Header("���� ���� Ȯ�� 100% (����)")]
	public int coinRate;
	[Header("�ƹ����̳� �巡���ص� �����ϵ���")]
	public bool alltouch;

	[Header("���� Ÿ�� ����")]
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
			Debug.Log("��Ʈ �ҷ����� �������� ����");
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
			Debug.Log("��Ʈ �ҷ����� �������� ����");
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
	/// ���� ���� ���� ����
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
