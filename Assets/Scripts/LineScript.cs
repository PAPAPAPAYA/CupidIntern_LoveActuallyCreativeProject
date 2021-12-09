using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour
{
    public GameObject a;
    public GameObject b;
    private LineRenderer lr;
    public int relpBetweenThem;
    public int newHobbyCost;
    private PersonScript psA;
    private PersonScript psB;

    public int state;
    public int friend = 0;
    public int closeFriend = 1;
    public int dating = 2;

    public Color close;
    public Color dating_color;

    private void Start()
	{
        lr = GetComponent<LineRenderer>();
        psA = a.GetComponent<PersonScript>();
        psB = b.GetComponent<PersonScript>();
        state = friend;
    }

	void Update()
    {
        lr.SetPosition(0, a.transform.position);
        lr.SetPosition(1, b.transform.position);

        if (!lr.enabled)
		{
            GameManager.me.lines.Remove(gameObject);
            Destroy(gameObject);
		}

        if (relpBetweenThem >= newHobbyCost)
		{
            if (state == friend)
			{
                List<HobbyStruct> overlappedHobbies = new List<HobbyStruct>();
                List<HobbyStruct> aHasBDoesnt = new List<HobbyStruct>();
                List<HobbyStruct> bHasADoesnt = new List<HobbyStruct>();
                // get overlapped hobbies
                for (int i = 0; i < psA.hobbies.Count; i++)
                {
                    for (int j = 0; j < psB.hobbies.Count; j++)
                    {
                        if (psA.hobbies[i].name == psB.hobbies[j].name)
                        {
                            overlappedHobbies.Add(psA.hobbies[i]);
                        }
                    }
                }
                if (overlappedHobbies.Count < psA.hobbies.Count)
                {
                    // get what a has but b doesn't
                    foreach (HobbyStruct hobby in overlappedHobbies)
                    {
                        for (int i = 0; i < psA.hobbies.Count; i++)
                        {
                            if (psA.hobbies[i].name != hobby.name)
                            {
                                aHasBDoesnt.Add(psA.hobbies[i]);
                            }
                        }
                    }
                }
                if (overlappedHobbies.Count < psB.hobbies.Count)
                {
                    // get what b has but a doesn't
                    foreach (HobbyStruct hobby in overlappedHobbies)
                    {
                        for (int i = 0; i < psB.hobbies.Count; i++)
                        {
                            if (psB.hobbies[i].name != hobby.name)
                            {
                                bHasADoesnt.Add(psB.hobbies[i]);
                            }
                        }
                    }
                }
                // roll one from aHasBDoesnt and give it to b
                if (aHasBDoesnt.Count > 0)
                {
                    psB.hobbies.Add(aHasBDoesnt[Random.Range(0, aHasBDoesnt.Count)]);
                }
                // roll one from bHasADoesnt and give it to a
                if (bHasADoesnt.Count > 0)
                {
                    psA.hobbies.Add(bHasADoesnt[Random.Range(0, bHasADoesnt.Count)]);
                }
                // check if close friends
                if (overlappedHobbies.Count == psA.hobbies.Count &&
                    overlappedHobbies.Count == psB.hobbies.Count)
                {
                    psA.closeFriends.Add(b);
                    psB.closeFriends.Add(a);
                    state = closeFriend;
                    lr.startColor = close;
                    lr.endColor = close;
                }
            }
			else if (state == closeFriend)
			{
                state = dating;
                lr.startColor = dating_color;
                lr.endColor = dating_color;
                psA.offTheMarket = true;
                psB.offTheMarket = true;
			}
            // finish this cycle and spend the relp
            relpBetweenThem = 0;
            newHobbyCost *= 2;
        }

        if (psA.offTheMarket && psB.offTheMarket)
		{
            dating_color.a -= 0.3f * Time.deltaTime;
            lr.startColor = dating_color;
            lr.endColor = dating_color;
            if (dating_color.a <= 0)
			{
                lr.enabled = false;
			}
		}
    }
}
