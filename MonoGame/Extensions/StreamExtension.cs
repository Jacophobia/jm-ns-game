using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace MonoGame.Extensions;

internal static class StreamExtensions
    {
        internal static void WriteRectangle(this BinaryWriter writer, Rectangle value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Width);
            writer.Write(value.Height);
        }

        internal static Rectangle ReadRectangle(this BinaryReader reader)
        {
            var x = reader.ReadInt32();
            var y = reader.ReadInt32();
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            return new Rectangle(x, y, width, height);
        }

        internal static void WriteColor(this BinaryWriter writer, Color value)
        {
            writer.Write(value.PackedValue); // Storing color as a single integer
        }

        internal static Color ReadColor(this BinaryReader reader)
        {
            var packedValue = reader.ReadUInt32();
            return new Color(packedValue);
        }

        internal static void WriteFloat(this BinaryWriter writer, float value)
        {
            writer.Write(value);
        }

        internal static float ReadFloat(this BinaryReader reader)
        {
            return reader.ReadSingle();
        }

        internal static void WriteVector2(this BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
        }

        internal static Vector2 ReadVector2(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        internal static void WriteInt(this BinaryWriter writer, int value)
        {
            writer.Write(value);
        }

        internal static int ReadInt(this BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        internal static void WriteString(this BinaryWriter writer, string value)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(value);
            writer.Write(bytes.Length); // Write the length of the string
            writer.Write(bytes);
        }

        internal static string ReadUtf8String(this BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
            return System.Text.Encoding.Default.GetString(bytes);
        }
    }