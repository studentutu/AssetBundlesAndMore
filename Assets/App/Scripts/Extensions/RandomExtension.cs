
public static class RandomExtension
{
    public static int[] ShuffledArray(int length)
    {
        int[] shuffledIndices = new int[length];
        for (int i = 0; i < shuffledIndices.Length; i++)
        {
            shuffledIndices[i] = i;
        }
        int temportal;
        int random;
        for (int i = 0; i < length / 2; i++)
        {
            random = UnityEngine.Random.Range(0, length);
            temportal = shuffledIndices[random];
            shuffledIndices[random] = shuffledIndices[i];
            shuffledIndices[i] = temportal;
        }
        return shuffledIndices;
    }
}
