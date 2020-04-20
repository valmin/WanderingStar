using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public const float MASS_LOSE = 0.1f;
    public const float GRAVITY = 0.0005f;
    public const float DELTA_Z = 0.01f;

    [SerializeField]
    private Transform starsParent;
    [SerializeField]
    private CameraFollower cameraFollower;
    [SerializeField]
    private MusicController musicController;
    [SerializeField]
    private SoundsController soundsController;

    [Header("Game Settings")]
    [SerializeField]
    private float starsAliveDistance = 100;
    [SerializeField]
    private float starsMaxCount = 30;
    [SerializeField]
    private float starsMaxRadius = 5;
    [SerializeField]
    private float starsMinRadius = 1;
    [SerializeField]
    private float sectorCount = 10;
    [SerializeField]
    private float minOffset = 1;
    [SerializeField]
    private float startMass = 2;
    [SerializeField]
    private float forcesDistance = 10;

    [Header("Prefabs")]
    [SerializeField]
    private CharacterStarController playerPrefab;
    [SerializeField]
    private StarController[] starPrefabs;

    [Header("UI")]
    [SerializeField]
    private GameObject initUI;
    [SerializeField]
    private Text timerMin;
    [SerializeField]
    private Text timerSec;
    [SerializeField]
    private GameObject timer;

    [Header("Logs")]
    [SerializeField]
    private Text starsText;
    [SerializeField]
    private Text lastTimeText;
    [SerializeField]
    private Text bestTimeText;

    private List<StarController> stars = new List<StarController>();
    private CharacterStarController player;
    private float sumChance;
    private float lifeTime;
    private float bestTime;
    private int starsCount;

    private void Start()
    {
        sumChance = 0;
        foreach (StarController starPrefab in starPrefabs)
        {
            sumChance += starPrefab.chance;
        }

        timer.SetActive(false);
    }

    public void InitGame()
    {
        soundsController.PlaySound(SoundsController.Type.Click);

        initUI.SetActive(false);
        musicController.StartGame();

        lifeTime = 0;
        starsCount = 0;
        timer.SetActive(true);

        Destroy(starsParent.gameObject);

        starsParent = (new GameObject("StarsParent")).transform;
        starsParent.parent = this.transform;

        stars.Clear();

        player = Instantiate(playerPrefab, starsParent) as CharacterStarController;
        player.Init(Vector3.zero, startMass);
        player.onDead += PlayerDead;
        player.onEat += () =>
        {
            soundsController.PlaySound(SoundsController.Type.Eat);
            starsCount++;
        };

        cameraFollower.Init(player);
    }

    private void PlayerDead(StarController star)
    {
        soundsController.PlaySound(SoundsController.Type.Death);
        initUI.SetActive(true);
        musicController.EndGame();
        timer.SetActive(false);

        bestTime = Mathf.Max(bestTime, lifeTime);
        bestTimeText.text = string.Format("{0:00}:{1:00}", (int)(bestTime / 60), (bestTime % 60));
        lastTimeText.text = string.Format("{0:00}:{1:00}", (int)(lifeTime / 60), (lifeTime % 60));
        starsText.text = starsCount.ToString();
    }

    private void Update()
    {
        if (player == null)
            return;

        lifeTime += Time.deltaTime;

        timerMin.text = string.Format("{0:00}", (int)(lifeTime / 60));
        timerSec.text = string.Format("{0:00}", (lifeTime % 60));

        for (int i = stars.Count - 1; i >= 0; i--)
        {
            if ((stars[i].transform.position - player.transform.position).magnitude > starsAliveDistance)
            {
                stars[i].Death();
            }
            else
            {
                stars[i].transform.position = new Vector3(stars[i].transform.position.x, stars[i].transform.position.y, 0);
            }
        }

        if (stars.Count < starsMaxCount)
        {
            SpawnStar();
        }

        ApplyForces();

        CleanUpStars();
    }

    private void ApplyForces()
    {
        for (int i = 0; i < stars.Count-1; i++)
        {
            for (int j = i+1; j < stars.Count; j++)
            {
                
                Vector3 vector = stars[i].transform.position - stars[j].transform.position;
                Vector3 direction = vector.normalized;
                float distance = vector.magnitude;

                if (distance < forcesDistance)
                {
                    stars[i].ApplyForce(stars[j], distance, -direction);
                    stars[j].ApplyForce(stars[i], distance, direction);

                    if(stars[i].radius < stars[j].radius)
                        stars[i].transform.position = new Vector3(stars[i].transform.position.x, stars[i].transform.position.y, stars[j].transform.position.z - DELTA_Z);
                    else
                        stars[j].transform.position = new Vector3(stars[j].transform.position.x, stars[j].transform.position.y, stars[i].transform.position.z - DELTA_Z);
                }
            }
        }

        if(!player.IsDying)
            for (int i = 0; i < stars.Count; i++)
            {
                Vector3 vector = (stars[i].transform.position - player.transform.position);
                Vector3 direction = vector.normalized;
                float distance = vector.magnitude;
                if (distance < forcesDistance)
                {
                    stars[i].ApplyForce(player, distance, -direction);
                    player.ApplyForce(stars[i], distance, direction);

                    if (stars[i].radius < player.radius)
                        stars[i].transform.position = new Vector3(stars[i].transform.position.x, stars[i].transform.position.y, player.transform.position.z - DELTA_Z);
                    else
                        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, stars[i].transform.position.z - DELTA_Z);
                }
            }
    }

    private void SpawnStar()
    {
        List<Vector2> freeSectors = new List<Vector2>();

        for(int i = 0; i < sectorCount; i++)
        {
            for (int j = 0; j < sectorCount; j++)
            {
                if (SectorFree(i, j))
                    freeSectors.Add(new Vector2(i, j));
            }
        }

        if (freeSectors.Count == 0)
            return;

        int sectorInd = UnityEngine.Random.Range(0, freeSectors.Count);
        Vector3 starPosition = GetRandomPositionInSector(freeSectors[sectorInd]);

        float maxRadius = starsMaxRadius;
        for (int i = stars.Count - 1; i >= 0; i--)
        {
            float distance = (stars[i].transform.position - starPosition).magnitude - minOffset - stars[i].radius;
            maxRadius = Mathf.Min(maxRadius, distance);
        }

        float distanceToPlayer = (player.transform.position - starPosition).magnitude - minOffset - player.radius;
        maxRadius = Mathf.Min(maxRadius, distanceToPlayer);

        if (maxRadius < starsMinRadius)
            return;

        int starInd = GetRandomStarIndex();
        StarController newStar = Instantiate(starPrefabs[starInd], starsParent) as StarController;
        newStar.Init(starPosition, starsMinRadius, maxRadius);
        stars.Add(newStar);
    }

    private int GetRandomStarIndex()
    {
        float chance = UnityEngine.Random.Range(0, sumChance);
        float sum = 0;
        for(int i = 0; i < starPrefabs.Length; i++)
        {
            sum += starPrefabs[i].chance;
            if (chance < sum)
                return i;
        }

        return starPrefabs.Length - 1;
    }

    private Vector3 GetRandomPositionInSector(Vector2 sector)
    {
        float sectorSize = (starsAliveDistance / sectorCount);
        float minX = player.transform.position.x - starsAliveDistance / 2 + sector.x * sectorSize;
        float maxX = minX + sectorSize;

        float minY = player.transform.position.y - starsAliveDistance / 2 + sector.y * sectorSize;
        float maxY = minY + sectorSize;

        return new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), 0);
    }

    private bool SectorFree(int i, int j)
    {
        float sectorSize = (starsAliveDistance / sectorCount);
        float minX = player.transform.position.x - starsAliveDistance / 2 + i * sectorSize;
        float maxX = minX + sectorSize;

        float minY = player.transform.position.y - starsAliveDistance / 2 + j * sectorSize;
        float maxY = minY + sectorSize;

        for (int k = stars.Count - 1; k >= 0; k--)
        {
            if (stars[k].transform.position.x > minX && stars[k].transform.position.x < maxX
                 && stars[k].transform.position.y > minY && stars[k].transform.position.y < maxY)
                return false;
        }

        if (player.transform.position.x > minX && player.transform.position.x < maxX
                && player.transform.position.y > minY && player.transform.position.y < maxY)
            return false;

        return true;
    }

    private void CleanUpStars()
    {
        for (int k = stars.Count - 1; k >= 0; k--)
        {
            if (stars[k].IsDying)
                stars.RemoveAt(k);
        }
    }
}
