using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
[CompilerGenerated]
public readonly struct FixedPoint2 : IComparable<FixedPoint2>, IEquatable<FixedPoint2>, IFormattable
{
	public int RawValue { get; }
	private FixedPoint2(int rawValue)
	{
		this.RawValue = rawValue;
	}
	
 public static FixedPoint2 FromObject(object fixedPointObject)
    {
        if (fixedPointObject == null)
        {
            throw new ArgumentNullException(nameof(fixedPointObject), "Исходный объект FixedPoint2 не может быть null.");
        }
        
        if (fixedPointObject is int intVal)
        {
             return new FixedPoint2(intVal);
        }
        
        if (fixedPointObject is FixedPoint2 fp)
        {
            return fp;
        }

        try
        {
            var type = fixedPointObject.GetType();
            
            var propInfo = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (propInfo != null)
            {
                var val = propInfo.GetValue(fixedPointObject);
                if (val is int iVal)
                {
                    return new FixedPoint2(iVal);
                }
            }
            
            var fieldInfo = type.GetField("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                var val = fieldInfo.GetValue(fixedPointObject);
                if (val is int iVal)
                {
                    return new FixedPoint2(iVal);
                }
            }
            
             var rawPropInfo = type.GetProperty("RawValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (rawPropInfo != null)
            {
                var val = rawPropInfo.GetValue(fixedPointObject);
                if (val is int iVal)
                {
                    return new FixedPoint2(iVal);
                }
            }

            throw new ArgumentException($"Объект типа '{type.FullName}' не имеет доступного свойства/поля 'Value' или 'RawValue' типа int.");
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Неожиданная ошибка при попытке получить значение из объекта типа '{fixedPointObject.GetType().FullName}'.", ex);
        }
    }

	public static FixedPoint2 FromInt(int value)
	{
		FixedPoint2 dmgVal;
		try
		{
			dmgVal = new FixedPoint2(checked(value * 100));
		}
		catch (OverflowException ex)
		{
			string text = "value";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Значение ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(value);
			defaultInterpolatedStringHandler.AppendLiteral(" слишком велико для преобразования в UniversalFixedPoint2.");
			throw new ArgumentOutOfRangeException(text, defaultInterpolatedStringHandler.ToStringAndClear(), ex.ToString());
		}
		return dmgVal;
	}
	public static FixedPoint2 FromFloat(float value)
	{
		return new FixedPoint2((int)(value * 100f + 1E-05f * (float)Math.Sign(value)));
	}
	public static FixedPoint2 FromDouble(double value)
	{
		return new FixedPoint2((int)(value * 100.0 + 9.999999747378752E-06 * (double)Math.Sign(value)));
	}
	public static FixedPoint2 FromRawValue(int rawValue)
	{
		return new FixedPoint2(rawValue);
	}
	public static FixedPoint2 Zero { get; } = new FixedPoint2(0);
	public static FixedPoint2 Epsilon { get; } = new FixedPoint2(1);
	public static FixedPoint2 MaxValue { get; } = new FixedPoint2(int.MaxValue);
	public float ToFloat()
	{
		return (float)this.RawValue / 100f;
	}
	public double ToDouble()
	{
		return (double)this.RawValue / 100.0;
	}
	public int ToInt()
	{
		return this.RawValue / 100;
	}
	public static FixedPoint2 operator +(FixedPoint2 a, FixedPoint2 b)
	{
		return new FixedPoint2(a.RawValue + b.RawValue);
	}
	public static FixedPoint2 operator -(FixedPoint2 a, FixedPoint2 b)
	{
		return new FixedPoint2(a.RawValue - b.RawValue);
	}
	public static FixedPoint2 operator -(FixedPoint2 a)
	{
		return new FixedPoint2(-a.RawValue);
	}
	public static FixedPoint2 operator *(FixedPoint2 a, int scalar)
	{
		return new FixedPoint2(a.RawValue * scalar);
	}
	public static FixedPoint2 operator /(FixedPoint2 a, int scalar)
	{
		return new FixedPoint2(a.RawValue / scalar);
	}
	public static FixedPoint2 operator *(FixedPoint2 a, float b)
	{
		return FixedPoint2.FromFloat(a.ToFloat() * b);
	}
	public static FixedPoint2 operator *(FixedPoint2 a, double b)
	{
		return FixedPoint2.FromDouble(a.ToDouble() * b);
	}
	public static FixedPoint2 operator /(FixedPoint2 a, float b)
	{
		return FixedPoint2.FromFloat(a.ToFloat() / b);
	}
	public static FixedPoint2 operator /(FixedPoint2 a, double b)
	{
		return FixedPoint2.FromDouble(a.ToDouble() / b);
	}
	public static bool operator ==(FixedPoint2 a, FixedPoint2 b)
	{
		return a.RawValue == b.RawValue;
	}
	public static bool operator !=(FixedPoint2 a, FixedPoint2 b)
	{
		return !(a == b);
	}
	public static bool operator <(FixedPoint2 a, FixedPoint2 b)
	{
		return a.RawValue < b.RawValue;
	}
	public static bool operator >(FixedPoint2 a, FixedPoint2 b)
	{
		return a.RawValue > b.RawValue;
	}
	public static bool operator <=(FixedPoint2 a, FixedPoint2 b)
	{
		return a.RawValue <= b.RawValue;
	}
	public static bool operator >=(FixedPoint2 a, FixedPoint2 b)
	{
		return a.RawValue >= b.RawValue;
	}
	
	public override bool Equals(object obj)
	{
		bool flag;
		if (obj is FixedPoint2)
		{
			FixedPoint2 dmgVal = (FixedPoint2)obj;
			flag = this.Equals(dmgVal);
		}
		else
		{
			flag = false;
		}
		return flag;
	}
	public bool Equals(FixedPoint2 other)
	{
		return this.RawValue == other.RawValue;
	}
	public int CompareTo(FixedPoint2 other)
	{
		return this.RawValue.CompareTo(other.RawValue);
	}
	public override int GetHashCode()
	{
		return this.RawValue.GetHashCode();
	}
	
	public override string ToString()
	{
		return this.ToDouble().ToString("0.##", CultureInfo.InvariantCulture);
	}
	
	public string ToString(string format, IFormatProvider formatProvider)
	{
		return this.ToDouble().ToString(format ?? "0.##", formatProvider ?? CultureInfo.InvariantCulture);
	}
	public static FixedPoint2 Abs(FixedPoint2 a)
	{
		return FixedPoint2.FromRawValue(Math.Abs(a.RawValue));
	}
	public static FixedPoint2 Min(FixedPoint2 a, FixedPoint2 b)
	{
		return (a < b) ? a : b;
	}
	public static FixedPoint2 Max(FixedPoint2 a, FixedPoint2 b)
	{
		return (a > b) ? a : b;
	}
	public static FixedPoint2 Clamp(FixedPoint2 number, FixedPoint2 min, FixedPoint2 max)
	{
		bool flag = min > max;
		if (flag)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 4);
			defaultInterpolatedStringHandler.AppendFormatted("min");
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(min);
			defaultInterpolatedStringHandler.AppendLiteral(" не может быть больше ");
			defaultInterpolatedStringHandler.AppendFormatted("max");
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(max);
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		return (number < min) ? min : ((number > max) ? max : number);
	}
	private const int ScalingFactor = 100;
	private const float FloatEpsilon = 1E-05f;
}
