using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    static public GameManager me;
    public List<HobbyStruct> hobbies;
	public List<string> names;
	public List<string> lastNames;
	public List<string> personalities;

	public GameObject personPrefab;
	public GameObject textHolderPrefab;
	public int population;

	public Vector3 min;
	public Vector3 max;

	public GameObject connectionA;
	public GameObject connectionB;

	public GameObject linePrefab;
	public List<GameObject> lines;
	private GameObject lineToRemove;

	public int totalCouples = 0;
	public float spawnInterval;
	private float spawnTimer = 0;

	public TextMeshProUGUI score;

	public int state;
	public int tutorial;
	public int pregame;
	public int game;
	public float tutInterval;
	private float tutTimer = 0;
	private int tutIndex = 0;

	public GameObject maryPrefab;
	public GameObject celinePrefab;
	public List<GameObject> tuts;
	private GameObject mary;
	private GameObject celine;

	private void Start()
	{
		me = this;
		state = tutorial;
		tutTimer = tutInterval;
		// spawn mary
		mary = Instantiate(maryPrefab);
		mary.transform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
		GameObject textHolder = Instantiate(textHolderPrefab);
		PersonScript psMary = mary.GetComponent<PersonScript>();
		TextHolderScript ths = textHolder.GetComponent<TextHolderScript>();
		psMary.textHolder = textHolder;
		psMary.namae_text = ths.namae;
		psMary.hobby_text = ths.hobby;
		psMary.log_text = ths.log;
		psMary.personality_text = ths.personality;
		psMary.idealType_text = ths.idealType;
		psMary.ratio_text = ths.ratio;

		// spawn celine
		celine = Instantiate(celinePrefab);
		celine.transform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
		GameObject textHolder_celine = Instantiate(textHolderPrefab);
		PersonScript psCeline = celine.GetComponent<PersonScript>();
		TextHolderScript ths_celine = textHolder_celine.GetComponent<TextHolderScript>();
		psCeline.textHolder = textHolder_celine;
		psCeline.namae_text = ths_celine.namae;
		psCeline.hobby_text = ths_celine.hobby;
		psCeline.log_text = ths_celine.log;
		psCeline.personality_text = ths_celine.personality;
		psCeline.idealType_text = ths_celine.idealType;
		psCeline.ratio_text = ths_celine.ratio;
	}

	private void Update()
	{
		if (state == tutorial)
		{
			if (tutTimer > 0)
			{
				tutTimer -= Time.deltaTime;
			}
			else
			{
				tutTimer = tutInterval;
				tutIndex++;
				if (tutIndex < tuts.Count)
				{
					tuts[tutIndex - 1].SetActive(false);
					tuts[tutIndex].SetActive(true);
				}
				else
				{
					tuts[tuts.Count - 1].SetActive(false);
				}
			}
			if (mary == null && celine == null)
			{
				state = pregame;
			}
		}
		else if (state == pregame)
		{
			foreach(GameObject tut in tuts)
			{
				tut.SetActive(false);
			}
			//Destroy(mary.GetComponent<PersonScript>().textHolder);
			//Destroy(celine.GetComponent<PersonScript>().textHolder);
			//Destroy(mary);
			//Destroy(celine);
			for (int i = 0; i < population; i++)
			{
				GameObject person = Instantiate(personPrefab);
				person.transform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
				GameObject textHolder = Instantiate(textHolderPrefab);
				PersonScript ps = person.GetComponent<PersonScript>();
				TextHolderScript ths = textHolder.GetComponent<TextHolderScript>();
				ps.textHolder = textHolder;
				ps.namae_text = ths.namae;
				ps.hobby_text = ths.hobby;
				ps.log_text = ths.log;
				ps.personality_text = ths.personality;
				ps.idealType_text = ths.idealType;
				ps.ratio_text = ths.ratio;
				if (i == population - 1)
				{
					state = game;
				}
			}
		}
		MakeConnection();
		if (state == game)
		{
			if (spawnTimer > 0)
			{
				spawnTimer -= Time.deltaTime;
			}
			else
			{
				SpawnPeople();
				spawnTimer = spawnInterval;
			}
			score.text = "KPI: " + (totalCouples * 5).ToString();
		}
		
	}

	private void MakeConnection()
	{
		if (connectionA != null && connectionB != null)
		{
			PersonScript psA = connectionA.GetComponent<PersonScript>();
			PersonScript psB = connectionB.GetComponent<PersonScript>();
			
			if (AlreadyConnected())
			{
				foreach(GameObject line in lines)
				{
					LineScript ls = line.GetComponent<LineScript>();
					if ((ls.a == connectionA && ls.b == connectionB) ||
						(ls.b == connectionA && ls.a == connectionB))
					{
						line.GetComponent<LineRenderer>().enabled = false;
						lineToRemove = line;
						connectionA.GetComponent<SpriteRenderer>().color = psA.defaultColor;
						connectionB.GetComponent<SpriteRenderer>().color = psB.defaultColor;
						psA.isConnectTo.Remove(connectionB);
						psB.isConnectTo.Remove(connectionA);
					}
				}
				lines.Remove(lineToRemove);
				lineToRemove = null;
				connectionA = null;
				connectionB = null;
			}
			else
			{
				psA.isConnectTo.Add(connectionB);
				psB.isConnectTo.Add(connectionA);
				connectionA.GetComponent<SpriteRenderer>().color = psA.defaultColor;
				connectionB.GetComponent<SpriteRenderer>().color = psB.defaultColor;
				GameObject line = Instantiate(linePrefab);
				lines.Add(line);
				line.GetComponent<LineRenderer>().SetPosition(0, connectionA.transform.position);
				line.GetComponent<LineRenderer>().SetPosition(1, connectionB.transform.position);
				line.GetComponent<LineScript>().a = connectionA;
				line.GetComponent<LineScript>().b = connectionB;
				connectionA = null;
				connectionB = null;
			}
		}
	}

	private bool AlreadyConnected()
	{
		PersonScript psA = connectionA.GetComponent<PersonScript>();
		for (int i = 0; i < psA.isConnectTo.Count; i++)
		{
			if (connectionB == psA.isConnectTo[i])
			{
				return true;
			}
		}
		return false;
	}

	private void SpawnPeople()
	{
		spawnInterval *= 0.99f;
		GameObject person = Instantiate(personPrefab);
		person.transform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 0);
		GameObject textHolder = Instantiate(textHolderPrefab);
		PersonScript ps = person.GetComponent<PersonScript>();
		TextHolderScript ths = textHolder.GetComponent<TextHolderScript>();
		ps.textHolder = textHolder;
		ps.namae_text = ths.namae;
		ps.hobby_text = ths.hobby;
		ps.log_text = ths.log;
		ps.personality_text = ths.personality;
		ps.idealType_text = ths.idealType;
		ps.ratio_text = ths.ratio;
	}
}
