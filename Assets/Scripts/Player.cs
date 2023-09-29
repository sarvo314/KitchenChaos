using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;

    [SerializeField] private Transform raycastEnd;
    [SerializeField] private Transform playerTop;

    [SerializeField] private LayerMask counterLayerMask;

    public static Player Instance { get; private set; }

    ClearCounter selectedCounter;

    private Vector3 lastInteractDir;




    private bool isWalking;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one player instance");
        }

        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float interactDistance = 2f;

        if (movDir != Vector3.zero)
        {
            lastInteractDir = movDir;
        }

        Debug.DrawRay(transform.position, movDir * 10);
        //RayCast takes global direction
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit hit, interactDistance, counterLayerMask))
        {
            if (hit.transform.TryGetComponent<ClearCounter>(out ClearCounter clearCounter))
            {
                Debug.Log("We select a counter");
                SetSelectedCounter(clearCounter);
            }
            else
            {
                SetSelectedCounter(null);
            }

        }
        else
        {
            SetSelectedCounter(null);
        }

    }



    private void HandleMovement()
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
    }

    public bool IsWalking()
    {
        return isWalking;
    }
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
}
