﻿using make_primitive_outlines;
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
var pivot1 = new NodeBuilder("Cube1").WithLocalTranslation(new Vector3(-5, 0, 0));
var scene = new SceneBuilder();
scene.AddRigidMesh(mesh, pivot1);
var model = scene.ToGltf2();
// model.SaveGLB("test.glb");

// var model = SharpGLTF.Schema2.ModelRoot.Load("BoxWithPrimitiveOutline.gltf");

var originalIndices = model.LogicalMeshes[0].Primitives[0].IndexAccessor.AsIndicesArray().ToArray();
var INDICES_PER_QUAD = 6;
var quads = originalIndices.Count() / INDICES_PER_QUAD;

var quadBytes = new List<byte>();

for (var i=0;i<quads; i++)
{
    var start_index = INDICES_PER_QUAD * i;
    var end_index = start_index + INDICES_PER_QUAD;

    var indices = originalIndices[start_index..end_index];
    var a = indices[0];
    var b = indices[1];
    var c = indices[2];
    var d = indices[4];

    var quad_edges = new uint[] {
            a, d,
            d, b,
            b, c,
            c, a
    };

    foreach(var item in quad_edges)
    {
        var bytes = BitConverter.GetBytes((UInt16)item).ToList();
        quadBytes.AddRange(bytes);
    }
}

model.UseBuffer(quadBytes.ToArray());
//model.MergeBuffers();
// var settings = new WriteSettings { JsonIndented = true };
// model.SaveGLTF("test1.gltf", settings);
model.SaveGLB("test1.glb");

// File.WriteAllBytes("outlines1.bin", quadBytes.ToArray());