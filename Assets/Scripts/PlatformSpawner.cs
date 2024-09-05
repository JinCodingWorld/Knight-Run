using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab; // ���� ������
    public int count = 3; // ������ ����

    public float timeBetSpawnMin = 1.25f; // ���� ��ġ������ �ð� ���� �ּڰ�
    public float timeBetSpawnMax = 2.25f; // ���� ��ġ������ �ð� ���� �ִ�
    private float timeBetSpawn; // ���� ��ġ������ �ð� ����

    public float xMin = 1.5f; // ��ġ�� ��ġ�� �ּ� y�� -> x��
    public float xMax = 8f; // ��ġ�� ��ġ�� �ִ� y�� -> x��
    private float yPos = 4f; // ��ġ�� ��ġ�� y ��

    private GameObject[] platforms; // �̸� ������ ���ǵ�
    private int currentIndex = 0; // ����� ���� ������ ����

    private Vector2 poolPosition = new Vector2(2, 8); // �ʹݿ� ������ ���ǵ��� ȭ�� �ۿ� ���ܵ� ��ġ ????
    private float lastSpawnTime; // ������ ��ġ ����
    void Start()
    {
        // �������� �ʱ�ȭ�ϰ� ����� ���ǵ��� �̸� ����
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
        // ������ ���ư��� �ֱ������� ������ ��ġ
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
