using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using System.Numerics;

namespace make_primitive_outlines
{
    using VERTEX = VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>;

    public static class MeshUtils
    {
        public static void AddCube<TMaterial>(this IMeshBuilder<TMaterial> meshBuilder, TMaterial material, Matrix4x4 xform)
        {
            var cube = new Cube<TMaterial>(material);

            cube.AddTo(meshBuilder, xform);
        }

    }

    class Cube<TMaterial> : ParametricShape<TMaterial>
    {
        // TODO:
        // Faces and UV alignment should follow: https://github.com/KhronosGroup/glTF/blob/master/extensions/2.0/Vendor/EXT_lights_image_based/figures/Cube_map.svg

        #region lifecycle

        public Cube(TMaterial material)
        {
            _Front = _Back = _Left = _Right = _Top = _Bottom = material;
        }

        public Cube(TMaterial material, float width, float height, float length)
        {
            _Front = _Back = _Left = _Right = _Top = _Bottom = material;
            _Size = new Vector3(width, height, length);
        }

        #endregion

        #region data

        private Vector3 _Size = Vector3.One;

        private TMaterial _Front;
        private TMaterial _Back;

        private TMaterial _Left;
        private TMaterial _Right;

        private TMaterial _Top;
        private TMaterial _Bottom;

        #endregion

        #region API

        public override void AddTo(IMeshBuilder<TMaterial> meshBuilder, Matrix4x4 xform)
        {
            var x = Vector3.UnitX * _Size.X * 0.5f;
            var y = Vector3.UnitY * _Size.Y * 0.5f;
            var z = Vector3.UnitZ * _Size.Z * 0.5f;

            _AddCubeFace(meshBuilder.UsePrimitive(_Right), x, y, z, xform);
            _AddCubeFace(meshBuilder.UsePrimitive(_Left), -x, z, y, xform);

            _AddCubeFace(meshBuilder.UsePrimitive(_Top), y, z, x, xform);
            _AddCubeFace(meshBuilder.UsePrimitive(_Bottom), -y, x, z, xform);

            _AddCubeFace(meshBuilder.UsePrimitive(_Front), z, x, y, xform);
            _AddCubeFace(meshBuilder.UsePrimitive(_Back), -z, y, x, xform);
        }

        private static void _AddCubeFace(IPrimitiveBuilder primitiveBuilder, Vector3 origin, Vector3 axisX, Vector3 axisY, Matrix4x4 xform)
        {
            var p1 = Vector3.Transform(origin - axisX - axisY, xform);
            var p2 = Vector3.Transform(origin + axisX - axisY, xform);
            var p3 = Vector3.Transform(origin + axisX + axisY, xform);
            var p4 = Vector3.Transform(origin - axisX + axisY, xform);
            var n = Vector3.Normalize(Vector3.TransformNormal(origin, xform));

            primitiveBuilder.AddQuadrangle
                (
                new VERTEX((p1, n), (Vector4.One, Vector2.Zero)),
                new VERTEX((p2, n), (Vector4.One, Vector2.UnitX)),
                new VERTEX((p3, n), (Vector4.One, Vector2.One)),
                new VERTEX((p4, n), (Vector4.One, Vector2.UnitY))
                );
        }

        #endregion
    }

    abstract class ParametricShape<TMaterial>
    {
        public abstract void AddTo(IMeshBuilder<TMaterial> meshBuilder, Matrix4x4 xform);

        public MeshBuilder<TMaterial, VertexPositionNormal, VertexColor1Texture1, VertexEmpty> ToMesh(Matrix4x4 xform)
        {
            var mesh = new MeshBuilder<TMaterial, VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();

            AddTo(mesh, xform);

            return mesh;
        }
    }



}
