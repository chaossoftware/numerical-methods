namespace ChaosSoft.Core
{
    public static class ArrayUtil
    {
        public static double[] GenerateArray(int length, double start, double step)
        {
            double[] array = new double[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = start + step * i;
            }

            return array;
        }
    }

}
