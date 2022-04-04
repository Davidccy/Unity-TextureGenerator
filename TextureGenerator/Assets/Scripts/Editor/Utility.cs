using UnityEngine;

public static class Utility {
    #region Common Definitions
    public static string DEFAULT_OUTPUT_PATH = "Assets/Textures/Outputs";

    public static string DEFAULT_FILE_NAME_CIRCLE               = "NewCircle";
    public static string DEFAULT_FILE_NAME_COLOR_REPLACING      = "NewColorReplacing";
    public static string DEFAULT_FILE_NAME_GRID                 = "NewGrid";
    public static string DEFAULT_FILE_NAME_RECTANGLE            = "NewRectangle";    
    public static string DEFAULT_FILE_NAME_RING                 = "NewRing";
    public static string DEFAULT_FILE_NAME_TRIANGLE             = "NewTriangle";
    #endregion

    #region APIs
    public static Texture2D ConvertToReadableAndDecompressed(Texture2D tex) {
        // NOTE:
        // Following Texture2D functions can not be executed if texture is not readable
        // EncodeToPNG()
        // GetPixel()

        if (tex.isReadable && !IsCompressedFormat(tex)) {
            return tex;
        }

        Texture2D newTexture = tex.Decompress();

        return newTexture;
    }

    public static bool IsCompressedFormat(Texture2D tex) {
        if (tex == null) {
            return false;
        }

        switch (tex.format) {
            case TextureFormat.DXT1:
            case TextureFormat.DXT5:
            case TextureFormat.BC6H:
            case TextureFormat.BC7:
            case TextureFormat.BC4:
            case TextureFormat.BC5:
            case TextureFormat.DXT1Crunched:
            case TextureFormat.DXT5Crunched:
            case TextureFormat.PVRTC_RGB2:
            case TextureFormat.PVRTC_RGBA2:
            case TextureFormat.PVRTC_RGB4:
            case TextureFormat.PVRTC_RGBA4:
            case TextureFormat.ETC_RGB4:
            case TextureFormat.EAC_R:
            case TextureFormat.EAC_R_SIGNED:
            case TextureFormat.EAC_RG:
            case TextureFormat.EAC_RG_SIGNED:
            case TextureFormat.ETC2_RGB:
            case TextureFormat.ETC2_RGBA8:
            case TextureFormat.ASTC_4x4:
            //case TextureFormat.ASTC_RGB_4x4:
            case TextureFormat.ASTC_5x5:
            //case TextureFormat.ASTC_RGB_5x5:
            case TextureFormat.ASTC_6x6:
            //case TextureFormat.ASTC_RGB_6x6:
            case TextureFormat.ASTC_8x8:
            //case TextureFormat.ASTC_RGB_8x8:
            case TextureFormat.ASTC_10x10:
            //case TextureFormat.ASTC_RGB_10x10:
            case TextureFormat.ASTC_12x12:
            //case TextureFormat.ASTC_RGB_12x12:
            case TextureFormat.ASTC_RGBA_4x4:
            case TextureFormat.ASTC_RGBA_5x5:
            case TextureFormat.ASTC_RGBA_6x6:
            case TextureFormat.ASTC_RGBA_8x8:
            case TextureFormat.ASTC_RGBA_10x10:
            case TextureFormat.ASTC_RGBA_12x12:
            //case TextureFormat.ETC_RGB4_3DS:
            //case TextureFormat.ETC_RGBA8_3DS:
            case TextureFormat.ETC_RGB4Crunched:
            case TextureFormat.ETC2_RGBA8Crunched:
            case TextureFormat.ASTC_HDR_4x4:
            case TextureFormat.ASTC_HDR_5x5:
            case TextureFormat.ASTC_HDR_6x6:
            case TextureFormat.ASTC_HDR_8x8:
            case TextureFormat.ASTC_HDR_10x10:
            case TextureFormat.ASTC_HDR_12x12:
                return true;
        }

        return false;
    }
    #endregion
}
