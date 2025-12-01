using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckMover : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public AudioSource TruckAudioSource => audioSource;

    public void MoveToDestination(Vector3 truckDestPoint)
    {
        transform.DOMove(truckDestPoint, 1f);
    }
}
