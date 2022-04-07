using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeExport
{
    public class OBJ : Exporter
    {
        public OBJ(GameObject target): base(target) {}

        override public Dictionary<string, string> Export()
        {
            MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
            var ret = new Dictionary<string, string>(); // filename: data
            string data = "";

            // Name the exported file
            data += $"# ==========\n";
            data += $"# {this.target.name}\n";
            data += $"# ==========\n\n\n";
            data += $"mtlib {this.target.name + ".mtl"}\n\n";

            // Keep track of offsets
            int vertexIndexOffset = 0;
            int normalIndexOffset = 0;
            int uvIndexOffset = 0;
            string materialData = "";

            // Create OBJ data for each mesh that's a child of this mesh
            foreach (var filter in meshFilters)
            {
                Mesh mesh = filter.mesh;

                // Create Object header
                data += $"g {filter.name}\n";

                // Add Materials
                Material[] materials = filter.GetComponent<Renderer>().sharedMaterials;
                materialData += this.GenerateMaterialData(materials);

                // Add Verticies
                foreach (var vert in mesh.vertices)
                {
                    Vector3 globalVert = this.target.transform.InverseTransformPoint(filter.transform.TransformPoint(vert));
                    data += $"v {-globalVert.x} {globalVert.y} {globalVert.z}\n";
                }
                data += "\n";

                // Add Normals
                foreach (var norm in mesh.normals)
                {
                    Vector3 globalNorm = this.target.transform.InverseTransformDirection(filter.transform.TransformDirection(norm));
                    data += $"vn {-globalNorm.x} {globalNorm.y} {globalNorm.z}\n";
                }
                data += "\n";

                // Add UVs
                foreach (var uv in mesh.uv)
                {
                    data += $"vt {uv.x} {uv.y}\n";
                }
                data += "\n";

                // Add Triangles
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    data += $"usemtl {materials[i].name}\n";
                    data += $"usemap {materials[i].name}\n\n";

                    int[] triangles = mesh.GetTriangles(i);
                    for (int t = 0; t < triangles.Length; t += 3)
                        data += string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                            triangles[t + 0] + 1 + vertexIndexOffset,
                            triangles[t + 1] + 1 + normalIndexOffset,
                            triangles[t + 2] + 1 + uvIndexOffset);
                }
                data += "\n\n";

                // Update offsets
                vertexIndexOffset += mesh.vertices.Length;
                normalIndexOffset += mesh.normals.Length;
                uvIndexOffset += mesh.uv.Length;
            }

            // Add Materials
            data += materialData;

            ret.Add(this.target.name + ".obj", data);
            ret.Add(this.target.name + ".mtl", materialData);
            return ret;
        }

        private string GenerateMaterialData(Material[] mats)
        {
            string data = "";

            foreach (Material mat in mats)
            {
                data += $"newmtl {mat.name}\n";

                //data += $"Ka {mat.color.r} {mat.color.g} {mat.color.b}\n"; // ambient
                data += $"Kd {mat.color.r} {mat.color.g} {mat.color.b}\n"; // diffuse
                //data += $"Ks {mat.color.r} {mat.color.g} {mat.color.b}\n"; // specular
                data += $"Tr {mat.color.a}\n"; // transparency
                data += "\n\n";
            }
            return data;
        }
    }
}
