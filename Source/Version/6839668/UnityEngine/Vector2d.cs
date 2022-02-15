using System;

namespace UnityEngine;

public struct Vector2d
{
	public const double kEpsilon = 1E-05;

	public double x;

	public double y;

	public double this[int index]
	{
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				_ => throw new IndexOutOfRangeException("Invalid Vector2d index!"), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid Vector2d index!");
			}
		}
	}

	public Vector2d normalized
	{
		get
		{
			Vector2d result = new Vector2d(x, y);
			result.Normalize();
			return result;
		}
	}

	public double magnitude => Mathd.Sqrt(x * x + y * y);

	public double sqrMagnitude => x * x + y * y;

	public static Vector2d zero => new Vector2d(0.0, 0.0);

	public static Vector2d one => new Vector2d(1.0, 1.0);

	public static Vector2d up => new Vector2d(0.0, 1.0);

	public static Vector2d right => new Vector2d(1.0, 0.0);

	public Vector2d(double x, double y)
	{
		this.x = x;
		this.y = y;
	}

	public static implicit operator Vector2d(Vector3d v)
	{
		return new Vector2d(v.x, v.y);
	}

	public static implicit operator Vector3d(Vector2d v)
	{
		return new Vector3d(v.x, v.y, 0.0);
	}

	public static Vector2d operator +(Vector2d a, Vector2d b)
	{
		return new Vector2d(a.x + b.x, a.y + b.y);
	}

	public static Vector2d operator -(Vector2d a, Vector2d b)
	{
		return new Vector2d(a.x - b.x, a.y - b.y);
	}

	public static Vector2d operator -(Vector2d a)
	{
		return new Vector2d(0.0 - a.x, 0.0 - a.y);
	}

	public static Vector2d operator *(Vector2d a, double d)
	{
		return new Vector2d(a.x * d, a.y * d);
	}

	public static Vector2d operator *(float d, Vector2d a)
	{
		return new Vector2d(a.x * (double)d, a.y * (double)d);
	}

	public static Vector2d operator /(Vector2d a, double d)
	{
		return new Vector2d(a.x / d, a.y / d);
	}

	public static bool operator ==(Vector2d lhs, Vector2d rhs)
	{
		return SqrMagnitude(lhs - rhs) < 0.0;
	}

	public static bool operator !=(Vector2d lhs, Vector2d rhs)
	{
		return SqrMagnitude(lhs - rhs) >= 0.0;
	}

	public void Set(double new_x, double new_y)
	{
		x = new_x;
		y = new_y;
	}

	public static Vector2d Lerp(Vector2d from, Vector2d to, double t)
	{
		t = Mathd.Clamp01(t);
		return new Vector2d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t);
	}

	public static Vector2d MoveTowards(Vector2d current, Vector2d target, double maxDistanceDelta)
	{
		Vector2d vector2d = target - current;
		double num = vector2d.magnitude;
		if (num <= maxDistanceDelta || num == 0.0)
		{
			return target;
		}
		return current + vector2d / num * maxDistanceDelta;
	}

	public static Vector2d Scale(Vector2d a, Vector2d b)
	{
		return new Vector2d(a.x * b.x, a.y * b.y);
	}

	public void Scale(Vector2d scale)
	{
		x *= scale.x;
		y *= scale.y;
	}

	public void Normalize()
	{
		double num = magnitude;
		if (num > 9.99999974737875E-06)
		{
			this /= num;
		}
		else
		{
			this = zero;
		}
	}

	public override string ToString()
	{
		return "not implemented";
	}

	public string ToString(string format)
	{
		return "not implemented";
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2);
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector2d vector2d))
		{
			return false;
		}
		if (x.Equals(vector2d.x))
		{
			return y.Equals(vector2d.y);
		}
		return false;
	}

	public static double Dot(Vector2d lhs, Vector2d rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y;
	}

	public static double Angle(Vector2d from, Vector2d to)
	{
		return Mathd.Acos(Mathd.Clamp(Dot(from.normalized, to.normalized), -1.0, 1.0)) * 57.29578;
	}

	public static double Distance(Vector2d a, Vector2d b)
	{
		return (a - b).magnitude;
	}

	public static Vector2d ClampMagnitude(Vector2d vector, double maxLength)
	{
		if (vector.sqrMagnitude > maxLength * maxLength)
		{
			return vector.normalized * maxLength;
		}
		return vector;
	}

	public static double SqrMagnitude(Vector2d a)
	{
		return a.x * a.x + a.y * a.y;
	}

	public double SqrMagnitude()
	{
		return x * x + y * y;
	}

	public static Vector2d Min(Vector2d lhs, Vector2d rhs)
	{
		return new Vector2d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y));
	}

	public static Vector2d Max(Vector2d lhs, Vector2d rhs)
	{
		return new Vector2d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y));
	}
}
