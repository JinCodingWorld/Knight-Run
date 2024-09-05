using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab; // 발판 프리팹
    public int count = 3; // 발판의 개수

    public float timeBetSpawnMin = 1.25f; // 다음 배치까지의 시간 간격 최솟값
    public float timeBetSpawnMax = 2.25f; // 다음 배치까지의 시간 간격 최댓값
    private float timeBetSpawn; // 다음 배치까지의 시간 간격

    public float xMin = 1.5f; // 배치할 위치의 최소 y값 -> x값
    public float xMax = 8f; // 배치할 위치의 최대 y값 -> x값
    private float yPos = 4f; // 배치할 위치의 y 값

    private GameObject[] platforms; // 미리 생성한 발판들
    private int currentIndex = 0; // 사용할 현재 순번의 발판

    private Vector2 poolPosition = new Vector2(2, 8); // 초반에 생성된 발판들을 화면 밖에 숨겨둘 위치 ????
    private float lastSpawnTime; // 마지막 배치 시점
    void Start()
    {
        // 변수들을 초기화하고 사용할 발판들을 미리 생성
        platforms = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            platforms[i] = Instantiate(platformPrefab, poolPosition, Quaternion.identity);
        }
        lastSpawnTime = 0f;
        timeBetSpawn = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // 순서를 돌아가며 주기적으로 발판을 배치
        if (Time.time >= lastSpawnTime + timeBetSpawn)
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

            float xPos = Random.Range(xMin, xMax);

            platforms[currentIndex].SetActive(false);
            platforms[currentIndex].SetActive(true);

            platforms[currentIndex].transform.position = new Vector2(xPos, yPos);
            currentIndex++;

            if (currentIndex >= count)
            {
                currentIndex = 0;
            }
        }
    }
}
