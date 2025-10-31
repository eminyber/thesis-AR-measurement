using System.Collections.Generic;

using UnityEngine;


namespace ARMeasurementApp.Scripts.Controllers
{
    public class MeshFilterController
    {
        public List<Vector3> GetMeshesVerticePositions(List<MeshFilter> meshFilters) 
        {
            if (meshFilters == null) return new List<Vector3>();

            var verticePositions = new List<Vector3>();
            foreach(MeshFilter meshFilter in meshFilters)
            {
                verticePositions.AddRange(meshFilter.mesh.vertices);
            }

            return verticePositions;
        }
    }
}
