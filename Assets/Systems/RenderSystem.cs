using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RenderSystem : SystemBase
{
    bool isRendererOn = true;
    Material material;
    List<Vector3> verts;
    List<int> tris;
    List<Vector2> UVs;
    Mesh cubeMesh;
    NativeList<Matrix4x4> matrices;
    protected override void OnCreate()
    {
        verts = new List<Vector3>();
        tris = new List<int>();
        UVs = new List<Vector2>();
        material = new Material(Shader.Find("Standard"));
        material.enableInstancing = true;
        int vertexIndex = 0;
        for (int p = 0; p < 6; p++)
        {
            verts.Add(VoxelData.voxelVerts [VoxelData.voxelTris[p, 0]]);
            verts.Add(VoxelData.voxelVerts [VoxelData.voxelTris[p, 1]]);
            verts.Add(VoxelData.voxelVerts [VoxelData.voxelTris[p, 2]]);
            verts.Add(VoxelData.voxelVerts [VoxelData.voxelTris[p, 3]]);
            UVs.Add(new Vector2(0, 0));
            UVs.Add(new Vector2(0, 1));
            UVs.Add(new Vector2(1, 0));
            UVs.Add(new Vector2(1, 1));
            tris.Add(vertexIndex);
            tris.Add(vertexIndex+1);
            tris.Add(vertexIndex+2);
            tris.Add(vertexIndex+2);
            tris.Add(vertexIndex+1);
            tris.Add(vertexIndex+3);
            vertexIndex += 4;
        }
        cubeMesh = new Mesh();
        cubeMesh.vertices = verts.ToArray();
        cubeMesh.triangles = tris.ToArray();
        cubeMesh.uv = UVs.ToArray();

        cubeMesh.RecalculateNormals();
    }
    protected override void OnUpdate()
    {
        // if(isRendererOn)
        Entities.ForEach((in DynamicBuffer<PlayerBoard> board, in DynamicBuffer<PlayerPiece> piece, in Translation transform) => {
            matrices = new NativeList<Matrix4x4>(Allocator.Temp);
            for (int i = 0; i < board.Length; i++)
            {
                bool isItPlayerMino = false;
                for (int j = 0; j < piece.Length; j++)
                {
                    isItPlayerMino = math.all(new int2(i%10, i/10) == piece[j].value);
                    if (isItPlayerMino) break;
                }
                if(board[i].value < 128 || isItPlayerMino)matrices.Add(Matrix4x4.Translate(transform.Value + new float3(i%10, math.floor(i/10), 0f)));
            }
            Graphics.DrawMeshInstanced(cubeMesh, 0, material, matrices.ToArray());
            matrices.Dispose();
        }).WithoutBurst().Run();

        // throw new System.NotImplementedException();
    }
}