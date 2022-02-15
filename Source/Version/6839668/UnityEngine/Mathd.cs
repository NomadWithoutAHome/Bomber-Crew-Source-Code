using System;
using System.Runtime.InteropServices;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Mathd
{
	public const double PI = 3.141593;

	public const double Infinity = double.PositiveInfinity;

	public const double NegativeInfinity = double.NegativeInfinity;

	public const double Deg2Rad = 0.01745329;

	public const double Rad2Deg = 57.29578;

	public const double Epsilon = 1.401298E-45;

	public static double Sin(double d)
	{
		return Math.Sin(d);
	}

	public static double Cos(double d)
	{
		return Math.Cos(d);
	}

	public static double Tan(double d)
	{
		return Math.Tan(d);
	}

	public static double Asin(double d)
	{
		return Math.Asin(d);
	}

	public static double Acos(double d)
	{
		return Math.Acos(d);
	}

	public static double Atan(double d)
	{
		return Math.Atan(d);
	}

	public static double Atan2(double y, double x)
	{
		return Math.Atan2(y, x);
	}

	public static double Sqrt(double d)
	{
		return Math.Sqrt(d);
	}

	public static double Abs(double d)
	{
		return Math.Abs(d);
	}

	public static int Abs(int value)
	{
		return Math.Abs(value);
	}

	public static double Min(double a, double b)
	{
		if (a < b)
		{
			return a;
		}
		return b;
	}

	public static double Min(params double[] values)
	{
		int num = values.Length;
		if (num == 0)
		{
			return 0.0;
		}
		double num2 = values[0];
		for (int i = 1; i < num; i++)
		{
			if (values[i] < num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}

	public static int Min(int a, int b)
	{
		if (a < b)
		{
			return a;
		}
		return b;
	}

	public static int Min(params int[] values)
	{
		int num = values.Length;
		if (num == 0)
		{
			return 0;
		}
		int num2 = values[0];
		for (int i = 1; i < num; i++)
		{
			if (values[i] < num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}

	public static double Max(double a, double b)
	{
		if (a > b)
		{
			return a;
		}
		return b;
	}

	public static double Max(params double[] values)
	{
		int num = values.Length;
		if (num == 0)
		{
			return 0.0;
		}
		double num2 = values[0];
		for (int i = 1; i < num; i++)
		{
			if (values[i] > num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}

	public static int Max(int a, int b)
	{
		if (a > b)
		{
			return a;
		}
		return b;
	}

	public static int Max(params int[] values)
	{
		int num = values.Length;
		if (num == 0)
		{
			return 0;
		}
		int num2 = values[0];
		for (int i = 1; i < num; i++)
		{
			if (values[i] > num2)
			{
				num2 = values[i];
			}
		}
		return num2;
	}

	public static double Pow(double d, double p)
	{
		return Math.Pow(d, p);
	}

	public static double Exp(double power)
	{
		return Math.Exp(power);
	}

	public static double Log(double d, double p)
	{
		return Math.Log(d, p);
	}

	public static double Log(double d)
	{
		return Math.Log(d);
	}

	public static double Log10(double d)
	{
		return Math.Log10(d);
	}

	public static double Ceil(double d)
	{
		return Math.Ceiling(d);
	}

	public static double Floor(double d)
	{
		return Math.Floor(d);
	}

	public static double Round(double d)
	{
		return Math.Round(d);
	}

	public static int CeilToInt(double d)
	{
		return (int)Math.Ceiling(d);
	}

	public static int FloorToInt(double d)
	{
		return (int)Math.Floor(d);
	}

	public static int RoundToInt(double d)
	{
		return (int)Math.Round(d);
	}

	public static double Sign(double d)
	{
		return (!(d >= 0.0)) ? (-1.0) : 1.0;
	}

	public static double Clamp(double value, double min, double max)
	{
		if (value < min)
		{
			value = min;
		}
		else if (value > max)
		{
			value = max;
		}
		return value;
	}

	public static int Clamp(int value, int min, int max)
	{
		if (value < min)
		{
			value = min;
		}
		else if (value > max)
		{
			value = max;
		}
		return value;
	}

	public static double Clamp01(double value)
	{
		if (value < 0.0)
		{
			return 0.0;
		}
		if (value > 1.0)
		{
			return 1.0;
		}
		return value;
	}

	public static double Lerp(double from, double to, double t)
	{
		return from + (to - from) * Clamp01(t);
	}

	public static double LerpAngle(double a, double b, double t)
	{
		double num = Repeat(b - a, 360.0);
		if (num > 180.0)
		{
			num -= 360.0;
		}
		return a + num * Clamp01(t);
	}

	public static double MoveTowards(double current, double target, double maxDelta)
	{
		if (Abs(target - current) <= maxDelta)
		{
			return target;
		}
		return current + Sign(target - current) * maxDelta;
	}

	public static double MoveTowardsAngle(double current, double target, double maxDelta)
	{
		target = current + DeltaAngle(current, target);
		return MoveTowards(current, target, maxDelta);
	}

	public static double SmoothStep(double from, double to, double t)
	{
		t = Clamp01(t);
		t = -2.0 * t * t * t + 3.0 * t * t;
		return to * t + from * (1.0 - t);
	}

	public static double Gamma(double value, double absmax, double gamma)
	{
		bool flag = false;
		if (value < 0.0)
		{
			flag = true;
		}
		double num = Abs(value);
		if (num > absmax)
		{
			if (flag)
			{
				return 0.0 - num;
			}
			return num;
		}
		double num2 = Pow(num / absmax, gamma) * absmax;
		if (flag)
		{
			return 0.0 - num2;
		}
		return num2;
	}

	public static bool Approximately(double a, double b)
	{
		return Abs(b - a) < Max(1E-06 * Max(Abs(a), Abs(b)), 1.121039E-44);
	}

	public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed)
	{
		double deltaTime = Time.deltaTime;
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime)
	{
		double deltaTime = Time.deltaTime;
		double maxSpeed = double.PositiveInfinity;
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
	{
		smoothTime = Max(0.0001, smoothTime);
		double num = 2.0 / smoothTime;
		double num2 = num * deltaTime;
		double num3 = 1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2);
		double value = current - target;
		double num4 = target;
		double num5 = maxSpeed * smoothTime;
		double num6 = Clamp(value, 0.0 - num5, num5);
		target = current - num6;
		double num7 = (currentVelocity + num * num6) * deltaTime;
		currentVelocity = (currentVelocity - num * num7) * num3;
		double num8 = target + (num6 + num7) * num3;
		if (num4 - current > 0.0 == num8 > num4)
		{
			num8 = num4;
			currentVelocity = (num8 - num4) / deltaTime;
		}
		return num8;
	}

	public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed)
	{
		double deltaTime = Time.deltaTime;
		return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime)
	{
		double deltaTime = Time.deltaTime;
		double maxSpeed = double.PositiveInfinity;
		return SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
	{
		target = current + DeltaAngle(current, target);
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static double Repeat(double t, double length)
	{
		return t - Floor(t / length) * length;
	}

	public static double PingPong(double t, double length)
	{
		t = Repeat(t, length * 2.0);
		return length - Abs(t - length);
	}

	public static double InverseLerp(double from, double to, double value)
	{
		if (from < to)
		{
			if (value < from)
			{
				return 0.0;
			}
			if (value > to)
			{
				return 1.0;
			}
			value -= from;
			value /= to - from;
			return value;
		}
		if (from <= to)
		{
			return 0.0;
		}
		if (value < to)
		{
			return 1.0;
		}
		if (value > from)
		{
			return 0.0;
		}
		return 1.0 - (value - to) / (from - to);
	}

	public static double DeltaAngle(double current, double target)
	{
		double num = Repeat(target - current, 360.0);
		if (num > 180.0)
		{
			num -= 360.0;
		}
		return num;
	}

	internal static bool LineIntersection(Vector2d p1, Vector2d p2, Vector2d p3, Vector2d p4, ref Vector2d result)
	{
		double num = p2.x - p1.x;
		double num2 = p2.y - p1.y;
		double num3 = p4.x - p3.x;
		double num4 = p4.y - p3.y;
		double num5 = num * num4 - num2 * num3;
		if (num5 == 0.0)
		{
			return false;
		}
		double num6 = p3.x - p1.x;
		double num7 = p3.y - p1.y;
		double num8 = (num6 * num4 - num7 * num3) / num5;
		result = new Vector2d(p1.x + num8 * num, p1.y + num8 * num2);
		return true;
	}

	internal static bool LineSegmentIntersection(Vector2d p1, Vector2d p2, Vector2d p3, Vector2d p4, ref Vector2d result)
	{
		double num = p2.x - p1.x;
		double num2 = p2.y - p1.y;
		double num3 = p4.x - p3.x;
		double num4 = p4.y - p3.y;
		double num5 = num * num4 - num2 * num3;
		if (num5 == 0.0)
		{
			return false;
		}
		double num6 = p3.x - p1.x;
		double num7 = p3.y - p1.y;
		double num8 = (num6 * num4 - num7 * num3) / num5;
		if (num8 < 0.0 || num8 > 1.0)
		{
			return false;
		}
		double num9 = (num6 * num2 - num7 * num) / num5;
		if (num9 < 0.0 || num9 > 1.0)
		{
			return false;
		}
		result = new Vector2d(p1.x + num8 * num, p1.y + num8 * num2);
		return true;
	}
}
