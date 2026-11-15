using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class FxManager : Singleton<FxManager>
{
    public GameObject FireworkLeft;
    public GameObject FireworkRight;


    private void Start()
    {
    }


    public void PlayFireworks(Transform LeftPoint, Transform RightPoint)
    {
        Instantiate(
            FireworkLeft,
            LeftPoint.position,
            Quaternion.identity,
            transform
        );



        Instantiate(
            FireworkRight,
            RightPoint.position,
            Quaternion.identity,
            transform
        );


    }












}