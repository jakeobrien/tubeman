using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeManSkeleton : MonoBehaviour 
{

    public int numberTorsoNodes;
    public float torsoNodeDistance;
    public float maxTorsoNodeAngle;
    public Vector3 basePoint;

    private class node
    {
        public Vector3 pos;

        private Vector3 _force;
        private Vector3 _velocity;

        public void AddForce(Vector3 force)
        {
            _force += force;
        }

        public void Update()
        {
            // CalculateVelocity();
            // ApplyVelocity();
            // ConstrainPosition();
        }

    }

    private Vector3[] _torsoNodes;



    private void OnDrawGizmos()
    {
        foreach (var node in _torsoNodes)
        {
            Gizmos.DrawSphere(node, 0.2f);
        }
    }

}
