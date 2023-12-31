using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Windows;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;

    [SerializeField] private Transform raycastEnd;
    [SerializeField] private Transform playerTop;

    [SerializeField] private LayerMask counterLayerMask;

    public static Player Instance { get; private set; }

    BaseCounter selectedCounter;

    private Vector3 lastInteractDir;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    private KitchenObject kitchenObject;



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
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
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
            if (hit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                Debug.Log("We select a counter");
                SetSelectedCounter(baseCounter);
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
            canMove = movDirX.x != 0 && !Physics.CapsuleCast(transform.position, playerTop.position, playerRad, movDirX, moveDistance);
            if (canMove)
            {
                movDir = movDirX;
            }
            else
            {
                Vector3 movDirZ = new Vector3(0f, 0f, movDir.z);
                canMove = movDirX.x != 0 && !Physics.CapsuleCast(transform.position, playerTop.position, playerRad, movDirZ, moveDistance);

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
        Debug.Log(movDir);

        isWalking = movDir != Vector3.zero;
        float rotateSpeed = 10f;

        transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking()
    {
        return isWalking;
    }
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
