using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesController : MonoBehaviour
{
    private float speed = 5;

    private void Update()
    {
        this.transform.position += Vector3.down * Time.deltaTime * speed;
    }

}
