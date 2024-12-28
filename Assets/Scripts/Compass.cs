using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Compass : MonoBehaviour
{
    public Vector3 northDiraction;
    public Transform Player;
    public Quaternion artifactDirection;

    public RectTransform NorthLayer;
    public RectTransform ArtifactLayer;

    public Transform artifactPlace;

    void Update()
    {
        changeNorthDirection();
        changeArtifactDirection();
    }

    public void SetArtifact(Transform artifact)
    {
        artifactPlace = artifact;
    }

    public void changeNorthDirection()
    {
        northDiraction.z = Player.eulerAngles.y;
        NorthLayer.localEulerAngles = northDiraction;
    }

    public void changeArtifactDirection()
    {if(artifactPlace != null)
        {
            Vector3 direction = transform.position - artifactPlace.position;

            artifactDirection = Quaternion.LookRotation(direction);

            artifactDirection.z = -artifactDirection.y;
            artifactDirection.x = 0;
            artifactDirection.y = 0;


            ArtifactLayer.localRotation = artifactDirection * Quaternion.Euler(northDiraction);
        }
    }
}