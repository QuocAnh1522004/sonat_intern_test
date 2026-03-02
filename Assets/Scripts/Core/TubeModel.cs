using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

//this class contains the tube raw structure/data of the tube
public class TubeModel
{
    private Stack<ColorType> _layers;
    public int Capacity { get; }
    public bool IsEmpty => _layers.Count == 0;
    public bool IsFull => _layers.Count >= Capacity;
    public int Count => _layers.Count;
    public TubeModel(int capacity)
    {
        Capacity = capacity;
        _layers = new Stack<ColorType>(capacity);
    }
    public ColorType? PeekTop()
    {
        if(IsEmpty) return null;
        return _layers.Peek();
    }
    public bool CanPourTo(TubeModel target)
    {
        if(IsEmpty || target.IsFull) return false;  
        if(target.IsEmpty) return true;
        return target.PeekTop() == PeekTop();
    }

    public void PourTo(TubeModel target) 
    {
        if(!CanPourTo(target)) return;
        ColorType colorToMove = _layers.Peek();

        while (!IsEmpty 
               &&_layers.Peek() == colorToMove   
               && !target.IsFull)
        {
            target._layers.Push(_layers.Pop()); //move the top color to the target tube
        }
    }

    public bool IsComplete()
    {
        if (_layers.Count == 0) return true;
        if(_layers.Count != Capacity) return false;
        return _layers.All(c => c == _layers.Peek());
    }

    public bool IsFullAndFilledWithOneColor()
    {
        return (_layers.All(c => c == _layers.Peek()) && IsFull);
    }

    public void AddLayer(ColorType color)
    {
        if (!IsFull)
            _layers.Push(color);
    }
    public List<ColorType> GetLayers()
    {
        return _layers.Reverse().ToList();
    }

    public int GetTopSameColorCount()
    {
        if (_layers.Count == 0)
            return 0;

        ColorType topColor = _layers.Peek();
        int count = 0;

        foreach (var layer in _layers)
        {
            if (layer.Equals(topColor))
                count++;
            else
                break;
        }

        return count;
    }

    public void RemoveTopLayers(int count)
    {
        for (int i = 0; i < count; i++)
            _layers.Pop();
    }

    public void AddManySameColorLayers(ColorType color, int count)
    {
        for (int i = 0; i < count; i++)
            _layers.Push(color);
    }
}


public enum ColorType
{
    Red,
    Blue,
    Yellow,
    Green,
    Purple
}

