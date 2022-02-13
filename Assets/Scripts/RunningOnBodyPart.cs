using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningOnBodyPart : MonoBehaviour
{
    private bool _filled;
    private float _filledValue;
    
    public void IncrementBodyPartSize(float value)
    {
        _filledValue += value;
        int bodyPartCount = PlayerController.Current.bodyParts.Count;

        if(_filledValue > 1)
        {
            //make the size exactly one
            //create new part as exact amount which is left from one
            float leftValue = value - 1;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.8f * (bodyPartCount -1) - 0.03f ,transform.localPosition.z);
            transform.localScale = new Vector3(0.8f, transform.localScale.y, 0.8f);
            PlayerController.Current.CreateBodyPart(leftValue);
            
        }else if(_filledValue < 0)
        {
            //tell the character to destroy the part
            PlayerController.Current.DestroyBodyPart(this);
        }else
        {
            //update body part size
            transform.localPosition = new Vector3(transform.localPosition.x, -0.8f * (bodyPartCount -1) - 0.03f * _filledValue,transform.localPosition.z);
            transform.localScale = new Vector3(0.8f * _filledValue, transform.localScale.y, 0.8f * _filledValue);
        }
    }
}
