using System;
using UnityEngine;

namespace Project.Scripts {
    public class PlaceForWall : MonoBehaviour {
        [Range(0, 1f)][SerializeField] private float radius = 0.01f;
        [SerializeField] private int y;
        [SerializeField] private int x;
        // [SerializeField] private GameObject horizontal;
        // [SerializeField] private GameObject vertical;

        // public GameObject GetHorizontal => horizontal;
        // public GameObject GetVertical => vertical;

        public int GetY => y;
        public int GetX => x;

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}