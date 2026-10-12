using UnityEngine;
using System.Collections.Generic;

public static class ListExtensions
{
    public static T Draw<T>(this List<T> list)      //Return a card, remove it from the list
    {
        if (list.Count == 0)
        {
            return default;
        }

        int r = Random.Range(0, list.Count);
        T t = list[r];
        list.Remove(t);
        return t;
    }
}
