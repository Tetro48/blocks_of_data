using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class RenderSystem : SystemBase
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
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
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
        BlobAssetReference<PieceBlob> collisionRef = PlayerSystem.pieceCollisionReference;
        // if(isRendererOn)
        Entities.ForEach((in PlayerComponent player, in DynamicBuffer<PlayerBoard> board, in DynamicBuffer<PlayerBag> bag, in Translation transform) => {
            var matrices = new NativeList<Matrix4x4>(Allocator.Temp);
            for (int i = 0; i < board.Length; i++)
            {
                if(board[i].value < 128)matrices.Add(Matrix4x4.Translate(transform.Value + new float3(i%10, math.floor(i/10), 0f)));
            }
            if (player.pieceSpawned)
            for (int i = 0; i < player.minos; i++)
            {
                matrices.Add(Matrix4x4.Translate(transform.Value + new float3(player.piecePos + collisionRef.Value.array[player.minoIndex+i], 0f)));
            }
            for (int i = 0; i < math.min(bag.Length, 6); i++)
            {
                for (int j = 0; j < bag[i].value.y; j++)
                {
                    matrices.Add(Matrix4x4.Translate(transform.Value + new float3(new int2(14, 21 - (3 * i)) + collisionRef.Value.array[(bag[i].value.x<<4)+j], 0f)));
                }
            }
            for (int j = 0; j < player.holdMinos; j++)
            {
                matrices.Add(Matrix4x4.Translate(transform.Value + new float3(new int2(-5, 21) + collisionRef.Value.array[(player.holdMinoIndex)+j], 0f)));
            }
            Graphics.DrawMeshInstanced(cubeMesh, 0, material, matrices.ToArray());
            matrices.Dispose();
        }).WithoutBurst().Run();

        // throw new System.NotImplementedException();
    }
}