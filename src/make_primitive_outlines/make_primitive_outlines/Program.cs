using make_primitive_outlines;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using System.Numerics;
using VPOSNRM = SharpGLTF.Geometry.VertexTypes.VertexPositionNormal;

var material1 = new MaterialBuilder()
    .WithDoubleSide(true)
    .WithMetallicRoughnessShader()
    .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(0, 1, 0, 1));

var mesh = new MeshBuilder<VPOSNRM>("mesh");
mesh.AddCube(material1, Matrix4x4.Identity);
var pivot1 = new NodeBuilder("Cube1").WithLocalTranslation(new Vector3(0, 0, 0));
var scene = new SceneBuilder();
scene.AddRigidMesh(mesh, pivot1);
var model = scene.ToGltf2();
var originalIndices = model.LogicalMeshes[0].Primitives[0].IndexAccessor.AsIndicesArray().ToArray();
var INDICES_PER_QUAD = 6;
var quads = originalIndices.Count() / INDICES_PER_QUAD;

var quadBytes = new List<byte>();

for (var i = 0; i < quads; i++)
{
    var start_index = INDICES_PER_QUAD * i;
    var end_index = start_index + INDICES_PER_QUAD;

    var indices = originalIndices[start_index..end_index];
    var a = indices[0];
    var b = indices[1];
    var c = indices[2];
    var d = indices[5];

    var quad_edges = new uint[] {
            a, b,
            b, c,
            a, d,
            c, d
    };

    foreach (var item in quad_edges)
    {
        var bytes = BitConverter.GetBytes((UInt16)item).ToList();
        quadBytes.AddRange(bytes);
    }
}

//var quadBytes1 = new List<byte>();
//for (var i = 1; i < originalIndices.Length; i++)
//{
//    var b1 = BitConverter.GetBytes((UInt16)originalIndices[i - 1]);
//    var b2 = BitConverter.GetBytes((UInt16)originalIndices[i]);
//    quadBytes1.AddRange(b1);
//    quadBytes1.AddRange(b2);
//}

var buffer = model.UseBufferView(quadBytes.ToArray());
var accessor = model.CreateAccessor("Animation.Input");
accessor.SetData(buffer, 0, quadBytes.Count / 2, DimensionType.SCALAR, EncodingType.UNSIGNED_SHORT, false);
// accessor.UpdateBounds();

var gltf = "test10.gltf";
model.SaveGLTF(gltf);

var o1 = JObject.Parse(File.ReadAllText(gltf));
string[] extensionsUsed = new List<string>() { "CESIUM_primitive_outline" }.ToArray();
o1.Add("extensionsUsed", JArray.FromObject(extensionsUsed));

var ext = new Extensions();
ext.CESIUM_primitive_outline = new CESIUM_Primitive_Outline() { indices = accessor.LogicalIndex };

o1["meshes"][0]["primitives"][0]["extensions"] = JObject.FromObject(ext);

var res = JsonConvert.SerializeObject(o1, Formatting.Indented);

File.WriteAllText(gltf, res);
var p = 0;