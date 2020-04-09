using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{   
    [Header("Parameters")]
    public int minSize = 15;
    public int maxSize = 29;
    public int clusters = 6;

    public int minExtraDoors = 0;
    public int maxExtraDoors = 2;
    
    float levelScale = 7; //adjust at your own risk lmao
    public float colorMin;
    public float colorMax;
    [Header("Prefabs")]
    public GameObject interiorBox;
    public GameObject fence;

    //trackers
    Dictionary<Vector3,GameObject> mapDict;
    Dictionary<Vector3,bool> outsideDict;
    List<InteriorBox> interiorBoxes;
    int levelWidth = 0;
    int levelHeight = 0;
    

    void Start(){
        
        Generate();
    }

    void Generate(){
        Camera.main.backgroundColor = new Color(Random.Range(colorMin,colorMax),Random.Range(colorMin,colorMax),Random.Range(colorMin,colorMax));
        mapDict = new Dictionary<Vector3, GameObject>();
        outsideDict = new Dictionary<Vector3, bool>();
        interiorBoxes = new List<InteriorBox>();
        levelWidth = Random.Range(minSize,maxSize+1);
        levelHeight = Random.Range(minSize,maxSize+1);
        if(levelWidth % 2 == 0){
            levelWidth+=1;
        }
        //place fences
        for(int x = -3; x<levelWidth+3; x++){
            for(int y = -3; y<levelHeight+3;y++){
                InteriorBox newFence;
                if(x == -3 || x == levelWidth+2 || y == -3 || y == levelHeight+2){
                    newFence = Instantiate(fence,new Vector3(x*levelScale,y*levelScale,0),Quaternion.identity).GetComponent<InteriorBox>();
                    newFence.ClearWalls();
                    if(x == -3){
                        newFence.SetWall(3);
                    }
                    if(x == levelWidth + 2){
                        newFence.SetWall(2);
                    }
                    if(y == -3){
                        newFence.SetWall(1);
                    }
                    if(y == levelHeight + 2){
                        newFence.SetWall(0);
                    }

                    //place an enterance at the bottom
                    if(y == -3 && x == (levelWidth) / 2 ){
                        newFence.SetDoor(1);
                    }
                }
            }
        }
        //place interiors
        //choose 3 positions
        for(int i = 0; i<clusters; i++){
            int xPos = Random.Range(0,levelWidth);
            int yPos = Random.Range(0,levelHeight);

            //draw a 3x3, add blocks around it
             for(int _x = xPos - 1; _x < xPos + 2; _x++){
                 for(int _y = yPos - 1; _y < yPos + 2; _y++){
                    if((_x == 0 && _y == 0) || Random.Range(0f,1f) < 0.75f){
                        PlaceInteriorBox(new Vector3(_x,_y));
                    }
                 }
             }
        }

        PropagateOutsideDict(new Vector2(-2,-2));
        
        int freeDoors = Random.Range(minExtraDoors,maxExtraDoors);
        List<InteriorBox> boxes = interiorBoxes;
        while(boxes.Count > 0){
            InteriorBox selectedBox = boxes[Random.Range(0,boxes.Count)];
            if(!selectedBox.connectedToOutside){
                List<int> openDirections = GetOpenDirections(selectedBox.transform.position);
                if(openDirections.Count > 0){
                    selectedBox.SetDoor(openDirections[Random.Range(0,openDirections.Count)]);
                    if(freeDoors > 0){
                        freeDoors -= 1;
                    }else{
                        PropagateOutsideConnection(selectedBox.transform.position);
                    }
                    
                }
            }
            
            
            boxes.Remove(selectedBox);
            //check to see if we can place a door
        }

    }

    public GameObject PlaceInteriorBox(Vector3 position){
        if(!mapDict.ContainsKey(position)){
            InteriorBox newBox = Instantiate(interiorBox,position*levelScale,Quaternion.identity).GetComponent<InteriorBox>();
            interiorBoxes.Add(newBox);
            if(mapDict.ContainsKey(position + new Vector3(0,1))){
                if(mapDict[position + new Vector3(0,1)].GetComponent<InteriorBox>() != null){
                    newBox.ClearWall(0);
                    mapDict[position + new Vector3(0,1)].GetComponent<InteriorBox>().ClearWall(1);
                }
            }
            if(mapDict.ContainsKey(position + new Vector3(0,-1))){
                if(mapDict[position + new Vector3(0,-1)].GetComponent<InteriorBox>() != null){
                    newBox.ClearWall(1);
                    mapDict[position + new Vector3(0,-1)].GetComponent<InteriorBox>().ClearWall(0);
                }
            }
            if(mapDict.ContainsKey(position + new Vector3(1,0))){
                if(mapDict[position + new Vector3(1,0)].GetComponent<InteriorBox>() != null){
                    newBox.ClearWall(2);
                    mapDict[position + new Vector3(1,0)].GetComponent<InteriorBox>().ClearWall(3);
                }
            }
            if(mapDict.ContainsKey(position + new Vector3(-1,0))){
                if(mapDict[position + new Vector3(-1,0)].GetComponent<InteriorBox>() != null){
                    newBox.ClearWall(3);
                    mapDict[position + new Vector3(-1,0)].GetComponent<InteriorBox>().ClearWall(2);
                }
            }
            mapDict[position] = newBox.gameObject;
            return newBox.gameObject;
        }
        return null;
    }

    void PropagateOutsideConnection(Vector3 key){
        key = key / levelScale;
        if(mapDict.ContainsKey(key)){
            if(mapDict[key].GetComponent<InteriorBox>()!=null){
                if(!mapDict[key].GetComponent<InteriorBox>().connectedToOutside){
                    mapDict[key].GetComponent<InteriorBox>().connectedToOutside = true;
                    PropagateOutsideConnection((key + new Vector3(0,1,0)) * levelScale);
                    PropagateOutsideConnection((key + new Vector3(0,-1,0)) * levelScale);
                    PropagateOutsideConnection((key + new Vector3(1,0,0)) * levelScale);
                    PropagateOutsideConnection((key + new Vector3(-1,0,0)) * levelScale);
                }
            }
        }
    }

    void PropagateOutsideDict(Vector3 key){
        if(key.x > -3 && key.x < (levelWidth + 3)){
            if(key.y > -3 && key.y < (levelHeight + 3)){
                if(!mapDict.ContainsKey(key) && !outsideDict.ContainsKey(key)){
                    outsideDict[key] = true;
                    PropagateOutsideDict((key + new Vector3(0,1,0)));
                    PropagateOutsideDict((key + new Vector3(0,-1,0)));
                    PropagateOutsideDict((key + new Vector3(1,0,0)));
                    PropagateOutsideDict((key + new Vector3(-1,0,0)));
                }
            }
        }
    }

    List<int> GetOpenDirections(Vector3 position){
        position = position/levelScale;
        List<int> returnList = new List<int>();
        if(outsideDict.ContainsKey(position + new Vector3(0,1,0))){
            returnList.Add(0);
        }
        if(outsideDict.ContainsKey(position + new Vector3(0,-1,0))){
            returnList.Add(1);
        }
        if(outsideDict.ContainsKey(position + new Vector3(1,0,0))){
            returnList.Add(2);
        }
        if(outsideDict.ContainsKey(position + new Vector3(-1,0,0))){
            returnList.Add(3);
        }
        return returnList;
    }

}
