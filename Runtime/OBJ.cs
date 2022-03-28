using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeExport
{
    public class OBJ : Exporter
    {
        public OBJ(MeshFilter filter): base(filter) {}

        override public string Export()
        {
            MeshFilter[] meshFilters = filter.GetComponentsInChildren<MeshFilter>();
            string data = "";

            // Name the exported file
            data += $"# ==========\n";
            data += $"# {this.filter.gameObject.name}\n";
            data += $"# ==========\n\n\n";

            // Keep track of offsets
            int vertexIndexOffset = 0;
            int normalIndexOffset = 0;
            int uvIndexOffset = 0;

            // Create OBJ data for each mesh that's a child of this mesh
            foreach (var filter in meshFilters)
            {
                Mesh mesh = filter.mesh;
                Material[] materials = filter.GetComponent<Renderer>().sharedMaterials;

                // Create Object header
                data += $"g {filter.name}\n";

                // Add Verticies
                foreach (var vert in mesh.vertices)
                {
                    Vector3 tvert = filter.transform.TransformPoint(vert);
                    data += $"v {-tvert.x} {tvert.y} {tvert.z}\n";
                }
                data += "\n";

                // Add Normals
                foreach (var norm in mesh.normals)
                {
                    Vector3 tnorm = filter.transform.TransformPoint(norm);
                    data += $"vn {-tnorm.x} {tnorm.y} {tnorm.z}\n";
                }
                data += "\n";

                // Add UVs
                foreach (var uv in mesh.uv)
                {
                    data += $"vt {uv.x} {uv.y}\n";
                }
                data += "\n";

                // Add Material / Triangles
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    // Material
                    data += $"usemtl {materials[i].name}\n";
                    data += $"usemap {materials[i].name}\n";
                    // Triangles
                    int[] triangles = mesh.GetTriangles(i);
                    for (int t = 0; t < triangles.Length; t += 3)
                        data += string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                            triangles[t] + 1 + vertexIndexOffset,
                            triangles[t + 1] + 1 + normalIndexOffset,
                            triangles[t + 2] + 1 + uvIndexOffset);
                }
                data += "\n\n";

                // Update offsets
                vertexIndexOffset += mesh.vertices.Length;
                normalIndexOffset += mesh.normals.Length;
                uvIndexOffset += mesh.uv.Length;
            }

            return data;
        }
    }
}
