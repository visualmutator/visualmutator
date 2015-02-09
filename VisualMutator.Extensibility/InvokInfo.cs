namespace VisualMutator.Extensibility
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Cci.MutableCodeModel;

    public class InvokInfo
    {
        private readonly int _levelCount;
        private readonly object _parentObject;
        private readonly string _methodType;
        private readonly object _obj;

        public InvokInfo(DebugCodeTraverser traverser, string methodType, object obj)
        {
            _levelCount = traverser.LevelCount;
            _parentObject = traverser.CurrentObject;
            _methodType = methodType;
            _obj = obj;
        }

        public object Obj
        {
            get { return _obj; }
        }

        public string MethodType
        {
            get { return _methodType; }
        }

        public int LevelCount
        {
            get { return _levelCount; }
        }
        public override string ToString()
        {
            string indent = new string(' ', LevelCount*3);
            string methodParamName = MethodType;


            string prefix = "";
                
            if (_parentObject != null)
            {
                var prop = _parentObject.GetType().GetProperties().FirstOrDefault(p =>
                {
                    try
                    {
                        return _obj.Equals(p.GetValue(_parentObject, null));
                    }
                    catch (TargetInvocationException )
                    {
                        prefix = "exc on: "+p.Name ;
                        return false;
                    }
                });
                if (prop != null)
                {
                    prefix = prop.Name;
                }
            }
            else
            {
                prefix = "parentnull";
            }
            string body = ObjToString();

            string ret = indent+prefix + ": ("+methodParamName+") - "+body;
           // Console.WriteLine(ret);
            return ret;
        }
        public string ObjToString()
        {
                

            var boundExpression = _obj as BoundExpression;
            if(boundExpression != null)
            {
                return "BoundExpression {Instance=" + boundExpression.Instance +
                       ", Definition=" + boundExpression.Definition +
                       ", Type=" + boundExpression.Type + "}";
            }
            else
            {
                var astFormatter = new AstFormatter();
                //var objToString = astFormatter.Format(_obj);
                string objToString = _obj.ToString();
                if (objToString != _obj.GetType().FullName)
                {
                    return objToString;
                }
                else
                {
                    return _obj.GetType().Name;
                }

            }
				
                
        }

        public string ToStringBasicVisit()
        {
            string indent = new string(' ', LevelCount * 3);
            string methodParamName = MethodType;
            return indent + methodParamName + " -- " + ObjToString();
        }
    }
}