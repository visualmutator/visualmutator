namespace TestGround
{
    using System;

    public class NewTestGround
    {
        public static int FailOnZero(int x)
        {
            if (x == 0)
            {
                throw new InvalidOperationException("VisualMutator: x == 0");
            }
            return x;
        }
    }

    public class TestGround
    {
        public class TestGroundInner
        {
            public int SomethingElse(int z)
            {
                return z - 3;
            }
        }

        private int y = 5;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int Something(int x)
         {
             if (x > 10)
             {
                 return x + y;
             }
             else
             {
                 return y + 5;
             }
         }
        public int Something2(int x)
        {
            return -x;
        }
    }
}