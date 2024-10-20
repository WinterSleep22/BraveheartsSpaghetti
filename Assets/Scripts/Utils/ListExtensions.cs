
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public static class ListExtensions  {

    private static Random rng = new Random();

    public static void RemoveFirst(this IList list)
    {
        list.RemoveAt(0);
    }
    public static void RemoveLast(this IList list)
    {
        list.RemoveAt(list.Count-1);
    }

    public static List<T> FindRange<T>(this IList<T> list, Predicate<T> predicate)
    {
        List<T> result = new List<T>();

        foreach(var valuee in list){
            if(predicate.Invoke(valuee)){
                result.Add(valuee);
            }
        }
        return result;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    public static string ToStringValues(this IList list)
    {
        string result="";
        foreach(var valuee in list){
            result += valuee.ToString()+" ";
        }
        return result;
    }
   
    public static string ToStringValues<T>(this T[] array)
    {
        if(array ==null){
            return "";
        }
        string result = "";
        foreach (var valuee in array)
        {
            result += valuee.ToString() + " ";
        }
        return result;
    }
}
