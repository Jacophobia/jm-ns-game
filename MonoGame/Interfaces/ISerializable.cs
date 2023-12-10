using System.IO;

namespace MonoGame.Interfaces;

public interface ISerializable
{
    public int Load(byte[] data, int offset);
    public void Serialize(BinaryWriter writer);
}