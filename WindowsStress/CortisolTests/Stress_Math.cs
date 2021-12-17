using System;
using System.Numerics;
using NUnit.Framework;

namespace Cortisol.Tests;

[TestFixture]
public class Stress_Math
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    public void IsPrimeLess2(int value)
    {
        Assert.IsFalse(Math.IsPrime(value), $"{value} is not a prime number!");
    }

    [TestCase(127)]
    [TestCase(8191)]
    [TestCase(524287)]
    public void IsPrime(int value)
    {
        Assert.IsTrue(Math.IsPrime(value), $"{value} is a Mersenne prime!");
    }
    
    [TestCase(9)]
    [TestCase(4096)]
    [TestCase(131071)]
    public void BigIntSqrtPrime(int value)
    {
        BigInteger bigInteger = value;
        Assert.AreEqual((BigInteger)Convert.ToInt32(System.Math.Round(System.Math.Sqrt(value), MidpointRounding.AwayFromZero)),  bigInteger.Sqrt());
    }
    
    [TestCase(4)]
    [TestCase(126)]
    [TestCase(241128)]
    public void BigIntSqrtNotPrime(int value)
    {
        BigInteger bigInteger = value;
        Assert.AreEqual((BigInteger)Convert.ToInt32(System.Math.Round(System.Math.Sqrt(value), MidpointRounding.AwayFromZero)),  bigInteger.Sqrt());
    }
    
}