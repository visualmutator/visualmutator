namespace CommonUtilityInfrastructure
{
    public static class ExtUnits
    {
        public static ByteUnit KiB(this int value)
        {
            return new ByteUnit(value*1024);
        }
        public static ByteUnit MiB(this int value)
        {
            return new ByteUnit(value*1024*1024);
        }
    }

    public struct ByteUnit
    {
        private int value;

        public ByteUnit(int value)
            : this()
        {
            this.value = value;
        }

        public int ToBytes()
        {
            return value;
        }
        public int ToKilobytes()
        {
            return value/1024;
        }
        public int ToMegabytes()
        {
            return value / 1024 / 1024;
        }
    }
}