using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float xOffset = 2.0f;
    [SerializeField] private float yOffset = 2.0f;
    [SerializeField] private float followSpeed = 5.0f; 

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = CalculateTargetPosition();
    }

    private void Update()
    {
        Vector3 newTargetPosition = CalculateTargetPosition();
        targetPosition = Vector3.Lerp(targetPosition, newTargetPosition, followSpeed * Time.deltaTime);
        
        transform.position = targetPosition;
    }

    private Vector3 CalculateTargetPosition()
    {
        return new Vector3(player.position.x - xOffset, player.position.y - yOffset, transform.position.z);
    }
}
