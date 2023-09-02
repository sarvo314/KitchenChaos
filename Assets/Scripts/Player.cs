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
    Quaternion rot;

    //[SerializeField][Range(0f, 1f)] float a;
    //[SerializeField][Range(0f, 1f)] float b;
    //[SerializeField][Range(0f, 1f)] float c;
    //the last one is th a part
    //[SerializeField][Range(0f, 1f)] float d;

    private bool isWalking;
    private void Update()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1;
        }
        inputVector = inputVector.normalized;

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
