using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersonScript : MonoBehaviour
{
	// TMPro things
    public GameObject textHolder;
    public TextMeshProUGUI namae_text;
    public TextMeshProUGUI hobby_text;
    public TextMeshProUGUI log_text;
    public TextMeshProUGUI personality_text;
    public TextMeshProUGUI idealType_text;
    public TextMeshProUGUI ratio_text;

	// thingys
	public string namae;
	public List<HobbyStruct> hobbies;
	public List<string> personalities;
	public float intimacy;
	public float passion;
	public float commitment;
	public float relpFactor; // for close friends to dating
	public List<string> idealType;

	// thingy amounts
	public int minHobby;
	public int maxHobby;
	public int minPersonality;
	public int maxPersonality;
	public int minIT; // min ideal type
	public int maxIT;

	// text
	private string logText;
	private List<GameObject> friends = new List<GameObject>(); // people this person may meet when doing a specific action

	// tick
	public float interval;
	private float timer;

	// offsets
	public float offX;
	public float offY;

	// connection
	public Color highlight;
	public Color defaultColor;
	public List<GameObject> isConnectTo;
	public List<GameObject> closeFriends; // people that have the exact same hobbies as this person
	public bool offTheMarket = false;

	private void Start()
	{
		if (GameManager.me.state == GameManager.me.game)
		{
			RollHobbies();
			RollPersonalities();
			RollIdealType();
			RollTriangleOfLove();
		}
		Show_Personality_Text();
		Show_IT_Text();
		Show_ratio_text();
		namae = GameManager.me.names[Random.Range(0, GameManager.me.names.Count)] + " " + GameManager.me.lastNames[Random.Range(0,GameManager.me.lastNames.Count)];
		defaultColor = GetComponent<SpriteRenderer>().color;
		timer = Random.Range(0, interval);

	}

	private void Update()
	{
		// disappear when off the market
		if (offTheMarket)
		{
			GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r,
															 GetComponent<SpriteRenderer>().color.g,
															 GetComponent<SpriteRenderer>().color.b,
															 GetComponent<SpriteRenderer>().color.a - 0.3f * Time.deltaTime);
			if (GetComponent<SpriteRenderer>().color.a <= 0)
			{
				GameManager.me.totalCouples++;
				Destroy(textHolder);
				Destroy(gameObject);
			}
		}

		// offset text holder
		textHolder.transform.position = new Vector3(transform.position.x + offX, transform.position.y + offY, 0);

		// show name
		namae_text.text = namae;

		// show hobby
		Update_Hobby_Text();

		// show log
		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}
		else
		{
			timer = interval;
			Update_Log_Text();
		}

		if (Input.GetMouseButtonDown(1))
		{
			GetComponent<SpriteRenderer>().color = defaultColor;
		}
		
	}

	private void OnMouseOver()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			if (Input.GetMouseButtonDown(0))
			{
				GetComponent<SpriteRenderer>().color = highlight;
				if (GameManager.me.connectionA == null)
				{
					GameManager.me.connectionA = gameObject;
				}
				else if (GameManager.me.connectionA != gameObject)
				{
					GameManager.me.connectionB = gameObject;
				}
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			PlayerScript.me.personGrabbed = gameObject;
			
		}
		else if (Input.GetMouseButtonUp(0))
		{
			PlayerScript.me.personGrabbed = null;
		}
	}


	private void RollHobbies()
	{
		List<HobbyStruct> availableList = new List<HobbyStruct>();
		CopyHobbyStructList(GameManager.me.hobbies, availableList);
		int hobbyAmount = Random.Range(minHobby, maxHobby + 1);
		for (int i = 0; i < hobbyAmount; i++)
		{
			int hobbyNum = Random.Range(0, availableList.Count);
			hobbies.Add(availableList[hobbyNum]);
			availableList.RemoveAt(hobbyNum);
		}
	}

	private void RollPersonalities()
	{
		List<string> availableList = new List<string>();
		CopyStringList(GameManager.me.personalities, availableList);
		int personalityAmount = Random.Range(minPersonality, maxPersonality + 1);
		for (int i = 0; i < personalityAmount; i++)
		{
			int personalityNum = Random.Range(0, availableList.Count);
			personalities.Add(availableList[personalityNum]);
			availableList.RemoveAt(personalityNum);
		}
	}

	private void RollIdealType()
	{
		List<string> availableList = new List<string>();
		CopyStringList(GameManager.me.personalities, availableList);
		int idealTypeAmount = Random.Range(minIT, maxIT + 1);
		for (int i = 0; i < idealTypeAmount; i++)
		{
			int idealTypeNum = Random.Range(0, availableList.Count);
			idealType.Add(availableList[idealTypeNum]);
			availableList.RemoveAt(idealTypeNum);
		}
	}

	private void CopyHobbyStructList(List<HobbyStruct> from, List<HobbyStruct> to)
	{
		for (int i = 0; i < from.Count; i++)
		{
			to.Add(from[i]);
		}
	}

	private void CopyStringList(List<string> from, List<string> to)
	{
		for (int i = 0; i < from.Count; i++)
		{
			to.Add(from[i]);
		}
	}

	private void Update_Hobby_Text()
	{
		hobby_text.text = "Hobby: \n";
		for (int i = 0; i < hobbies.Count; i++)
		{
			if (i > 0)
			{
				hobby_text.text += ", " + hobbies[i].name;
			}
			else
			{
				hobby_text.text += hobbies[i].name;
			}
		}
	}

	private void Update_Log_Text()
	{
		// roll to show personality text or hobby text
		int rnd = Random.Range(0, hobbies.Count + idealType.Count);
		if (rnd > hobbies.Count - 2) // show personality log
		{
			print("roll pass");
			// get the person who is close friend and is connected
			List<GameObject> potentialDates = new List<GameObject>();
			for (int i = 0; i < closeFriends.Count; i++)
			{
				for (int j = 0; j < isConnectTo.Count; j++)
				{
					PersonScript psClose = closeFriends[i].GetComponent<PersonScript>();
					PersonScript psConnect = isConnectTo[j].GetComponent<PersonScript>();
					if (psClose.namae == psConnect.namae)
					{
						potentialDates.Add(closeFriends[i]);
					}
				}
			}
			if (potentialDates.Count > 0) // if there is such person
			{
				print("potential date pass");
				// roll one date
				GameObject date = potentialDates[Random.Range(0, potentialDates.Count)];
				PersonScript psDate = date.GetComponent<PersonScript>();
				// get date's personality that fits this person's ideal type
				List<string> idealPsnlts = new List<string>();
				for (int i = 0; i < idealType.Count; i++)
				{
					for (int j = 0; j < psDate.personalities.Count; j++)
					{
						if (idealType[i] == psDate.personalities[j])
						{
							idealPsnlts.Add(idealType[i]);
						}
					}
				}
				if (idealPsnlts.Count > 0) // if date's personalities match this person's ideal type
				{
					// print("ideal personality pass");
					// // roll one ideal personality
					// string idealP = idealPsnlts[Random.Range(0, idealPsnlts.Count)];
					// // show text
					// logText = "Log: \n";
					// logText += namae + " finds " + psDate.namae + "'s being " + idealP + " very attractive.";
					// log_text.text = logText;
					// // find the line connecting them and add relationship
					FindLineWith(date);
					Show_HobbyLog();
				}
				else // if no match
				{
					// show hobby log
					Show_HobbyLog();
					FindLineWith(date);
				}
			}
			else // if no such person
			{
				// show hobby log
				Show_HobbyLog();
			}
		}
		else
		{
			// show hobby log
			Show_HobbyLog();
		}
	}

	private void Show_HobbyLog()
	{
		int randomHobbyNum = Random.Range(0, hobbies.Count);
		logText = "Log: \n";
		for (int i = 0; i < isConnectTo.Count; i++)
		{
			for (int j = 0; j < isConnectTo[i].GetComponent<PersonScript>().hobbies.Count; j++)
			{
				if (isConnectTo[i].GetComponent<PersonScript>().hobbies[j].name == hobbies[randomHobbyNum].name)
				{
					friends.Add(isConnectTo[i]);
				}
			}
		}
		if (friends.Count > 0) // doing it with another person
		{
			GameObject randomPerson = friends[Random.Range(0, friends.Count)];
			logText += namae + " " + hobbies[randomHobbyNum].socialText[Random.Range(0, hobbies[randomHobbyNum].socialText.Count)] + " " + randomPerson.GetComponent<PersonScript>().namae;
			log_text.text = logText;
			// find the line connecting them and add relationship
			FindLineWith(randomPerson);
		}
		else // doing it alone
		{
			logText += namae + " " + hobbies[randomHobbyNum].individualText[Random.Range(0, hobbies[randomHobbyNum].individualText.Count)];
			log_text.text = logText;
		}
		friends.Clear();
	}

	private void FindLineWith(GameObject person)
	{
		for (int i = 0; i < GameManager.me.lines.Count; i++)
		{
			LineScript ls = GameManager.me.lines[i].GetComponent<LineScript>();
			PersonScript psA = ls.a.GetComponent<PersonScript>();
			PersonScript psB = ls.b.GetComponent<PersonScript>();
			if ((psA.namae == namae && psB.namae == person.GetComponent<PersonScript>().namae) ||
				(psB.namae == namae && psA.namae == person.GetComponent<PersonScript>().namae))
			{
				if (ls.state == 0) // friends
				{
					ls.relpBetweenThem++;
				}
				else if (ls.state == 1) // close friends
				{
					ls.relpBetweenThem += CalculateRatio(psA, psB);
				}
			}
		}
	}

	private float CalculateRatio(PersonScript a, PersonScript b)
	{
		float relpIncreaseNum = 0;
		float intimacyDiff = Mathf.Abs(a.intimacy - b.intimacy) ;
		float passionDiff = Mathf.Abs(a.passion - b.passion) ;
		relpIncreaseNum = 1 / (intimacyDiff + passionDiff * relpFactor);
		print(relpIncreaseNum);
		return relpIncreaseNum;
	}

	private void Show_Personality_Text()
	{
		personality_text.text = "Personality: \n";
		for (int i = 0; i < personalities.Count; i++)
		{
			if (i > 0)
			{
				personality_text.text += ", " + personalities[i];
			}
			else
			{
				personality_text.text += personalities[i];
			}
		}
	}

	private void Show_IT_Text()
	{
		idealType_text.text = "Ideal Type: \n";
		for (int i = 0; i < idealType.Count; i++)
		{
			if (i > 0)
			{
				idealType_text.text += ", " + idealType[i];
			}
			else
			{
				idealType_text.text += idealType[i];
			}
		}
	}

	private void Show_ratio_text()
	{
		ratio_text.text = "intimacy : passion : commitment: \n";
		ratio_text.text += intimacy.ToString("F1") + " : " + passion.ToString("F1") + " : " + commitment.ToString("F1");
	}

	private void RollTriangleOfLove()
	{
		float i = Random.Range(0, 1f);
		float p = Random.Range(0, 1f);
		float c = Random.Range(0, 1f);
		float factor = 1 / (i + p + c);
		intimacy = i * factor;
		passion = p * factor;
		commitment = c * factor;
	}
}
