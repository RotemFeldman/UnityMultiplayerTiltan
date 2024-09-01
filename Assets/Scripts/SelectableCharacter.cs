using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCharacter : MonoBehaviour
{
    public int Id = 0;
    [SerializeField] public Material charColor;
    [SerializeField] private Image image;
    
    private bool _isTaken = false;
    public bool IsTaken => _isTaken;

    private void Start()
    {
        image.color = charColor.color;
    }

    public SelectableCharacter Take()
    {
        _isTaken = true;
        return this;
    }
}