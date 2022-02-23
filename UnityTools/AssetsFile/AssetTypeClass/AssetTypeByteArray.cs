namespace UnityTools
{
    public struct AssetTypeByteArray
    {
        public uint size;
        public byte[] data;

        public AssetTypeByteArray(byte[] data)
        {
            size = (uint)data.Length;
            this.data = data;
        }
    }
}
