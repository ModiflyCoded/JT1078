namespace JT1078.Protocol.Audio
{
    public interface IAudioEncoder
    {
        byte[] Encode(byte[] pcmData);
    }
}
