using UnityEngine;
using System.Collections;

public class CompleteCameraController : MonoBehaviour
{

    public GameObject player;
    private Vector3 offset;

    private void Update()
    {
        transform.LookAt(player.transform);
    }
}