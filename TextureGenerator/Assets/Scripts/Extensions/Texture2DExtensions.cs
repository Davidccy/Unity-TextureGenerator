using UnityEngine;

public static class Texture2DExtension {
    public static Texture2D Decompress(this Texture2D source) {
        // NOTE:
        // https://stackoverflow.com/questions/51315918/how-to-encodetopng-compressed-textures-in-unity

        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height);

        // Execute these steps before 'Graphics.Blit' to prevent black screen
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        // Execute these steps before 'Graphics.Blit' to prevent black screen

        Graphics.Blit(source, renderTex);

        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();

        // Revert settings
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        return readableText;
    }
}
