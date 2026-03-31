using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.AI.GOAP
{
    public class Locations : MonoBehaviour
    {
        public List<GameObject> items = new();

        public void AddItem(GameObject i) {

            items.Add(i);
        }
    
        public GameObject FindItemWithTag(string tag) {
        
            foreach (GameObject i in items) {
                if (i.CompareTag(tag)) {

                    return i;
                }
            }
            return null;
        }
    
        public void RemoveItem(GameObject i) {

            int indexToRemove = -1;
        
            foreach (GameObject g in items) {
            
                indexToRemove++;
                if (g == i) {

                    break;
                }
            }
            if (indexToRemove >= 1) {
            
                items.RemoveAt(indexToRemove);
            }
        }
    }
}
