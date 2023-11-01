using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Campfire : MonoBehaviour
{
    public static Campfire Instance { get; set; }

    [Space]
    [Header("Components")]
    public GameObject[] campFire;
    public Vector3 lastCheckPointPos;

    private int m_orginalCheckpoint;

    void Awake()
    {
        Instance = this;
    }

    public void CheckPoint()
    {
        if (ThirdPersonController.Instance.lastCheckNumber >= ThirdPersonController.Instance.checkNumber)
        {
            lastCheckPointPos = campFire[ThirdPersonController.Instance.checkNumber].transform.position;
        }
        else
        {
            lastCheckPointPos = campFire[ThirdPersonController.Instance.spawnCheckNumber].transform.position;
        }
    }
}
