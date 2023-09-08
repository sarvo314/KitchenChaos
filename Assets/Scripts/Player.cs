using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    Quaternion rot;



    private bool isWalking;
    private void Awake()
    {
        PlayerInputActions playerInputActions = new PlayerInputActions();
    }
    private void Update()
    {

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        isWalking = movDir != Vector3.zero;

        transform.position += movDir * moveSpeed * Time.deltaTime;

        float rotateSpeed = 10f;

        transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * rotateSpeed);

        //transform.rotation = new Quaternion(a, b, c, d);
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
