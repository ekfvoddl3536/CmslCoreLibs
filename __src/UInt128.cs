using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public sealed class UInt128 : IComparable, IComparable<UInt128>, IEquatable<UInt128>, IEnumerable<uint>
{
    public static UInt128 MaxValue => new UInt128(ulong.MaxValue, ulong.MaxValue);
    public static UInt128 MinValue => 0;
    public static UInt128 One => 1;
    public static UInt128 HighOne => new UInt128(0, 1);
    public static UInt128 LowMax => new UInt128(ulong.MaxValue, 0);
    public static UInt128 HighMax => new UInt128(0, ulong.MaxValue);
    public static UInt128 LowInt32Max => new UInt128(uint.MaxValue, 0);
    public static UInt128 HighInt32max => new UInt128(0, uint.MaxValue);

    #region 필드
    [FieldOffset(0)]
    public ulong Low;
    [FieldOffset(8)]
    public ulong High;

    [FieldOffset(0)]
    public long SignedLow;
    [FieldOffset(8)]
    public long SignedHigh;

    [FieldOffset(0)]
    public uint UInt_Lowest;
    [FieldOffset(4)]
    public uint UInt_Low;
    [FieldOffset(8)]
    public uint UInt_High;
    [FieldOffset(12)]
    public uint UInt_Highest;

    [FieldOffset(0)]
    public int Int_Lowest;
    [FieldOffset(4)]
    public int Int_Low;
    [FieldOffset(8)]
    public int Int_High;
    [FieldOffset(12)]
    public int Int_Highest;
    #endregion

    public UInt128(ulong _l, ulong _h)
    {
        Low = _l;
        High = _h;
    }
    public UInt128(long _sl, long _sh)
    {
        SignedLow = _sl;
        SignedHigh = _sh;
    }
    public UInt128(byte[] _arr)
    {
        int x, a = 0;
        for (x = 0; x < 8; x++, a++)
            Low |= (ulong)_arr[a] << (8 * x);
        for (x = 0; x < 8; x++, a++)
            High |= (ulong)_arr[a] << (8 * x);
    }
    public UInt128(BigInteger biginte) : this(biginte.ToByteArray()) { }
    public UInt128(BigInteger biginte, out uint count) : this(biginte) => count = BitCount();

    #region 메서드
    public int BitScanForward()
    {
        int[] bits = GetBits32();
        int[] tp = { 65535, 255, 15, 3, 1, 1 };
        int[] vs = { 16, 4, 2, 2, 1, 0 };
        for (int count = 0, x = 0, a; x < 4; x++)
            if (bits[x] == 0) count += 32;
            else
                for (a = 0; a < 5; a++)
                    if ((bits[x] & tp[a]) != 0)
                        if ((bits[x] & 1) != 1)
                            return count;
                        else
                            continue;
                    else
                    {
                        bits[x] >>= vs[a];
                        count += vs[a];
                    }

        return -1;
    }

    public int BitScanReverse()
    {
        int[] bits = GetBits32();
        int[] tp = { -65536, -16777216, -268435456, -1073741824, int.MinValue, int.MinValue };
        int[] vs = { 16, 4, 2, 2, 1, 0 };
        for (int count = 0, x = 3, a; x >= 0; x--)
            if (bits[x] == 0) count += 32;
            else
                for (a = 0; a < 5; a++)
                    if ((bits[x] & tp[a]) != 0)
                        if ((bits[x] & int.MinValue) != int.MinValue)
                            return count;
                        else
                            continue;
                    else
                    {
                        bits[x] <<= vs[a];
                        count += vs[a];
                    }

        return -1;
    }

    public bool IsBitSet(int bitpos) =>
        bitpos < 64 ?
        (Low & 1UL << bitpos) != 0 :
        bitpos < 128 ?
        (High & 1UL << bitpos % 64) != 0 : false;

    public uint BitCount()
    {
        uint count = 0, tmp;
        foreach (uint val in this)
        {
            if (val == 0) continue;
            tmp = (val & 0x55555555) + ((val >> 1) & 0x55555555);
            tmp = (tmp & 0x33333333) + ((tmp >> 2) & 0x33333333);
            tmp = (tmp & 0x0F0F0F0F) + ((tmp >> 4) & 0x0F0F0F0F);
            tmp = (tmp & 0x00FF00FF) + ((tmp >> 8) & 0x00FF00FF);
            tmp = (tmp & 0x0000FFFF) + ((tmp >> 16) & 0x0000FFFF);
            count += tmp;
        }
        return count;
    }

    public byte[] ToByteArray()
    {
        byte[] ac = new byte[16];
        int x = 0, a;
        foreach (uint val in this)
            for (a = 0; a < 4; a++, x++)
                ac[x] = Convert.ToByte(val >> (8 * a));

        return ac;
    }

    // _BitRightShift 에서 사용되는 함수입니다.
    // BigInteger 는 Signed 형식이라서, UInt128에서는 양수로 표현되는 것이 음수로 표현됩니다.
    // 음수를 >> 연산하게 되면 밀어낸 만큼 비트가 채워지기 때문에, 그것을 방지하기 위함입니다.
    public byte[] ToExByteArray()
    {
        List<byte> vs = new List<byte>(ToByteArray()) { 0 };
        return vs.ToArray();
    }

    public int[] GetBits32() => new[] { Int_Lowest, Int_Low, Int_High, Int_Highest };

    public uint[] GetBitsU32() => new[] { UInt_Lowest, UInt_Low, UInt_High, UInt_Highest };

    public long[] GetBits64() => new[] { SignedLow, SignedHigh };

    public ulong[] GetBitsU64() => new[] { Low, High };

    public BigInteger ToBigInteger() => new BigInteger(ToByteArray());

    public BitArray ToBitArray() => new BitArray(GetBits32());

    private UInt128 _BitLeftShift(int shift)
    {
        if (shift < 0 || shift > 127) return 0;
        BigInteger big = this;
        big <<= shift;
        return new UInt128((ulong)(big & 0xFFFFFFFFFFFFFFFF), (ulong)(big >> 64 & 0xFFFFFFFFFFFFFFFF));
    }

    private UInt128 _BitRightShift(int shift)
    {
        if (shift < 0 || shift > 127) return 0;
        BigInteger big = new BigInteger(ToExByteArray());
        big >>= shift;
        return new UInt128((ulong)(big & 0xFFFFFFFFFFFFFFFF), (ulong)(big >> 64 & 0xFFFFFFFFFFFFFFFF));
    }

    private UInt128 Addition(UInt128 x)
    {
        uint[] others = x.GetBitsU32();
        uint[] thisbit = GetBitsU32();
        ulong[] tmp = new ulong[4];
        for (int c = 0; c < 4; c++)
            tmp[c] = thisbit[c] + (ulong)others[c];

        return new UInt128(tmp[0] + (tmp[1] << 32), tmp[2] + (tmp[3] << 32));
    }

    private UInt128 Subtraction(UInt128 x)
    {
        uint[] others = x.GetBitsU32();
        uint[] thisbit = GetBitsU32();
        ulong[] tmp = new ulong[4];
        for (int c = 3; c >= 0; c--)
            tmp[c] = thisbit[c] - (ulong)others[c];

        return new UInt128(tmp[0] + (tmp[1] << 32), tmp[2] + (tmp[3] << 32));
    }

    private UInt128 Multiplication(UInt128 x)
    {
        uint[] thisbit = GetBitsU32();
        ulong[] tmp = new ulong[4];
        for (int c = 0; c < 4; c++)
            foreach (uint val in x)
                tmp[c] += thisbit[c] * (ulong)val;

        return new UInt128(tmp[0] + (tmp[1] << 32), tmp[2] + (tmp[3] << 32));
    }

    private UInt128 Division(UInt128 x)
    {
        ulong[] thisbit = GetBitsU64();
        ulong[] others = x.GetBitsU64();
        ulong[] tmp = new ulong[2];
        for (int c = 0, a; c < 2; c++)
            for (a = 0; a < 2; a++)
                if (others[a] != 0)
                    tmp[c] += thisbit[c] / others[a];

        return new UInt128(tmp[0], tmp[1]);
    }

    private UInt128 Modulo(UInt128 x) => new UInt128(Low % x.Low, High % x.High);
    #endregion

    #region 공용 인터페이스 구현
    // object 가 인수이기 때문에 약간 귀찮아집니다.
    // 먼저 인수로 전달된 obj 가(이) UInt128가 지원하는 자료형인지 알아봅니다.
    //
    // * CompareTo(UInt128)가 있기 때문에 이 함수를 호출하는 경우
    // * 최소한 UInt128이 아니라는것을 뜻합니다.
    public int CompareTo(object obj)
    {
        // UInt128 에서 지원하는 기본 자료형입니다.
        if (obj is char || obj is byte || obj is sbyte || obj is short ||
            obj is ushort || obj is int || obj is uint || obj is long || obj is ulong)
            return CompareTo(new UInt128((ulong)obj));
        else
            return int.MinValue;
    }

    public int CompareTo(UInt128 other) => other > this ? 1 : other < this ? -1 : 0;

    public bool Equals(UInt128 other) => other == this;

    public IEnumerable<int> Int32Enumerator()
    {
        yield return Int_Lowest;
        yield return Int_Low;
        yield return Int_High;
        yield return Int_Highest;
        yield break;
    }

    public IEnumerator<uint> GetEnumerator()
    {
        yield return UInt_Lowest;
        yield return UInt_Low;
        yield return UInt_High;
        yield return UInt_Highest;
        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion

    #region 재정의 또는 오버로딩
    // override된 ToString()은 쉽게 알아보기 힘든 16진수로 표현되기 때문에,
    // BigInteger 를 이용하는 방법도 고려해볼 만한 좋은 방법중 하나입니다.
    //
    // 필요시 다음 주석을 제거하고, 기존의 ToString() 를(을) 대체하십시오
    // public override string ToString() => ((BigInteger)this).ToString();
    public override string ToString() => $"{UInt_Highest:X8}{UInt_High:X8}{UInt_Low:X8}{UInt_Lowest:X8}";
    public override bool Equals(object obj) => obj is UInt128; // object로 검사하는 경우 target obj이 UInt128인지만 확인하는게 좋습니다.
    public override int GetHashCode() => base.GetHashCode();

    #region 암시적 변환
    public static implicit operator BigInteger(UInt128 ex) => ex.ToBigInteger();
    public static implicit operator BitArray(UInt128 ex) => ex.ToBitArray();
    public static implicit operator ulong(UInt128 ex) => ex.Low;
    public static implicit operator long(UInt128 ex) => ex.SignedLow;
    public static implicit operator uint(UInt128 ex) => ex.UInt_Lowest;
    public static implicit operator int(UInt128 ex) => ex.Int_Lowest;

    // UInt128은 128비트이며 엄청나게 큰 수를 표현할 수 있는 크기입니다.
    // 따라서 C#의 크기가 정해진 정수 계열의 기본 자료형은 모두 받아낼 수 있습니다.
    public static implicit operator UInt128(ulong p) => new UInt128(p, 0);
    public static implicit operator UInt128(long p) => new UInt128(p, 0);
    public static implicit operator UInt128(uint p) => new UInt128(p, 0);
    public static implicit operator UInt128(int p) => new UInt128(p, 0);
    public static implicit operator UInt128(short p) => new UInt128(p, 0);
    public static implicit operator UInt128(ushort p) => new UInt128(p, 0);
    public static implicit operator UInt128(byte p) => new UInt128(p, 0);
    public static implicit operator UInt128(sbyte p) => new UInt128(p, 0);
    // C# 가이드 (char 형) 참고 바랍니다.
    // 참고: https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/char
    // 인용:
    // char는 ushort, int, uint, long, ulong, float, double 또는 decimal로 암시적으로 변환될 수 있습니다. 
    public static implicit operator UInt128(char p) => new UInt128(p, 0);
    #endregion

    #region 비교 연산자
    public static bool operator ==(UInt128 a1, UInt128 a2) => a1.Low == a2.Low && a1.High == a2.High;
    public static bool operator !=(UInt128 a1, UInt128 a2) => a1.Low != a2.Low || a1.High != a2.High;
    public static bool operator <=(UInt128 a1, UInt128 a2) => a1.High < a2.High || (a1.High == a2.High && a1.Low <= a2.Low);
    public static bool operator >=(UInt128 a1, UInt128 a2) => a1.High > a2.High || (a1.High == a2.High && a1.Low >= a2.Low);
    public static bool operator >(UInt128 a1, UInt128 a2) => a1.High > a2.High || (a1.High == a2.High && a1.Low > a2.Low);
    public static bool operator <(UInt128 a1, UInt128 a2) => a1.High < a2.High || (a1.High == a2.High && a1.Low < a2.Low);

    public static bool operator ==(UInt128 a1, ulong a2) => a1.Low == a2;
    public static bool operator !=(UInt128 a1, ulong a2) => a1.Low != a2;
    public static bool operator <=(UInt128 a1, ulong a2) => a1.Low <= a2;
    public static bool operator >=(UInt128 a1, ulong a2) => a1.Low >= a2;
    public static bool operator >(UInt128 a1, ulong a2) => a1.Low > a2;
    public static bool operator <(UInt128 a1, ulong a2) => a1.Low < a2;

    public static bool operator ==(UInt128 a1, long a2) => a1.SignedLow == a2;
    public static bool operator !=(UInt128 a1, long a2) => a1.SignedLow != a2;
    public static bool operator <=(UInt128 a1, long a2) => a1.SignedLow <= a2;
    public static bool operator >=(UInt128 a1, long a2) => a1.SignedLow >= a2;
    public static bool operator >(UInt128 a1, long a2) => a1.SignedLow > a2;
    public static bool operator <(UInt128 a1, long a2) => a1.SignedLow < a2;

    public static bool operator ==(UInt128 a1, uint a2) => a1.UInt_Lowest == a2;
    public static bool operator !=(UInt128 a1, uint a2) => a1.UInt_Lowest != a2;
    public static bool operator <=(UInt128 a1, uint a2) => a1.UInt_Lowest <= a2;
    public static bool operator >=(UInt128 a1, uint a2) => a1.UInt_Lowest >= a2;
    public static bool operator >(UInt128 a1, uint a2) => a1.UInt_Lowest > a2;
    public static bool operator <(UInt128 a1, uint a2) => a1.UInt_Lowest < a2;

    // int는 C#에 기본적인 표현이기 때문에 지정해주어야 한다.
    public static bool operator ==(UInt128 a1, int a2) => a1.Int_Lowest == a2;
    public static bool operator !=(UInt128 a1, int a2) => a1.Int_Lowest != a2;
    public static bool operator <=(UInt128 a1, int a2) => a1.Int_Lowest <= a2;
    public static bool operator >=(UInt128 a1, int a2) => a1.Int_Lowest >= a2;
    public static bool operator >(UInt128 a1, int a2) => a1.Int_Lowest > a2;
    public static bool operator <(UInt128 a1, int a2) => a1.Int_Lowest < a2;
    #endregion

    #region 산술 연산자
    /*                                                                                  decimal을 이용한 연산은 예기치 못한 결과를 초래할 수 있다. */
    public static UInt128 operator +(UInt128 a1, UInt128 a2) => a1.Addition(a2);        // new UInt128((decimal)a1 + (decimal)a2);
    public static UInt128 operator -(UInt128 a1, UInt128 a2) => a1.Subtraction(a2);     // new UInt128((decimal)a1 - (decimal)a2);
    public static UInt128 operator *(UInt128 a1, UInt128 a2) => a1.Multiplication(a2);  // new UInt128((decimal)a1 * (decimal)a2);
    public static UInt128 operator /(UInt128 a1, UInt128 a2) => a1.Division(a2);        // new UInt128((decimal)a1 / (decimal)a2);
    public static UInt128 operator %(UInt128 a1, UInt128 a2) => a1.Modulo(a2);          // new UInt128((decimal)a1 % (decimal)a2);

    public static UInt128 operator +(UInt128 a1, ulong a2) => a1.Addition(a2);
    public static UInt128 operator -(UInt128 a1, ulong a2) => a1.Subtraction(a2);
    public static UInt128 operator *(UInt128 a1, ulong a2) => a1.Multiplication(a2);
    public static UInt128 operator /(UInt128 a1, ulong a2) => a1.Division(a2);
    public static UInt128 operator %(UInt128 a1, ulong a2) => a1.Modulo(a2);

    public static UInt128 operator +(UInt128 a1, long a2) => a1.Addition(a2);
    public static UInt128 operator -(UInt128 a1, long a2) => a1.Subtraction(a2);
    public static UInt128 operator *(UInt128 a1, long a2) => a1.Multiplication(a2);
    public static UInt128 operator /(UInt128 a1, long a2) => a1.Division(a2);
    public static UInt128 operator %(UInt128 a1, long a2) => a1.Modulo(a2);

    public static UInt128 operator +(UInt128 a1, uint a2) => a1.Addition(a2);
    public static UInt128 operator -(UInt128 a1, uint a2) => a1.Subtraction(a2);
    public static UInt128 operator *(UInt128 a1, uint a2) => a1.Multiplication(a2);
    public static UInt128 operator /(UInt128 a1, uint a2) => a1.Division(a2);
    public static UInt128 operator %(UInt128 a1, uint a2) => a1.Modulo(a2);

    // int는 C#에 기본적인 표현이기 때문에 지정해주어야 한다.
    public static UInt128 operator +(UInt128 a1, int a2) => a1.Addition(a2);
    public static UInt128 operator -(UInt128 a1, int a2) => a1.Subtraction(a2);
    public static UInt128 operator *(UInt128 a1, int a2) => a1.Multiplication(a2);
    public static UInt128 operator /(UInt128 a1, int a2) => a1.Division(a2);
    public static UInt128 operator %(UInt128 a1, int a2) => a1.Modulo(a2);
    #endregion

    #region 비트 연산자
    public static UInt128 operator |(UInt128 a1, UInt128 a2) => new UInt128(a1.Low | a2.Low, a1.High | a2.High);
    public static UInt128 operator &(UInt128 a1, UInt128 a2) => new UInt128(a1.Low & a2.Low, a1.High & a2.High);
    public static UInt128 operator ^(UInt128 a1, UInt128 a2) => new UInt128(a1.Low ^ a2.Low, a1.High ^ a2.High);

    // class는 기본적으로 참조 형식으로 전달하게 됩니다.
    // 이는 (ref struct)형태와 같으며, 필요에 따라 쉽게 복제할 수 있어야 합니다.
    // (원하면 ~연산자를 ! 연산자 처럼 작동하도록 만드십시오)
    public static UInt128 operator ~(UInt128 a1) => new UInt128(a1);
    // 단항 부정연산자, 모든 비트에 대해 XOR 합니다. 다른 자료형의 ~연산자와 같은 역할입니다.
    // 음수를 표현할 수 없기 때문에 대체됩니다.
    // Int128을 이용하려는 경우 ! 연산자는 signed(부호, 최상단 비트) 를 xor 하는 역할을 맡아야합니다.
    public static UInt128 operator !(UInt128 a1) => new UInt128(a1 ^ MaxValue);

    public static UInt128 operator <<(UInt128 a1, int shift) => a1._BitLeftShift(shift);
    public static UInt128 operator >>(UInt128 a1, int shift) => a1._BitRightShift(shift);
    #endregion
    #endregion
}
