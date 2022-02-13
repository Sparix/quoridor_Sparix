using System;
using Project.Classes;
using UnityEngine;

namespace Project.Scripts {
    public class PlaceForPawnController : MonoBehaviour {
        public event Action OnClick;
        
        private void OnMouseDown() {
            // todo may not working on mobile 
            OnClick?.Invoke();
        }
    }
}