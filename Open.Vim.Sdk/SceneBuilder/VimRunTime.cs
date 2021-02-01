using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vim.Geometry;

namespace Vim.SceneBuilder
{
    public class VimRunTimeNode
    {
        public Matrix4x4 Transform;
        public IMesh Geometry;
    }

    public static class VimRunTime
    {
    }
}
