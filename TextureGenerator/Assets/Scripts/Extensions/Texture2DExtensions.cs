using UnityEngine;
using UnityEditor;

public static class Texture2DExtension {
    public static Texture2D Clone(this Texture2D source) {
        // NOTE:
        // https://forum.unity.com/threads/how-to-change-png-import-settings-via-script.734834/
        string path = AssetDatabase.GetAssetPath(source);

        TextureImporter importer = (TextureImporter) TextureImporter.GetAtPath(path);
        bool oriIsReadable = importer.isReadable;
        TextureImporterCompression oriCompression = importer.textureCompression;

        importer.isReadable = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        int width = source.width;
        int height = source.height;
        Texture2D t = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                Color c = source.GetPixel(w, h);
                t.SetPixel(w, h, c);
            }
        }

        byte[] bytes = t.EncodeToPNG();
        Texture2D newTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
        newTexture.LoadImage(bytes);

        // Revert settings
        importer.isReadable = oriIsReadable;
        importer.textureCompression = oriCompression;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        return newTexture;
    }

    //public static Texture2D Decompress(this Texture2D source) {
    //    // NOTE:
    //    // https://stackoverflow.com/questions/51315918/how-to-encodetopng-compressed-textures-in-unity

    //    RenderTexture renderTex = RenderTexture.GetTemporary(
    //                source.width,
    //                source.height);

    //    // Execute these steps before 'Graphics.Blit' to prevent black screen
    //    RenderTexture previous = RenderTexture.active;
    //    RenderTexture.active = renderTex;
    //    // Execute these steps before 'Graphics.Blit' to prevent black screen

    //    Graphics.Blit(source, renderTex);

    //    Texture2D readableText = new Texture2D(source.width, source.height);
    //    readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
    //    readableText.Apply();

    //    // Revert settings
    //    RenderTexture.active = previous;
    //    RenderTexture.ReleaseTemporary(renderTex);

    //    return readableText;
    //}
}
