using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCallback : MonoBehaviour
{
    [SerializeField] private Node node;
    [SerializeField] private ParticleSystem particleSystem;

    void OnParticleSystemStopped()
    {
        node.SetGameOverEmoji(false);
    }
}
