using System.Numerics;

namespace Cortisol;

public static class Extension
{ 
    /// <summary>
    /// Numerics.BigInteger Square Root extension method.
    /// </summary>
    /// <param name="x">Input BigInteger</param>
    /// <returns>Square Root of x</returns>
    /// <exception cref="ArgumentException">Negative numbers are not permitted.</exception>
    public static BigInteger Sqrt (this BigInteger x) {
        if (x < 0) throw new ArgumentException("Negative argument.");
        if (x < 2) return x;
        BigInteger y = x / 2;
        while (y > x / y) {
            y = ((x / y) + y) / 2;
        }
        return y;
    }
    

}