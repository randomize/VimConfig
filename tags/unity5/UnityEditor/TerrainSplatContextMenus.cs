namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TerrainSplatContextMenus
    {
        [UnityEditor.MenuItem("CONTEXT/TerrainEngineSplats/Add Texture...")]
        internal static void AddSplat(MenuCommand item)
        {
            TerrainSplatEditor.ShowTerrainSplatEditor("Add Terrain Texture", "Add", (Terrain) item.context, -1);
        }

        [UnityEditor.MenuItem("CONTEXT/TerrainEngineSplats/Edit Texture...")]
        internal static void EditSplat(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            string title = "Edit Terrain Texture";
            switch (context.materialType)
            {
                case Terrain.MaterialType.BuiltInStandard:
                    title = title + " (Standard)";
                    break;

                case Terrain.MaterialType.BuiltInLegacyDiffuse:
                    title = title + " (Diffuse)";
                    break;

                case Terrain.MaterialType.BuiltInLegacySpecular:
                    title = title + " (Specular)";
                    break;

                case Terrain.MaterialType.Custom:
                    title = title + " (Custom)";
                    break;
            }
            TerrainSplatEditor.ShowTerrainSplatEditor(title, "Apply", (Terrain) item.context, item.userData);
        }

        [UnityEditor.MenuItem("CONTEXT/TerrainEngineSplats/Edit Texture...", true)]
        internal static bool EditSplatCheck(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            return ((item.userData >= 0) && (item.userData < context.terrainData.splatPrototypes.Length));
        }

        [UnityEditor.MenuItem("CONTEXT/TerrainEngineSplats/Remove Texture")]
        internal static void RemoveSplat(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            TerrainEditorUtility.RemoveSplatTexture(context.terrainData, item.userData);
        }

        [UnityEditor.MenuItem("CONTEXT/TerrainEngineSplats/Remove Texture", true)]
        internal static bool RemoveSplatCheck(MenuCommand item)
        {
            Terrain context = (Terrain) item.context;
            return ((item.userData >= 0) && (item.userData < context.terrainData.splatPrototypes.Length));
        }
    }
}

