using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorBox : MonoBehaviour
{
    public List<GameObject> walls;
    public List<GameObject> doors;

    public bool connectedToOutside = false;

    public void ClearWalls(){
        foreach(GameObject g in walls){
            g.SetActive(false);
        }
        foreach(GameObject g in doors){
            g.SetActive(false);
        }
    }

    

    public void SetWall(int i){
        walls[i].SetActive(true);
        doors[i].SetActive(false);
    }

    public void ClearWall(int i){
        walls[i].SetActive(false);
        doors[i].SetActive(false);
    }

    public void SetDoor(int i){
        for(int j = 0; j<4; j++){
            doors[j].SetActive(false); 
        }
        doors[i].SetActive(true); 
        
        walls[i].SetActive(false);
    }

}
