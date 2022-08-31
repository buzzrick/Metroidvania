using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaView : MonoBehaviour
{
    public float roteSpeed = 0f;
    public float animationSpeed = 1.0f;
    private Animation _animation;
    private int _animationCount;
    private List<string> _animationList;

    void Awake()
    {
        _animation = GetComponent<Animation>();
        _animationCount = _animation.GetClipCount();

        print($"animationGetCount:{_animationCount}");
        _animationList = GetAnimationList();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, roteSpeed, 0f));
    }


    void OnGUI()
    {
        int margin = 10;

        //Buttons
        int buttonSpace = 25;
        int rectWidth = 100;
        int rectHeight = 40;
        int max = 10;
        List<Rect> rects = new List<Rect>();
        int i = 0;

        foreach (string name  in _animationList)
        {
            Rect rect = new Rect(15, margin + 20 * i + buttonSpace * i, rectWidth, rectHeight);
            if (GUI.Button(rect, _animationList[i].ToString()))
            {
                _animation.CrossFade(_animationList[i], 0.01f);
            }
            i++;
        }
    }

    private List<string> GetAnimationList()
    {
        List<string> tmpArray = new List<string>();
        foreach(AnimationState state in _animation)
        {
            tmpArray.Add(state.name);
        }
        return tmpArray;
    }
}
