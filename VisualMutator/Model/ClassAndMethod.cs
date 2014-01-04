namespace VisualMutator.Model
{
    public class ClassAndMethod
    {
        public ClassAndMethod(string s, string value)
        {
            ClassName = s;
            MethodName = value;
        }

        public ClassAndMethod()
        {
           
        }

        public string MethodName { get; set; } 
        public string ClassName { get; set; } 
    }
}