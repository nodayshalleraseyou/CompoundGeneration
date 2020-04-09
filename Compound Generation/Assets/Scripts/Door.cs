using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;

    public GameObject leftDoorClosedMarker;
    public GameObject rightDoorClosedMarker;

    public GameObject leftDoorOpenMarker;
    public GameObject rightDoorOpenMarker;

    IEnumerator currentRoutine;

    float doorSpeed = 0.075f;

    int peopleWaiting = 0;
    public void Start(){
        
    }
    void Open(){
        if(currentRoutine != null){
            StopCoroutine(currentRoutine);
        }
        currentRoutine = OpenRoutine();
        StartCoroutine(currentRoutine);
    }

    void Close(){
        if(currentRoutine != null){
            StopCoroutine(currentRoutine);
        }
        currentRoutine = CloseRoutine();
        StartCoroutine(currentRoutine);
    }
    IEnumerator OpenRoutine(){
        
        while(Vector3.Distance(rightDoor.transform.position,rightDoorOpenMarker.transform.position) > .1f){
           rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition,rightDoorOpenMarker.transform.localPosition,doorSpeed);
           leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition,leftDoorOpenMarker.transform.localPosition,doorSpeed);

           yield return null;
        }
        
        rightDoor.transform.localPosition = rightDoorOpenMarker.transform.localPosition;
        leftDoor.transform.localPosition = leftDoorOpenMarker.transform.localPosition;
        yield return null;
    }

    IEnumerator CloseRoutine(){
        
        while(Vector3.Distance(rightDoor.transform.position,rightDoorClosedMarker.transform.position) > .1f){
           rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition,rightDoorClosedMarker.transform.localPosition,doorSpeed);
           leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition,leftDoorClosedMarker.transform.localPosition,doorSpeed);

           yield return null;
        }
        rightDoor.transform.localPosition = rightDoorClosedMarker.transform.localPosition;
        leftDoor.transform.localPosition = leftDoorClosedMarker.transform.localPosition;
        yield return null;
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Character"){
            if(peopleWaiting == 0){
                Open();
            }
            peopleWaiting += 1;
        }
    }

    public void OnTriggerExit2D(Collider2D other){
        if(other.tag == "Character"){
            if(peopleWaiting == 1){
                Close();
            }
            peopleWaiting -= 1;
        }
    }


}
