// MIT License
// Copyright (C) 2019 VIMaec LLC.
// Copyright (C) 2019 Ara 3D. Inc
// https://ara3d.com
// Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.CompilerServices;

namespace Vim.Math3d
{
    public partial struct Ray : ITransformable3D<Ray>
    {
        // adapted from http://www.scratchapixel.com/lessons/3d-basic-lessons/lesson-7-intersecting-simple-shapes/ray-box-intersection/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float? Intersects(AABox box)
        {
            const float Epsilon = 1e-6f;

            float? tMin = null, tMax = null;

            if (Math.Abs(Direction.X) < Epsilon)
            {
                if (Position.X < box.Min.X || Position.X > box.Max.X)
                    return null;
            }
            else
            {
                tMin = (box.Min.X - Position.X) / Direction.X;
                tMax = (box.Max.X - Position.X) / Direction.X;

                if (tMin > tMax)
                {
                    var temp = tMin;
                    tMin = tMax;
                    tMax = temp;
                }
            }

            if (Math.Abs(Direction.Y) < Epsilon)
            {
                if (Position.Y < box.Min.Y || Position.Y > box.Max.Y)
                    return null;
            }
            else
            {
                var tMinY = (box.Min.Y - Position.Y) / Direction.Y;
                var tMaxY = (box.Max.Y - Position.Y) / Direction.Y;

                if (tMinY > tMaxY)
                {
                    var temp = tMinY;
                    tMinY = tMaxY;
                    tMaxY = temp;
                }

                if ((tMin.HasValue && tMin > tMaxY) || (tMax.HasValue && tMinY > tMax))
                    return null;

                if (!tMin.HasValue || tMinY > tMin) tMin = tMinY;
                if (!tMax.HasValue || tMaxY < tMax) tMax = tMaxY;
            }

            if (Math.Abs(Direction.Z) < Epsilon)
            {
                if (Position.Z < box.Min.Z || Position.Z > box.Max.Z)
                    return null;
            }
            else
            {
                var tMinZ = (box.Min.Z - Position.Z) / Direction.Z;
                var tMaxZ = (box.Max.Z - Position.Z) / Direction.Z;

                if (tMinZ > tMaxZ)
                {
                    var temp = tMinZ;
                    tMinZ = tMaxZ;
                    tMaxZ = temp;
                }

                if ((tMin.HasValue && tMin > tMaxZ) || (tMax.HasValue && tMinZ > tMax))
                    return null;

                if (!tMin.HasValue || tMinZ > tMin) tMin = tMinZ;
                if (!tMax.HasValue || tMaxZ < tMax) tMax = tMaxZ;
            }

            // having a positive tMin and a negative tMax means the ray is inside the box
            // we expect the intesection distance to be 0 in that case
            if (tMin.HasValue && tMin < 0 && tMax > 0) return 0;

            // a negative tMin means that the intersection point is behind the ray's origin
            // we discard these as not hitting the AABB
            if (tMin < 0) return null;

            return tMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float? Intersects(Plane plane, float tolerance = Constants.Tolerance)
        {
            var den = Vector3.Dot(Direction, plane.Normal);
            if (den.Abs() < tolerance)
                return null;

            var result = (-plane.D - Vector3.Dot(plane.Normal, Position)) / den;

            if (result < 0.0f)
            {
                if (result < -tolerance)
                {
                    return null;
                }

                result = 0.0f;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float? Intersects(Sphere sphere)
        {
            // Find the vector between where the ray starts the the sphere's centre
            var difference = sphere.Center - Position;
            var differenceLengthSquared = difference.LengthSquared();
            var sphereRadiusSquared = sphere.Radius * sphere.Radius;

            // If the distance between the ray start and the sphere's centre is less than
            // the radius of the sphere, it means we've intersected. N.B. checking the LengthSquared is faster.
            if (differenceLengthSquared < sphereRadiusSquared)
                return 0.0f;

            var distanceAlongRay = Vector3.Dot(Direction, difference);

            // If the ray is pointing away from the sphere then we don't ever intersect
            if (distanceAlongRay < 0)
                return null;

            // Next we kinda use Pythagoras to check if we are within the bounds of the sphere
            // if x = radius of sphere
            // if y = distance between ray position and sphere centre
            // if z = the distance we've travelled along the ray
            // if x^2 + z^2 - y^2 < 0, we do not intersect
            var dist = sphereRadiusSquared + distanceAlongRay.Sqr() - differenceLengthSquared;
            return (dist < 0) ? null : distanceAlongRay - (float?)Math.Sqrt(dist);
        }

        public Ray Transform(Matrix4x4 mat)
            => new Ray(Position.Transform(mat), Direction.TransformNormal(mat));

        public float Intersects(Triangle tri, float tolerance = Constants.Tolerance)
        {
            /*
            var e1 = tri.B- tri.A;
            var e2 = tri.C - tri.A;
            var p = Direction.Cross(e2);
            var a = e1.Dot(p);
            if (a.Abs() < tolerance)
                return -1.0f;

            var f = 1 / a;
            var s = Position - tri.A;
            var u = f * (s.Dot(p));
            if (u < 0.0 || u > 1.0)
                return -1.0f;

            var q = s.Cross(e1);
            var v = f * Direction.Dot(q);
            if (v < 0.0f || u + v > 1.0f)
                return -1.0f;

            return f * e2.Dot(q);
            */

            // https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/ray-triangle-intersection-geometric-solution

            // compute plane's normal
            var v0 = tri.A;
            var v1 = tri.B;
            var v2 = tri.C;
            var v0v1 = v1 - v0;
            var v0v2 = v2 - v0;
            // no need to normalize
            var normal = v0v1.Cross(v0v2); // N 
            
            // Step 1: finding P

            // check if ray and plane are parallel ?
            var normalDotDirection = normal.Dot(Direction);
            if (normalDotDirection.Abs() < tolerance) // almost 0 
                return -1; // they are parallel so they don't intersect ! 

            // compute d parameter using equation 2
            var d = normal.Dot(v0);

            // compute t (equation 3)
            var t = (normal.Dot(Position) + d) / normalDotDirection;
            // check if the triangle is in behind the ray
            if (t < 0) return -1; // the triangle is behind 

            // compute the intersection point using equation 1
            var P = Position + t * Direction;

            // Step 2: inside-outside test

            // edge 0
            var edge0 = v1 - v0;
            var vp0 = P - v0;
            // vector perpendicular to triangle's plane 
            var C = edge0.Cross(vp0);
            if (normal.Dot(C) < 0) return -t; // P is on the right side 

            // edge 1
            var edge1 = v2 - v1;
            var vp1 = P - v1;
            // vector perpendicular to triangle's plane 
            C = edge1.Cross(vp1);
            if (normal.Dot(C) < 0) return -t; // P is on the right side 

            // edge 2
            var edge2 = v0 - v2;
            var vp2 = P - v2;
            C = edge2.Cross(vp2);
            if (normal.Dot(C) < 0) return -t; // P is on the right side; 

            return t; // this ray hits the triangle 
        }
    }
}
