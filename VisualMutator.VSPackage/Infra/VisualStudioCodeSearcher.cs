namespace PiotrTrzpil.VisualMutator_VSPackage.Infra
{
    using EnvDTE;
    using EnvDTE80;

    public class VisualStudioCodeSearcher
    {

        public CodeFunction GetMethodAtCaret(DTE2 dte)
        {
            TextDocument objTextDocument = (TextDocument)dte.ActiveDocument.Object();
            var objCursorTextPoint = objTextDocument.Selection.ActivePoint;
            if (objCursorTextPoint != null)
            {

                CodeFunction methodElement = GetCodeElementAtTextPoint(vsCMElement.vsCMElementFunction,
                    dte.ActiveDocument.ProjectItem.FileCodeModel.CodeElements, objCursorTextPoint) as CodeFunction;
                return methodElement;
            }
            return null;
        }



        private CodeElement GetCodeElementAtTextPoint(vsCMElement eRequestedCodeElementKind,
            CodeElements colCodeElements, TextPoint objTextPoint)
        {

            //  CodeElement objCodeElement = default(CodeElement);
            CodeElement objResultCodeElement = default(CodeElement);
            CodeElements colCodeElementMembers = default(CodeElements);
            CodeElement objMemberCodeElement = default(CodeElement);


            if ((colCodeElements != null))
            {

                foreach (CodeElement objCodeElement in colCodeElements)
                {

                    if (objCodeElement.StartPoint.GreaterThan(objTextPoint))
                    {
                        // The code element starts beyond the point


                    }
                    else if (objCodeElement.EndPoint.LessThan(objTextPoint))
                    {
                        // The code element ends before the point

                        // The code element contains the point
                    }
                    else
                    {

                        if (objCodeElement.Kind == eRequestedCodeElementKind)
                        {
                            // Found
                            objResultCodeElement = objCodeElement;
                        }

                        // We enter in recursion, just in case there is an inner code element that also 
                        // satisfies the conditions, for example, if we are searching a namespace or a class
                        colCodeElementMembers = GetCodeElementMembers(objCodeElement);

                        objMemberCodeElement = GetCodeElementAtTextPoint(eRequestedCodeElementKind, colCodeElementMembers, objTextPoint);

                        if ((objMemberCodeElement != null))
                        {
                            // A nested code element also satisfies the conditions
                            objResultCodeElement = objMemberCodeElement;
                        }

                        break; // TODO: might not be correct. Was : Exit For

                    }

                }

            }

            return objResultCodeElement;

        }
        private EnvDTE.CodeElements GetCodeElementMembers(CodeElement objCodeElement)
        {

            EnvDTE.CodeElements colCodeElements = default(EnvDTE.CodeElements);


            if (objCodeElement is EnvDTE.CodeNamespace)
            {
                colCodeElements = ((EnvDTE.CodeNamespace)objCodeElement).Members;


            }
            else if (objCodeElement is EnvDTE.CodeType)
            {
                colCodeElements = ((EnvDTE.CodeType)objCodeElement).Members;


            }
            else if (objCodeElement is EnvDTE.CodeFunction)
            {
                colCodeElements = ((EnvDTE.CodeFunction)objCodeElement).Parameters;

            }

            return colCodeElements;

        } 
    }
}