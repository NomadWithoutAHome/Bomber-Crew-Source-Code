using System;

namespace UnityEngine;

public struct Vector3d
{
	public const float kEpsilon = 1E-05f;

	public double x;

	public double y;

	public double z;

	public double this[int index]
	{
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				_ => throw new IndexOutOfRangeException("Invalid index!"), 
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
			case 2:
				z = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid Vector3d index!");
			}
		}
	}

	public Vector3d normalized => Normalize(this);

	public double magnitude => Math.Sqrt(x * x + y * y + z * z);

	public double sqrMagnitude => x * x + y * y + z * z;

	public static Vector3d zero => new Vector3d(0.0, 0.0, 0.0);

	public static Vector3d one => new Vector3d(1.0, 1.0, 1.0);

	public static Vector3d forward => new Vector3d(0.0, 0.0, 1.0);

	public static Vector3d back => new Vector3d(0.0, 0.0, -1.0);

	public static Vector3d up => new Vector3d(0.0, 1.0, 0.0);

	public static Vector3d down => new Vector3d(0.0, -1.0, 0.0);

	public static Vector3d left => new Vector3d(-1.0, 0.0, 0.0);

	public static Vector3d right => new Vector3d(1.0, 0.0, 0.0);

	[Obsolete("Use Vector3d.forward instead.")]
	public static Vector3d fwd => new Vector3d(0.0, 0.0, 1.0);

	public Vector3d(double x, double y, double z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3d(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3d(Vector3 v3)
	{
		x = v3.x;
		y = v3.y;
		z = v3.z;
	}

	public Vector3d(double x, double y)
	{
		this.x = x;
		this.y = y;
		z = 0.0;
	}

	public static Vector3d operator +(Vector3d a, Vector3d b)
	{
		return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3d operator -(Vector3d a, Vector3d b)
	{
		return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector3d operator -(Vector3d a)
	{
		return new Vector3d(0.0 - a.x, 0.0 - a.y, 0.0 - a.z);
	}

	public static Vector3d operator *(Vector3d a, double d)
	{
		return new Vector3d(a.x * d, a.y * d, a.z * d);
	}

	public static Vector3d operator *(double d, Vector3d a)
	{
		return new Vector3d(a.x * d, a.y * d, a.z * d);
	}

	public static Vector3d operator /(Vector3d a, double d)
	{
		return new Vector3d(a.x / d, a.y / d, a.z / d);
	}

	public static bool operator ==(Vector3d lhs, Vector3d rhs)
	{
		return SqrMagnitude(lhs - rhs) < 0.0;
	}

	public static bool operator !=(Vector3d lhs, Vector3d rhs)
	{
		return SqrMagnitude(lhs - rhs) >= 0.0;
	}

	public static explicit operator Vector3(Vector3d vector3d)
	{
		return new Vector3((float)vector3d.x, (float)vector3d.y, (float)vector3d.z);
	}

	public static Vector3d Lerp(Vector3d from, Vector3d to, double t)
	{
		t = Mathd.Clamp01(t);
		return new Vector3d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
	}

	public static Vector3d Slerp(Vector3d from, Vector3d to, double t)
	{
		Vector3 v = Vector3.Slerp((Vector3)from, (Vector3)to, (float)t);
		return new Vector3d(v);
	}

	public static void OrthoNormalize(ref Vector3d normal, ref Vector3d tangent)
	{
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		vector = (Vector3)normal;
		vector2 = (Vector3)tangent;
		Vector3.OrthoNormalize(ref vector, ref vector2);
		normal = new Vector3d(vector);
		tangent = new Vector3d(vector2);
	}

	public static void OrthoNormalize(ref Vector3d normal, ref Vector3d tangent, ref Vector3d binormal)
	{
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		vector = (Vector3)normal;
		vector2 = (Vector3)tangent;
		vector3 = (Vector3)binormal;
		Vector3.OrthoNormalize(ref vector, ref vector2, ref vector3);
		normal = new Vector3d(vector);
		tangent = new Vector3d(vector2);
		binormal = new Vector3d(vector3);
	}

	public static Vector3d MoveTowards(Vector3d current, Vector3d target, double maxDistanceDelta)
	{
		Vector3d vector3d = target - current;
		double num = vector3d.magnitude;
		if (num <= maxDistanceDelta || num == 0.0)
		{
			return target;
		}
		return current + vector3d / num * maxDistanceDelta;
	}

	public static Vector3d RotateTowards(Vector3d current, Vector3d target, double maxRadiansDelta, double maxMagnitudeDelta)
	{
		Vector3 v = Vector3.RotateTowards((Vector3)current, (Vector3)target, (float)maxRadiansDelta, (float)maxMagnitudeDelta);
		return new Vector3d(v);
	}

	public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed)
	{
		double deltaTime = Time.deltaTime;
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime)
	{
		double deltaTime = Time.deltaTime;
		double maxSpeed = double.PositiveInfinity;
		return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
	}

	public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
	{
		smoothTime = Mathd.Max(0.0001, smoothTime);
		double num = 2.0 / smoothTime;
		double num2 = num * deltaTime;
		double num3 = 1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2);
		Vector3d vector = current - target;
		Vector3d vector3d = target;
		double maxLength = maxSpeed * smoothTime;
		Vector3d vector3d2 = ClampMagnitude(vector, maxLength);
		target = current - vector3d2;
		Vector3d vector3d3 = (currentVelocity + num * vector3d2) * deltaTime;
		currentVelocity = (currentVelocity - num * vector3d3) * num3;
		Vector3d vector3d4 = target + (vector3d2 + vector3d3) * num3;
		if (Dot(vector3d - current, vector3d4 - vector3d) > 0.0)
		{
			vector3d4 = vector3d;
			currentVelocity = (vector3d4 - vector3d) / deltaTime;
		}
		return vector3d4;
	}

	public void Set(double new_x, double new_y, double new_z)
	{
		x = new_x;
		y = new_y;
		z = new_z;
	}

	public static Vector3d Scale(Vector3d a, Vector3d b)
	{
		return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public void Scale(Vector3d scale)
	{
		x *= scale.x;
		y *= scale.y;
		z *= scale.z;
	}

	public static Vector3d Cross(Vector3d lhs, Vector3d rhs)
	{
		return new Vector3d(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector3d vector3d))
		{
			return false;
		}
		if (x.Equals(vector3d.x) && y.Equals(vector3d.y))
		{
			return z.Equals(vector3d.z);
		}
		return false;
	}

	public static Vector3d Reflect(Vector3d inDirection, Vector3d inNormal)
	{
		return -2.0 * Dot(inNormal, inDirection) * inNormal + inDirection;
	}

	public static Vector3d Normalize(Vector3d value)
	{
		double num = Magnitude(value);
		if (num > 9.99999974737875E-06)
		{
			return value / num;
		}
		return zero;
	}

	public void Normalize()
	{
		double num = Magnitude(this);
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
		return "(" + x + " : " + y + " : " + z + ")";
	}

	public static double Dot(Vector3d lhs, Vector3d rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
	}

	public static Vector3d Project(Vector3d vector, Vector3d onNormal)
	{
		double num = Dot(onNormal, onNormal);
		if (num < 1.40129846432482E-45)
		{
			return zero;
		}
		return onNormal * Dot(vector, onNormal) / num;
	}

	public static Vector3d Exclude(Vector3d excludeThis, Vector3d fromThat)
	{
		return fromThat - Project(fromThat, excludeThis);
	}

	public static double Angle(Vector3d from, Vector3d to)
	{
		return Mathd.Acos(Mathd.Clamp(Dot(from.normalized, to.normalized), -1.0, 1.0)) * 57.29578;
	}

	public static double Distance(Vector3d a, Vector3d b)
	{
		Vector3d vector3d = new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
		return Math.Sqrt(vector3d.x * vector3d.x + vector3d.y * vector3d.y + vector3d.z * vector3d.z);
	}

	public static Vector3d ClampMagnitude(Vector3d vector, double maxLength)
	{
		if (vector.sqrMagnitude > maxLength * maxLength)
		{
			return vector.normalized * maxLength;
		}
		return vector;
	}

	public static double Magnitude(Vector3d a)
	{
		return Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
	}

	public static double SqrMagnitude(Vector3d a)
	{
		return a.x * a.x + a.y * a.y + a.z * a.z;
	}

	public static Vector3d Min(Vector3d lhs, Vector3d rhs)
	{
		return new Vector3d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y), Mathd.Min(lhs.z, rhs.z));
	}

	public static Vector3d Max(Vector3d lhs, Vector3d rhs)
	{
		return new Vector3d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y), Mathd.Max(lhs.z, rhs.z));
	}

	[Obsolete("Use Vector3d.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
	public static double AngleBetween(Vector3d from, Vector3d to)
	{
		return Mathd.Acos(Mathd.Clamp(Dot(from.normalized, to.normalized), -1.0, 1.0));
	}
}
