using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    static public PlayerScript me;
    // cam
    public float camSpdX;
    public float camSpdY;
    public Transform cam;

    // drag people
    public GameObject personGrabbed;

    void Start()
    {
        me = this;
    }

    void Update()
    {
        MoveCam();
        GrabPeople();
    }

    private void MoveCam()
	{
        if (Input.GetKey(KeyCode.W))
        {
            cam.position = new Vector3(cam.position.x, cam.position.y + camSpdY * Time.deltaTime, cam.position.z);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            cam.position = new Vector3(cam.position.x, cam.position.y - camSpdY * Time.deltaTime, cam.position.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            cam.position = new Vector3(cam.position.x - camSpdX * Time.deltaTime, cam.position.y, cam.position.z);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            cam.position = new Vector3(cam.position.x + camSpdX * Time.deltaTime, cam.position.y, cam.position.z);
        }
    }

    private void GrabPeople()
	{
        if (personGrabbed != null)
		{
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            personGrabbed.transform.position = new Vector3(mousePos.x, mousePos.y, personGrabbed.transform.position.z);

            if (Input.GetMouseButtonUp(0))
			{
                personGrabbed = null;
			}
		}
	}


}
