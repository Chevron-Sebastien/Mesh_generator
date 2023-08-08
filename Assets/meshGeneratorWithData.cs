using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class meshGeneratorWithData : MonoBehaviour
{

    Mesh mesh;

    public int xSize = 1145;  //nbre de points
    public int ySize = 2152;

    Vector3[] vertices;
    int[] triangles;

    Color[] meshColors;
    public Gradient gradient;

    public float minTerrainHeight = 500;
    public float maxTerrainHeight = 135; //123 pas mal

    private string pathData = "Zone_Matrix_Maj.mtx";


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Premier message");
        if (!File.Exists(pathData))
        {
            Debug.Log("Le fichier n'existe pas");
        }
        else
        {
            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            GetComponent<MeshFilter>().mesh = mesh;

            CreateShape();
            UpdateMesh();
        }
    }




    void CreateShape()
    {
        int longueurFile = File.ReadLines(pathData).Count();
        string[] arrayMatrix = File.ReadAllLines(pathData);

        string[] ligne;
        float z = 0;


        this.vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        int i = 0;
        int compteur = 0;
        for (int y = 0; y < this.ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                ligne = arrayMatrix[compteur * xSize + x].Split(" ");
                z = float.Parse(ligne[1].Replace(".", ","));
                if (z == 0) { z = 150; }            //Debug d'un Z = 0 qqpart dans la matrice
                vertices[i] = new Vector3(x, z, y);

                //if (z > maxTerrainHeight){maxTerrainHeight = z;}
                if (z < minTerrainHeight)
                {
                    minTerrainHeight = z;
                }
                i++;
            }
            compteur++;
            Debug.Log(z);
        }


        triangles = new int[xSize * ySize * 6];

            int triss = 0;
            int vert = 0;

            for (int y = 0; y < ySize-1; y++)
            {
                for (int x = 0; x < xSize-1; x++)   //xSize-1
                {
                    triangles[0 + triss] = vert + 0;
                    triangles[1 + triss] = vert + xSize;
                    triangles[2 + triss] = vert + 1;
                    triangles[3 + triss] = vert + 1;
                    triangles[4 + triss] = vert + xSize;
                    triangles[5 + triss] = vert + xSize +1;

                    vert++;
                    triss += 6;
                }
                vert++; //coupe les colonne correctement, // compteur qui s'incrémente de y
            }

        Debug.Log("Valeur Zmax = " + this.maxTerrainHeight + "    & valeur Zmin = " + this.minTerrainHeight);

        meshColors = new Color[vertices.Length];
        i = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(this.minTerrainHeight, this.maxTerrainHeight, vertices[i].y);
                meshColors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }



    // Update is called once per frame
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = meshColors;
        mesh.RecalculateNormals();
    }
}
