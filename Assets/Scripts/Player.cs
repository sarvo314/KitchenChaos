using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;

    [SerializeField] private Transform raycastEnd;
    [SerializeField] private Transform playerTop;



    private bool isWalking;
    private void Awake()
    {
        PlayerInputActions playerInputActions = new PlayerInputActions();
    }
    private void Update()
    {

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRad = Vector3.Distance(raycastEnd.position, transform.position);
        float playerHeight = Vector3.Distance(transform.position, playerTop.position);
        bool canMove = !Physics.CapsuleCast(transform.position, playerTop.position, playerRad, movDir, moveDistance);
        //check for diagonal movement
        if (!canMove)
        {
            //check in x
            Vector3 movDirX = new Vector3(movDir.x, 0f, 0f);
            canMove = !Physics.CapsuleCast(transform.position, playerTop.position, playerRad, movDirX, moveDistance);
            if (canMove)
            {
                movDir = movDirX;
            }
            else
            {
                Vector3 movDirZ = new Vector3(0f, 0f, movDir.z);
                canMove = !Physics.CapsuleCast(transform.position, playerTop.position, playerRad, movDirZ, moveDistance);

                if (canMove)
                {
                    movDir = movDirZ;
                }
                else
                {
                    //cannot move in any direction
                }

            }

        }
        if (canMove)
        {
            transform.position += movDir * moveDistance;
        }


        isWalking = movDir != Vector3.zero;
        float rotateSpeed = 10f;

        transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * rotateSpeed);

        //transform.rotation = new Quaternion(a, b, c, d);
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
