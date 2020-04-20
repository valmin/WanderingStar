using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private CharacterStarController player;

    public void Init(CharacterStarController player)
    {
        this.transform.position = player.transform.position + Vector3.back * 10;
        this.player = player;
    }

    private void Update()
    {
        if (player == null)
            return;

        this.transform.position = Vector3.Lerp(this.transform.position, (player.transform.position + Vector3.back * 10), 0.5f);
    }
}
