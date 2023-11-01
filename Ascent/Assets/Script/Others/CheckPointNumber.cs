using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckPointNumber : MonoBehaviour
{
    public static CheckPointNumber Instance { get; set; }

    [Space]
    [Header("Components")]
    public LayerMask checkPointLayer;
    public LayerMask spawnPointLayer;

    [Space]
    [Header("Variables")]
    public int checkPointNumber;
    public float size;
    private bool m_collided, m_spawnCollided;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        m_collided = Physics.CheckSphere(transform.position, size, checkPointLayer);
        m_spawnCollided = Physics.CheckSphere(transform.position, size, spawnPointLayer);

        if (m_collided)
                ThirdPersonController.Instance.checkNumber = checkPointNumber;

        if (m_spawnCollided)
                ThirdPersonController.Instance.checkNumber = checkPointNumber;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, size);
    }
}