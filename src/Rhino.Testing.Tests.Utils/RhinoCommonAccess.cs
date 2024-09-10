using Rhino.Geometry;

namespace Rhino.Testing.Tests.Utils
{
    public static class RhinoCommonAccess
    {
        public static Point3d CreatePoint(double x, double y, double z) => new(x, y, z);
    }
}
