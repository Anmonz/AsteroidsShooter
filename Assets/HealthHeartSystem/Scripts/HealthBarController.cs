/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private GameObject[] heartContainers;
    private Image[] heartFills;

    public Transform heartsParent;
    public GameObject heartContainerPrefab;

    public void InitBar(int maxTotalHealth, int currentHealth)
    {
        if (heartContainers != null) 
        {
            for (int i = 0; i < heartContainers.Length; i++)
            {
                Destroy(heartContainers[i]);
            }
        }

        heartContainers = new GameObject[maxTotalHealth];
        heartFills = new Image[maxTotalHealth];

        InstantiateHeartContainers(maxTotalHealth);
        UpdateHeartsHUD(currentHealth,currentHealth);
    }

    public void UpdateHeartsHUD(int currentHealth, int maxHealth)
    {
        SetHeartContainers(maxHealth);
        SetFilledHearts(currentHealth);
    }

    void SetHeartContainers(int maxHealth)
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts(int currentHealth)
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < currentHealth)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }

    void InstantiateHeartContainers(int maxTotalHealth)
    {
        for (int i = 0; i < maxTotalHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }
}
