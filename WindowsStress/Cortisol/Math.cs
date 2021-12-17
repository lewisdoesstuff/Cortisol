using System.Numerics;

namespace Cortisol;

public static class Math
{
    /// <summary>
    /// Check if a given number is a prime.
    /// https://rosettacode.org/wiki/Mersenne_primes#C.23
    /// </summary>
    /// <param name="num">Number to test.</param>
    /// <returns></returns>
    public static bool IsPrime(BigInteger num) {
        // Compare against short list.
        if (num < 2) return false;
        if (num % 2 == 0) return num == 2;
        if (num % 3 == 0) return num == 3;
        if (num % 5 == 0) return num == 5;
        if (num % 7 == 0) return num == 7;
        if (num % 11 == 0) return num == 11;
        if (num % 13 == 0) return num == 13;
        if (num % 17 == 0) return num == 17;
        if (num % 19 == 0) return num == 19;
 
        // We use BigIntegers as the numbers get very big very fast.
        BigInteger limit = num.Sqrt();
        BigInteger test = 23;
            
        while (test < limit) {
            if (num % test == 0) return false;
            test += 2;
            if (num % test == 0) return false;
            test += 4;
        }
 
        return true;
    }
}