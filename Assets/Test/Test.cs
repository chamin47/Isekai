using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyCompare : IComparer<int>
{
    public int Compare(int a, int b)
    {
        return a - b;
    }
}

public class Compa : IComparable<Compa>
{
    public int CompareTo(Compa other)
    {
        return other.CompareTo(this);
    }
}


public class Te
{
    SortedSet<int> a = new SortedSet<int>(new MyCompare());
    string s = "hello";
    Queue<int> q = new Queue<int>();
    public void Test()
    {
        c[1] = 1;
        foreach(int a in c.Keys)
        {

        }
    }

    List<int> b;
    Dictionary<int, int> c;

}


public class Test : MonoBehaviour
{
    public AudioSource audioSource;
    [ContextMenu("Test")]
    public void Play()
    {
        audioSource.Play();
    }

}
