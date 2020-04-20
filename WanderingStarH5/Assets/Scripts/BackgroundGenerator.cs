using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField]
    private float paralaxCoef;

    [SerializeField]
    private List<GameObject> backgroundStars;

    public static float Width;
    public static float Height;

    private void Start()
    {
        Width = backgroundStars[7].transform.position.x;
        Height = backgroundStars[0].transform.position.y;
    }

    private void Update()
    {
        this.gameObject.transform.position = Camera.main.transform.position * paralaxCoef;
        this.gameObject.transform.localPosition += this.gameObject.transform.localPosition.z * Vector3.back;

        if (backgroundStars[7].transform.position.x - Camera.main.transform.position.x > 2* Width)
            MoveLeft();

        if (Camera.main.transform.position.x - backgroundStars[0].transform.position.x > 2 * Width)
            MoveRight();

        if (backgroundStars[3].transform.position.y - Camera.main.transform.position.y > 2 * Height)
            MoveDown();

        if (Camera.main.transform.position.y - backgroundStars[5].transform.position.y > 2 * Height)
            MoveUp();
    }

    private void MoveDown()
    {
        GameObject stars2 = backgroundStars[0];
        backgroundStars[0].transform.position = new Vector3(backgroundStars[0].transform.position.x, backgroundStars[0].transform.position.y - Height * 3, backgroundStars[0].transform.position.z);
        GameObject stars5 = backgroundStars[3];
        backgroundStars[3].transform.position = new Vector3(backgroundStars[3].transform.position.x, backgroundStars[3].transform.position.y - Height * 3, backgroundStars[3].transform.position.z);
        GameObject stars8 = backgroundStars[6];
        backgroundStars[6].transform.position = new Vector3(backgroundStars[6].transform.position.x, backgroundStars[6].transform.position.y - Height * 3, backgroundStars[6].transform.position.z);

        backgroundStars.RemoveAt(6);
        backgroundStars.RemoveAt(3);
        backgroundStars.RemoveAt(0);

        backgroundStars.Insert(2, stars2);
        backgroundStars.Insert(5, stars5);
        backgroundStars.Insert(8, stars8);

        UpdateNames();
    }

    private void MoveUp()
    {
        GameObject stars0 = backgroundStars[2];
        backgroundStars[2].transform.position = new Vector3(backgroundStars[2].transform.position.x, backgroundStars[2].transform.position.y + Height * 3, backgroundStars[2].transform.position.z);
        GameObject stars3 = backgroundStars[5];
        backgroundStars[5].transform.position = new Vector3(backgroundStars[5].transform.position.x, backgroundStars[5].transform.position.y + Height * 3, backgroundStars[5].transform.position.z);
        GameObject stars6 = backgroundStars[8];
        backgroundStars[8].transform.position = new Vector3(backgroundStars[8].transform.position.x, backgroundStars[8].transform.position.y + Height * 3, backgroundStars[8].transform.position.z);

        backgroundStars.RemoveAt(8);
        backgroundStars.RemoveAt(5);
        backgroundStars.RemoveAt(2);

        backgroundStars.Insert(0, stars0);
        backgroundStars.Insert(3, stars3);
        backgroundStars.Insert(6, stars6);

        UpdateNames();
    }

    private void MoveRight()
    {
        Debug.Log("??? right");
        GameObject stars6 = backgroundStars[0];
        backgroundStars[0].transform.position = new Vector3(backgroundStars[0].transform.position.x + Width * 3, backgroundStars[0].transform.position.y, backgroundStars[0].transform.position.z);
        GameObject stars7 = backgroundStars[1];
        backgroundStars[1].transform.position = new Vector3(backgroundStars[1].transform.position.x + Width * 3, backgroundStars[1].transform.position.y, backgroundStars[1].transform.position.z);
        GameObject stars8 = backgroundStars[2];
        backgroundStars[2].transform.position = new Vector3(backgroundStars[2].transform.position.x + Width * 3, backgroundStars[2].transform.position.y, backgroundStars[2].transform.position.z);

        backgroundStars.RemoveAt(2);
        backgroundStars.RemoveAt(1);
        backgroundStars.RemoveAt(0);

        backgroundStars.Add(stars6);
        backgroundStars.Add(stars7);
        backgroundStars.Add(stars8);

        UpdateNames();
    }

    private void MoveLeft()
    {
        GameObject stars0 = backgroundStars[6];
        backgroundStars[6].transform.position = new Vector3(backgroundStars[6].transform.position.x - Width * 3, backgroundStars[6].transform.position.y, backgroundStars[6].transform.position.z);
        GameObject stars1 = backgroundStars[7];
        backgroundStars[7].transform.position = new Vector3(backgroundStars[7].transform.position.x - Width * 3, backgroundStars[7].transform.position.y, backgroundStars[7].transform.position.z);
        GameObject stars2 = backgroundStars[8];
        backgroundStars[8].transform.position = new Vector3(backgroundStars[8].transform.position.x - Width * 3, backgroundStars[8].transform.position.y, backgroundStars[8].transform.position.z);

        backgroundStars.RemoveAt(8);
        backgroundStars.RemoveAt(7);
        backgroundStars.RemoveAt(6);

        backgroundStars.Insert(0, stars0);
        backgroundStars.Insert(1, stars1);
        backgroundStars.Insert(2, stars2);

        UpdateNames();
    }

    private void UpdateNames()
    {
        for(int i = 0; i < backgroundStars.Count; i++)
        {
            backgroundStars[i].name = "Stars" + i;
        }
    }
}
