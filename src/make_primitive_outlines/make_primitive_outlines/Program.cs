var model = SharpGLTF.Schema2.ModelRoot.Load("BoxWithPrimitiveOutline.gltf");

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

File.WriteAllBytes("outlines1.bin", quadBytes.ToArray());