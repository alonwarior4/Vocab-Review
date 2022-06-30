using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FibonacciNumbers
{
    public static int[] numbers = { 1 , 1, 2 ,3 ,5 ,8 ,13 ,21 ,34 ,55 , 89 , 144 , 233 , 377};
    public static int GetNumber(int count) => numbers[count];       
}
