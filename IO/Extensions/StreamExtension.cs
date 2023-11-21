using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace IO.Extensions;

public static class StreamExtensions
{
    public static void WriteRectangle(this Stream stream, Rectangle value)
    {
        stream.WriteInt(value.X);
        stream.WriteInt(value.Y);
        stream.WriteInt(value.Width);
        stream.WriteInt(value.Height);
    }

    public static Rectangle ReadRectangle(this Stream stream)
    {
        var x = stream.ReadInt();
        var y = stream.ReadInt();
        var width = stream.ReadInt();
        var height = stream.ReadInt();
        return new Rectangle(x, y, width, height);
    }

    public static void WriteColor(this Stream stream, Color value)
    {
        stream.WriteByte(value.R);
        stream.WriteByte(value.G);
        stream.WriteByte(value.B);
        stream.WriteByte(value.A);
    }

    public static Color ReadColor(this Stream stream)
    {
        var r = (byte)stream.ReadByte();
        var g = (byte)stream.ReadByte();
        var b = (byte)stream.ReadByte();
        var a = (byte)stream.ReadByte();
        return new Color(r, g, b, a);
    }

    public static void WriteFloat(this Stream stream, float value)
    {
        var bytes = BitConverter.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    public static float ReadFloat(this Stream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        return BitConverter.ToSingle(buffer, 0);
    }

    public static void WriteVector2(this Stream stream, Vector2 value)
    {
        stream.WriteFloat(value.X);
        stream.WriteFloat(value.Y);
    }

    public static Vector2 ReadVector2(this Stream stream)
    {
        var x = stream.ReadFloat();
        var y = stream.ReadFloat();
        return new Vector2(x, y);
    }

    public static void WriteInt(this Stream stream, int value)
    {
        var bytes = BitConverter.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    public static int ReadInt(this Stream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        return BitConverter.ToInt32(buffer, 0);
    }

    public static void WriteString(this Stream stream, string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        stream.WriteInt(bytes.Length); // Write the length of the string
        stream.Write(bytes, 0, bytes.Length);
    }

    public static string ReadString(this Stream stream)
    {
        var length = stream.ReadInt();
        var bytes = new byte[length];
        stream.Read(bytes, 0, length);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}