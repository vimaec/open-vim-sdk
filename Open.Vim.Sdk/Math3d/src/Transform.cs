using System;
using System.Runtime.Serialization;

namespace Vim.Math3d
{
    [DataContract]
    public class Transform : IEquatable<Transform>
    {
        [DataMember]
        public readonly Vector3 Position;

        [DataMember]
        public readonly Quaternion Orientation;

        public Transform()
            : this(Vector3.Zero, Quaternion.Identity)
        {  }

        public Transform(Vector3 position, Quaternion orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        public Transform((Vector3, Quaternion) tuple)
            : this(tuple.Item1, tuple.Item2) { }

        public override bool Equals(object obj)
            => obj is Transform other && Equals(other);

        public bool Equals(Transform other)
            => Position.Equals(other.Position) && Orientation.Equals(other.Orientation);

        public bool AlmostEquals(Transform other, float tolerance = Constants.Tolerance)
            => Position.AlmostEquals(other.Position, tolerance) &&
               Orientation.AlmostEquals(other.Orientation, tolerance);

        public override int GetHashCode()
            => Hash.Combine(Position.GetHashCode(), Orientation.GetHashCode());

        public Transform SetPosition(Vector3 position)
            => new Transform(position, Orientation);

        public Transform SetOrientation(Quaternion orientation)
            => new Transform(Position, orientation);
    }
}
