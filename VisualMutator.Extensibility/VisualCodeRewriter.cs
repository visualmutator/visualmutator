namespace VisualMutator.Extensibility
{
    using System.Collections.Generic;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using MethodReference = Microsoft.Cci.MutableCodeModel.MethodReference;

    public class VisualCodeRewriter : CodeRewriter
    {
        private readonly List<object> _mutationTargets;
        private readonly IList<TypeIdentifier> _allowedTypes;
       
        private OperatorCodeRewriter rewriter;

        public VisualCodeRewriter(IMetadataHost host, List<object> mutationTargets, IList<TypeIdentifier> allowedTypes, OperatorCodeRewriter rewriter,
            bool copyAndRewriteImmutableReferences = false)
            : base(host, copyAndRewriteImmutableReferences)
        {
            _mutationTargets = mutationTargets;
            _allowedTypes = allowedTypes;
            this.rewriter = rewriter;
        }

        private bool Process(object obj)
        {
       
            return _mutationTargets.Contains(obj);
        }

        public override void RewriteChildren(NamespaceTypeDefinition namespaceTypeDefinition)
        {
            if (_allowedTypes.Contains(new TypeIdentifier(namespaceTypeDefinition)))
            {
                base.RewriteChildren(namespaceTypeDefinition);
            }
        }

        public override IExpression Rewrite(IAddition addition)
        {if(Process(addition)){var additionNew = rewriter.Rewrite(addition); return base.Rewrite(additionNew);}
            return base.Rewrite(addition);
        }

        public override IAddressableExpression Rewrite(IAddressableExpression addressableExpression)
        {if(Process(addressableExpression)){var addressableExpressionNew = rewriter.Rewrite(addressableExpression); return base.Rewrite(addressableExpressionNew);}
            return base.Rewrite(addressableExpression);
        }

        public override IExpression Rewrite(IAddressDereference addressDereference)
        {if(Process(addressDereference)){var addressDereferenceNew = rewriter.Rewrite(addressDereference); return base.Rewrite(addressDereferenceNew);}
            return base.Rewrite(addressDereference);
        }

        public override IExpression Rewrite(IAddressOf addressOf)
        {if(Process(addressOf)){var addressOfNew = rewriter.Rewrite(addressOf); return base.Rewrite(addressOfNew);}
            return base.Rewrite(addressOf);
        }

        public override IExpression Rewrite(IAnonymousDelegate anonymousDelegate)
        {if(Process(anonymousDelegate)){var anonymousDelegateNew = rewriter.Rewrite(anonymousDelegate); return base.Rewrite(anonymousDelegateNew);}
            return base.Rewrite(anonymousDelegate);
        }

        public override IExpression Rewrite(IArrayIndexer arrayIndexer)
        {if(Process(arrayIndexer)){var arrayIndexerNew = rewriter.Rewrite(arrayIndexer); return base.Rewrite(arrayIndexerNew);}
            return base.Rewrite(arrayIndexer);
        }

        public override IStatement Rewrite(IAssertStatement assertStatement)
        {if(Process(assertStatement)){var assertStatementNew = rewriter.Rewrite(assertStatement); return base.Rewrite(assertStatementNew);}
            return base.Rewrite(assertStatement);
        }

        public override IExpression Rewrite(IAssignment assignment)
        {if(Process(assignment)){var assignmentNew = rewriter.Rewrite(assignment); return base.Rewrite(assignmentNew);}
            return base.Rewrite(assignment);
        }

        public override IStatement Rewrite(IAssumeStatement assumeStatement)
        {if(Process(assumeStatement)){var assumeStatementNew = rewriter.Rewrite(assumeStatement); return base.Rewrite(assumeStatementNew);}
            return base.Rewrite(assumeStatement);
        }

        public override IExpression Rewrite(IBinaryOperation binaryOperation)
        {if(Process(binaryOperation)){var binaryOperationNew = rewriter.Rewrite(binaryOperation); return base.Rewrite(binaryOperationNew);}
            return base.Rewrite(binaryOperation);
        }

        public override IExpression Rewrite(IBitwiseAnd bitwiseAnd)
        {if(Process(bitwiseAnd)){var bitwiseAndNew = rewriter.Rewrite(bitwiseAnd); return base.Rewrite(bitwiseAndNew);}
            return base.Rewrite(bitwiseAnd);
        }

        public override IExpression Rewrite(IBitwiseOr bitwiseOr)
        {if(Process(bitwiseOr)){var bitwiseOrNew = rewriter.Rewrite(bitwiseOr); return base.Rewrite(bitwiseOrNew);}
            return base.Rewrite(bitwiseOr);
        }

        public override IExpression Rewrite(IBlockExpression blockExpression)
        {if(Process(blockExpression)){var blockExpressionNew = rewriter.Rewrite(blockExpression); return base.Rewrite(blockExpressionNew);}
            return base.Rewrite(blockExpression);
        }

        public override IBlockStatement Rewrite(IBlockStatement block)
        {if(Process(block)){var blockNew = rewriter.Rewrite(block); return base.Rewrite(blockNew);}
            return base.Rewrite(block);
        }

        public override IExpression Rewrite(IBoundExpression boundExpression)
        {if(Process(boundExpression)){var boundExpressionNew = rewriter.Rewrite(boundExpression); return base.Rewrite(boundExpressionNew);}
            return base.Rewrite(boundExpression);
        }

        public override IStatement Rewrite(IBreakStatement breakStatement)
        {if(Process(breakStatement)){var breakStatementNew = rewriter.Rewrite(breakStatement); return base.Rewrite(breakStatementNew);}
            return base.Rewrite(breakStatement);
        }

        public override IExpression Rewrite(ICastIfPossible castIfPossible)
        {if(Process(castIfPossible)){var castIfPossibleNew = rewriter.Rewrite(castIfPossible); return base.Rewrite(castIfPossibleNew);}
            return base.Rewrite(castIfPossible);
        }

        public override ICatchClause Rewrite(ICatchClause catchClause)
        {if(Process(catchClause)){var catchClauseNew = rewriter.Rewrite(catchClause); return base.Rewrite(catchClauseNew);}
            return base.Rewrite(catchClause);
        }

        public override IExpression Rewrite(ICheckIfInstance checkIfInstance)
        {if(Process(checkIfInstance)){var checkIfInstanceNew = rewriter.Rewrite(checkIfInstance); return base.Rewrite(checkIfInstanceNew);}
            return base.Rewrite(checkIfInstance);
        }

        public override ICompileTimeConstant Rewrite(ICompileTimeConstant constant)
        {if(Process(constant)){var constantNew = rewriter.Rewrite(constant); return base.Rewrite(constantNew);}
            return base.Rewrite(constant);
        }

        public override IExpression Rewrite(IConditional conditional)
        {if(Process(conditional)){var conditionalNew = rewriter.Rewrite(conditional); return base.Rewrite(conditionalNew);}
            return base.Rewrite(conditional);
        }

        public override IStatement Rewrite(IConditionalStatement conditionalStatement)
        {if(Process(conditionalStatement)){var conditionalStatementNew = rewriter.Rewrite(conditionalStatement); return base.Rewrite(conditionalStatementNew);}
            return base.Rewrite(conditionalStatement);
        }

        public override IStatement Rewrite(IContinueStatement continueStatement)
        {if(Process(continueStatement)){var continueStatementNew = rewriter.Rewrite(continueStatement); return base.Rewrite(continueStatementNew);}
            return base.Rewrite(continueStatement);
        }

        public override IExpression Rewrite(IConversion conversion)
        {if(Process(conversion)){var conversionNew = rewriter.Rewrite(conversion); return base.Rewrite(conversionNew);}
            return base.Rewrite(conversion);
        }

        public override IStatement Rewrite(ICopyMemoryStatement copyMemoryStatement)
        {if(Process(copyMemoryStatement)){var copyMemoryStatementNew = rewriter.Rewrite(copyMemoryStatement); return base.Rewrite(copyMemoryStatementNew);}
            return base.Rewrite(copyMemoryStatement);
        }

        public override IExpression Rewrite(ICreateArray createArray)
        {if(Process(createArray)){var createArrayNew = rewriter.Rewrite(createArray); return base.Rewrite(createArrayNew);}
            return base.Rewrite(createArray);
        }

        public override IExpression Rewrite(ICreateDelegateInstance createDelegateInstance)
        {if(Process(createDelegateInstance)){var createDelegateInstanceNew = rewriter.Rewrite(createDelegateInstance); return base.Rewrite(createDelegateInstanceNew);}
            return base.Rewrite(createDelegateInstance);
        }

        public override IExpression Rewrite(ICreateObjectInstance createObjectInstance)
        {if(Process(createObjectInstance)){var createObjectInstanceNew = rewriter.Rewrite(createObjectInstance); return base.Rewrite(createObjectInstanceNew);}
            return base.Rewrite(createObjectInstance);
        }

        public override IStatement Rewrite(IDebuggerBreakStatement debuggerBreakStatement)
        {if(Process(debuggerBreakStatement)){var debuggerBreakStatementNew = rewriter.Rewrite(debuggerBreakStatement); return base.Rewrite(debuggerBreakStatementNew);}
            return base.Rewrite(debuggerBreakStatement);
        }

        public override IExpression Rewrite(IDefaultValue defaultValue)
        {if(Process(defaultValue)){var defaultValueNew = rewriter.Rewrite(defaultValue); return base.Rewrite(defaultValueNew);}
            return base.Rewrite(defaultValue);
        }

        public override IExpression Rewrite(IDivision division)
        {if(Process(division)){var divisionNew = rewriter.Rewrite(division); return base.Rewrite(divisionNew);}
            return base.Rewrite(division);
        }

        public override IStatement Rewrite(IDoUntilStatement doUntilStatement)
        {if(Process(doUntilStatement)){var doUntilStatementNew = rewriter.Rewrite(doUntilStatement); return base.Rewrite(doUntilStatementNew);}
            return base.Rewrite(doUntilStatement);
        }

        public override IExpression Rewrite(IDupValue dupValue)
        {if(Process(dupValue)){var dupValueNew = rewriter.Rewrite(dupValue); return base.Rewrite(dupValueNew);}
            return base.Rewrite(dupValue);
        }

        public override IStatement Rewrite(IEmptyStatement emptyStatement)
        {if(Process(emptyStatement)){var emptyStatementNew = rewriter.Rewrite(emptyStatement); return base.Rewrite(emptyStatementNew);}
            return base.Rewrite(emptyStatement);
        }

        public override IExpression Rewrite(IEquality equality)
        {if(Process(equality)){var equalityNew = rewriter.Rewrite(equality); return base.Rewrite(equalityNew);}
            return base.Rewrite(equality);
        }

        public override IExpression Rewrite(IExclusiveOr exclusiveOr)
        {if(Process(exclusiveOr)){var exclusiveOrNew = rewriter.Rewrite(exclusiveOr); return base.Rewrite(exclusiveOrNew);}
            return base.Rewrite(exclusiveOr);
        }

        public override IExpression Rewrite(IExpression expression)
        {if(Process(expression)){var expressionNew = rewriter.Rewrite(expression); return base.Rewrite(expressionNew);}
            return base.Rewrite(expression);
        }

        public override IStatement Rewrite(IExpressionStatement expressionStatement)
        {if(Process(expressionStatement)){var expressionStatementNew = rewriter.Rewrite(expressionStatement); return base.Rewrite(expressionStatementNew);}
            return base.Rewrite(expressionStatement);
        }

        public override IStatement Rewrite(IFillMemoryStatement fillMemoryStatement)
        {if(Process(fillMemoryStatement)){var fillMemoryStatementNew = rewriter.Rewrite(fillMemoryStatement); return base.Rewrite(fillMemoryStatementNew);}
            return base.Rewrite(fillMemoryStatement);
        }

        public override IStatement Rewrite(IForEachStatement forEachStatement)
        {if(Process(forEachStatement)){var forEachStatementNew = rewriter.Rewrite(forEachStatement); return base.Rewrite(forEachStatementNew);}
            return base.Rewrite(forEachStatement);
        }

        public override IStatement Rewrite(IForStatement forStatement)
        {if(Process(forStatement)){var forStatementNew = rewriter.Rewrite(forStatement); return base.Rewrite(forStatementNew);}
            return base.Rewrite(forStatement);
        }

        public override IExpression Rewrite(IGetTypeOfTypedReference getTypeOfTypedReference)
        {if(Process(getTypeOfTypedReference)){var getTypeOfTypedReferenceNew = rewriter.Rewrite(getTypeOfTypedReference); return base.Rewrite(getTypeOfTypedReferenceNew);}
            return base.Rewrite(getTypeOfTypedReference);
        }

        public override IExpression Rewrite(IGetValueOfTypedReference getValueOfTypedReference)
        {if(Process(getValueOfTypedReference)){var getValueOfTypedReferenceNew = rewriter.Rewrite(getValueOfTypedReference); return base.Rewrite(getValueOfTypedReferenceNew);}
            return base.Rewrite(getValueOfTypedReference);
        }

        public override IStatement Rewrite(IGotoStatement gotoStatement)
        {if(Process(gotoStatement)){var gotoStatementNew = rewriter.Rewrite(gotoStatement); return base.Rewrite(gotoStatementNew);}
            return base.Rewrite(gotoStatement);
        }

        public override IStatement Rewrite(IGotoSwitchCaseStatement gotoSwitchCaseStatement)
        {if(Process(gotoSwitchCaseStatement)){var gotoSwitchCaseStatementNew = rewriter.Rewrite(gotoSwitchCaseStatement); return base.Rewrite(gotoSwitchCaseStatementNew);}
            return base.Rewrite(gotoSwitchCaseStatement);
        }

        public override IExpression Rewrite(IGreaterThan greaterThan)
        {if(Process(greaterThan)){var greaterThanNew = rewriter.Rewrite(greaterThan); return base.Rewrite(greaterThanNew);}
            return base.Rewrite(greaterThan);
        }

        public override IExpression Rewrite(IGreaterThanOrEqual greaterThanOrEqual)
        {if(Process(greaterThanOrEqual)){var greaterThanOrEqualNew = rewriter.Rewrite(greaterThanOrEqual); return base.Rewrite(greaterThanOrEqualNew);}
            return base.Rewrite(greaterThanOrEqual);
        }

        public override IStatement Rewrite(ILabeledStatement labeledStatement)
        {if(Process(labeledStatement)){var labeledStatementNew = rewriter.Rewrite(labeledStatement); return base.Rewrite(labeledStatementNew);}
            return base.Rewrite(labeledStatement);
        }

        public override IExpression Rewrite(ILeftShift leftShift)
        {if(Process(leftShift)){var leftShiftNew = rewriter.Rewrite(leftShift); return base.Rewrite(leftShiftNew);}
            return base.Rewrite(leftShift);
        }

        public override IExpression Rewrite(ILessThan lessThan)
        {if(Process(lessThan)){var lessThanNew = rewriter.Rewrite(lessThan); return base.Rewrite(lessThanNew);}
            return base.Rewrite(lessThan);
        }

        public override IExpression Rewrite(ILessThanOrEqual lessThanOrEqual)
        {if(Process(lessThanOrEqual)){var lessThanOrEqualNew = rewriter.Rewrite(lessThanOrEqual); return base.Rewrite(lessThanOrEqualNew);}
            return base.Rewrite(lessThanOrEqual);
        }

        public override IStatement Rewrite(ILocalDeclarationStatement localDeclarationStatement)
        {if(Process(localDeclarationStatement)){var localDeclarationStatementNew = rewriter.Rewrite(localDeclarationStatement); return base.Rewrite(localDeclarationStatementNew);}
            return base.Rewrite(localDeclarationStatement);
        }

        public override IStatement Rewrite(ILockStatement lockStatement)
        {if(Process(lockStatement)){var lockStatementNew = rewriter.Rewrite(lockStatement); return base.Rewrite(lockStatementNew);}
            return base.Rewrite(lockStatement);
        }

        public override IExpression Rewrite(ILogicalNot logicalNot)
        {if(Process(logicalNot)){var logicalNotNew = rewriter.Rewrite(logicalNot); return base.Rewrite(logicalNotNew);}
            return base.Rewrite(logicalNot);
        }

        public override IExpression Rewrite(IMakeTypedReference makeTypedReference)
        {if(Process(makeTypedReference)){var makeTypedReferenceNew = rewriter.Rewrite(makeTypedReference); return base.Rewrite(makeTypedReferenceNew);}
            return base.Rewrite(makeTypedReference);
        }

        public override IMethodBody Rewrite(IMethodBody methodBody)
        {if(Process(methodBody)){var methodBodyNew = rewriter.Rewrite(methodBody); return base.Rewrite(methodBodyNew);}
            return base.Rewrite(methodBody);
        }

        public override IExpression Rewrite(IMethodCall methodCall)
        {if(Process(methodCall)){var methodCallNew = rewriter.Rewrite(methodCall); return base.Rewrite(methodCallNew);}
            return base.Rewrite(methodCall);
        }

        public override IExpression Rewrite(IModulus modulus)
        {if(Process(modulus)){var modulusNew = rewriter.Rewrite(modulus); return base.Rewrite(modulusNew);}
            return base.Rewrite(modulus);
        }

        public override IExpression Rewrite(IMultiplication multiplication)
        {if(Process(multiplication)){var multiplicationNew = rewriter.Rewrite(multiplication); return base.Rewrite(multiplicationNew);}
            return base.Rewrite(multiplication);
        }

        public override IExpression Rewrite(INamedArgument namedArgument)
        {if(Process(namedArgument)){var namedArgumentNew = rewriter.Rewrite(namedArgument); return base.Rewrite(namedArgumentNew);}
            return base.Rewrite(namedArgument);
        }

        public override IExpression Rewrite(INotEquality notEquality)
        {if(Process(notEquality)){var notEqualityNew = rewriter.Rewrite(notEquality); return base.Rewrite(notEqualityNew);}
            return base.Rewrite(notEquality);
        }

        public override IExpression Rewrite(IOldValue oldValue)
        {if(Process(oldValue)){var oldValueNew = rewriter.Rewrite(oldValue); return base.Rewrite(oldValueNew);}
            return base.Rewrite(oldValue);
        }

        public override IExpression Rewrite(IOnesComplement onesComplement)
        {if(Process(onesComplement)){var onesComplementNew = rewriter.Rewrite(onesComplement); return base.Rewrite(onesComplementNew);}
            return base.Rewrite(onesComplement);
        }

        public override IExpression Rewrite(IOutArgument outArgument)
        {if(Process(outArgument)){var outArgumentNew = rewriter.Rewrite(outArgument); return base.Rewrite(outArgumentNew);}
            return base.Rewrite(outArgument);
        }

        public override IExpression Rewrite(IPointerCall pointerCall)
        {if(Process(pointerCall)){var pointerCallNew = rewriter.Rewrite(pointerCall); return base.Rewrite(pointerCallNew);}
            return base.Rewrite(pointerCall);
        }

        public override IExpression Rewrite(IPopValue popValue)
        {if(Process(popValue)){var popValueNew = rewriter.Rewrite(popValue); return base.Rewrite(popValueNew);}
            return base.Rewrite(popValue);
        }

        public override IStatement Rewrite(IPushStatement pushStatement)
        {if(Process(pushStatement)){var pushStatementNew = rewriter.Rewrite(pushStatement); return base.Rewrite(pushStatementNew);}
            return base.Rewrite(pushStatement);
        }

        public override IExpression Rewrite(IRefArgument refArgument)
        {if(Process(refArgument)){var refArgumentNew = rewriter.Rewrite(refArgument); return base.Rewrite(refArgumentNew);}
            return base.Rewrite(refArgument);
        }

        public override IStatement Rewrite(IResourceUseStatement resourceUseStatement)
        {if(Process(resourceUseStatement)){var resourceUseStatementNew = rewriter.Rewrite(resourceUseStatement); return base.Rewrite(resourceUseStatementNew);}
            return base.Rewrite(resourceUseStatement);
        }

        public override IStatement Rewrite(IRethrowStatement rethrowStatement)
        {if(Process(rethrowStatement)){var rethrowStatementNew = rewriter.Rewrite(rethrowStatement); return base.Rewrite(rethrowStatementNew);}
            return base.Rewrite(rethrowStatement);
        }

        public override IStatement Rewrite(IReturnStatement returnStatement)
        {if(Process(returnStatement)){var returnStatementNew = rewriter.Rewrite(returnStatement); return base.Rewrite(returnStatementNew);}
            return base.Rewrite(returnStatement);
        }

        public override IExpression Rewrite(IReturnValue returnValue)
        {if(Process(returnValue)){var returnValueNew = rewriter.Rewrite(returnValue); return base.Rewrite(returnValueNew);}
            return base.Rewrite(returnValue);
        }

        public override IExpression Rewrite(IRightShift rightShift)
        {if(Process(rightShift)){var rightShiftNew = rewriter.Rewrite(rightShift); return base.Rewrite(rightShiftNew);}
            return base.Rewrite(rightShift);
        }

        public override IExpression Rewrite(IRuntimeArgumentHandleExpression runtimeArgumentHandleExpression)
        {if(Process(runtimeArgumentHandleExpression)){var runtimeArgumentHandleExpressionNew = rewriter.Rewrite(runtimeArgumentHandleExpression); return base.Rewrite(runtimeArgumentHandleExpressionNew);}
            return base.Rewrite(runtimeArgumentHandleExpression);
        }

        public override IExpression Rewrite(ISizeOf sizeOf)
        {if(Process(sizeOf)){var sizeOfNew = rewriter.Rewrite(sizeOf); return base.Rewrite(sizeOfNew);}
            return base.Rewrite(sizeOf);
        }

        public override IExpression Rewrite(IStackArrayCreate stackArrayCreate)
        {if(Process(stackArrayCreate)){var stackArrayCreateNew = rewriter.Rewrite(stackArrayCreate); return base.Rewrite(stackArrayCreateNew);}
            return base.Rewrite(stackArrayCreate);
        }

        public override ISourceMethodBody Rewrite(ISourceMethodBody sourceMethodBody)
        {if(Process(sourceMethodBody)){var sourceMethodBodyNew = rewriter.Rewrite(sourceMethodBody); return base.Rewrite(sourceMethodBodyNew);}
            return base.Rewrite(sourceMethodBody);
        }

        public override IStatement Rewrite(IStatement statement)
        {if(Process(statement)){var statementNew = rewriter.Rewrite(statement); return base.Rewrite(statementNew);}
            return base.Rewrite(statement);
        }

        public override IExpression Rewrite(ISubtraction subtraction)
        {if(Process(subtraction)){var subtractionNew = rewriter.Rewrite(subtraction); return base.Rewrite(subtractionNew);}
            return base.Rewrite(subtraction);
        }

        public override ISwitchCase Rewrite(ISwitchCase switchCase)
        {if(Process(switchCase)){var switchCaseNew = rewriter.Rewrite(switchCase); return base.Rewrite(switchCaseNew);}
            return base.Rewrite(switchCase);
        }

        public override IStatement Rewrite(ISwitchStatement switchStatement)
        {if(Process(switchStatement)){var switchStatementNew = rewriter.Rewrite(switchStatement); return base.Rewrite(switchStatementNew);}
            return base.Rewrite(switchStatement);
        }

        public override ITargetExpression Rewrite(ITargetExpression targetExpression)
        {if(Process(targetExpression)){var targetExpressionNew = rewriter.Rewrite(targetExpression); return base.Rewrite(targetExpressionNew);}
            return base.Rewrite(targetExpression);
        }

        public override IExpression Rewrite(IThisReference thisReference)
        {if(Process(thisReference)){var thisReferenceNew = rewriter.Rewrite(thisReference); return base.Rewrite(thisReferenceNew);}
            return base.Rewrite(thisReference);
        }

        public override IStatement Rewrite(IThrowStatement throwStatement)
        {if(Process(throwStatement)){var throwStatementNew = rewriter.Rewrite(throwStatement); return base.Rewrite(throwStatementNew);}
            return base.Rewrite(throwStatement);
        }

        public override IExpression Rewrite(ITokenOf tokenOf)
        {if(Process(tokenOf)){var tokenOfNew = rewriter.Rewrite(tokenOf); return base.Rewrite(tokenOfNew);}
            return base.Rewrite(tokenOf);
        }

        public override IStatement Rewrite(ITryCatchFinallyStatement tryCatchFilterFinallyStatement)
        {if(Process(tryCatchFilterFinallyStatement)){var tryCatchFilterFinallyStatementNew = rewriter.Rewrite(tryCatchFilterFinallyStatement); return base.Rewrite(tryCatchFilterFinallyStatementNew);}
            return base.Rewrite(tryCatchFilterFinallyStatement);
        }

        public override IExpression Rewrite(ITypeOf typeOf)
        {if(Process(typeOf)){var typeOfNew = rewriter.Rewrite(typeOf); return base.Rewrite(typeOfNew);}
            return base.Rewrite(typeOf);
        }

        public override IExpression Rewrite(IUnaryNegation unaryNegation)
        {if(Process(unaryNegation)){var unaryNegationNew = rewriter.Rewrite(unaryNegation); return base.Rewrite(unaryNegationNew);}
            return base.Rewrite(unaryNegation);
        }

        public override IExpression Rewrite(IUnaryPlus unaryPlus)
        {if(Process(unaryPlus)){var unaryPlusNew = rewriter.Rewrite(unaryPlus); return base.Rewrite(unaryPlusNew);}
            return base.Rewrite(unaryPlus);
        }

        public override IExpression Rewrite(IVectorLength vectorLength)
        {if(Process(vectorLength)){var vectorLengthNew = rewriter.Rewrite(vectorLength); return base.Rewrite(vectorLengthNew);}
            return base.Rewrite(vectorLength);
        }

        public override IStatement Rewrite(IWhileDoStatement whileDoStatement)
        {if(Process(whileDoStatement)){var whileDoStatementNew = rewriter.Rewrite(whileDoStatement); return base.Rewrite(whileDoStatementNew);}
            return base.Rewrite(whileDoStatement);
        }

        public override IStatement Rewrite(IYieldBreakStatement yieldBreakStatement)
        {if(Process(yieldBreakStatement)){var yieldBreakStatementNew = rewriter.Rewrite(yieldBreakStatement); return base.Rewrite(yieldBreakStatementNew);}
            return base.Rewrite(yieldBreakStatement);
        }

        public override IStatement Rewrite(IYieldReturnStatement yieldReturnStatement)
        {if(Process(yieldReturnStatement)){var yieldReturnStatementNew = rewriter.Rewrite(yieldReturnStatement); return base.Rewrite(yieldReturnStatementNew);}
            return base.Rewrite(yieldReturnStatement);
        }

        public override List<ICatchClause> Rewrite(List<ICatchClause> catchClauses)
        {if(Process(catchClauses)){var catchClausesNew = rewriter.Rewrite(catchClauses); return base.Rewrite(catchClausesNew);}
            return base.Rewrite(catchClauses);
        }

        public override List<IExpression> Rewrite(List<IExpression> expressions)
        {if(Process(expressions)){var expressionsNew = rewriter.Rewrite(expressions); return base.Rewrite(expressionsNew);}
            return base.Rewrite(expressions);
        }

        public override List<ISwitchCase> Rewrite(List<ISwitchCase> switchCases)
        {if(Process(switchCases)){var switchCasesNew = rewriter.Rewrite(switchCases); return base.Rewrite(switchCasesNew);}
            return base.Rewrite(switchCases);
        }

        public override List<IStatement> Rewrite(List<IStatement> statements)
        {if(Process(statements)){var statementsNew = rewriter.Rewrite(statements); return base.Rewrite(statementsNew);}
            return base.Rewrite(statements);
        }


        public override IAliasForType Rewrite(IAliasForType aliasForType)
        {if(Process(aliasForType)){var aliasForTypeNew = rewriter.Rewrite(aliasForType); return base.Rewrite(aliasForTypeNew);}
            return base.Rewrite(aliasForType);
        }

        public override IAliasMember Rewrite(IAliasMember aliasMember)
        {if(Process(aliasMember)){var aliasMemberNew = rewriter.Rewrite(aliasMember); return base.Rewrite(aliasMemberNew);}
            return base.Rewrite(aliasMember);
        }

        public override IArrayTypeReference Rewrite(IArrayTypeReference arrayTypeReference)
        {if(Process(arrayTypeReference)){var arrayTypeReferenceNew = rewriter.Rewrite(arrayTypeReference); return base.Rewrite(arrayTypeReferenceNew);}
            return base.Rewrite(arrayTypeReference);
        }

        public override IAssembly Rewrite(IAssembly assembly)
        {if(Process(assembly)){var assemblyNew = rewriter.Rewrite(assembly); return base.Rewrite(assemblyNew);}
            return base.Rewrite(assembly);
        }

        public override IAssemblyReference Rewrite(IAssemblyReference assemblyReference)
        {if(Process(assemblyReference)){var assemblyReferenceNew = rewriter.Rewrite(assemblyReference); return base.Rewrite(assemblyReferenceNew);}
            return base.Rewrite(assemblyReference);
        }

        public override ICustomAttribute Rewrite(ICustomAttribute customAttribute)
        {if(Process(customAttribute)){var customAttributeNew = rewriter.Rewrite(customAttribute); return base.Rewrite(customAttributeNew);}
            return base.Rewrite(customAttribute);
        }

        public override ICustomModifier Rewrite(ICustomModifier customModifier)
        {if(Process(customModifier)){var customModifierNew = rewriter.Rewrite(customModifier); return base.Rewrite(customModifierNew);}
            return base.Rewrite(customModifier);
        }

        public override IEventDefinition Rewrite(IEventDefinition eventDefinition)
        {if(Process(eventDefinition)){var eventDefinitionNew = rewriter.Rewrite(eventDefinition); return base.Rewrite(eventDefinitionNew);}
            return base.Rewrite(eventDefinition);
        }

        public override IFieldDefinition Rewrite(IFieldDefinition fieldDefinition)
        {if(Process(fieldDefinition)){var fieldDefinitionNew = rewriter.Rewrite(fieldDefinition); return base.Rewrite(fieldDefinitionNew);}
            return base.Rewrite(fieldDefinition);
        }

        public override IFieldReference Rewrite(IFieldReference fieldReference)
        {if(Process(fieldReference)){var fieldReferenceNew = rewriter.Rewrite(fieldReference); return base.Rewrite(fieldReferenceNew);}
            return base.Rewrite(fieldReference);
        }

        public override object RewriteReference(ILocalDefinition localDefinition)
        {if(Process(localDefinition)){var localDefinitionNew = rewriter.Rewrite(localDefinition); return base.Rewrite(localDefinitionNew);}
            return base.RewriteReference(localDefinition);
        }

        public override object RewriteReference(IParameterDefinition parameterDefinition)
        {if(Process(parameterDefinition)){var parameterDefinitionNew = rewriter.Rewrite(parameterDefinition); return base.Rewrite(parameterDefinitionNew);}
            return base.RewriteReference(parameterDefinition);
        }

        public override IFieldReference RewriteUnspecialized(IFieldReference fieldReference)
        {if(Process(fieldReference)){var fieldReferenceNew = rewriter.Rewrite(fieldReference); return base.Rewrite(fieldReferenceNew);}
            return base.RewriteUnspecialized(fieldReference);
        }

        public override IFileReference Rewrite(IFileReference fileReference)
        {if(Process(fileReference)){var fileReferenceNew = rewriter.Rewrite(fileReference); return base.Rewrite(fileReferenceNew);}
            return base.Rewrite(fileReference);
        }

        public override IFunctionPointerTypeReference Rewrite(IFunctionPointerTypeReference functionPointerTypeReference)
        {if(Process(functionPointerTypeReference)){var functionPointerTypeReferenceNew = rewriter.Rewrite(functionPointerTypeReference); return base.Rewrite(functionPointerTypeReferenceNew);}
            return base.Rewrite(functionPointerTypeReference);
        }

        public override IGenericMethodInstanceReference Rewrite(IGenericMethodInstanceReference genericMethodInstanceReference)
        {if(Process(genericMethodInstanceReference)){var genericMethodInstanceReferenceNew = rewriter.Rewrite(genericMethodInstanceReference); return base.Rewrite(genericMethodInstanceReferenceNew);}
            return base.Rewrite(genericMethodInstanceReference);
        }

        public override IGenericMethodParameter Rewrite(IGenericMethodParameter genericMethodParameter)
        {if(Process(genericMethodParameter)){var genericMethodParameterNew = rewriter.Rewrite(genericMethodParameter); return base.Rewrite(genericMethodParameterNew);}
            return base.Rewrite(genericMethodParameter);
        }

        public override ITypeReference Rewrite(IGenericMethodParameterReference genericMethodParameterReference)
        {if(Process(genericMethodParameterReference)){var genericMethodParameterReferenceNew = rewriter.Rewrite(genericMethodParameterReference); return base.Rewrite(genericMethodParameterReferenceNew);}
            return base.Rewrite(genericMethodParameterReference);
        }

        public override ITypeReference Rewrite(IGenericTypeInstanceReference genericTypeInstanceReference)
        {if(Process(genericTypeInstanceReference)){var genericTypeInstanceReferenceNew = rewriter.Rewrite(genericTypeInstanceReference); return base.Rewrite(genericTypeInstanceReferenceNew);}
            return base.Rewrite(genericTypeInstanceReference);
        }

        public override IGenericTypeParameter Rewrite(IGenericTypeParameter genericTypeParameter)
        {if(Process(genericTypeParameter)){var genericTypeParameterNew = rewriter.Rewrite(genericTypeParameter); return base.Rewrite(genericTypeParameterNew);}
            return base.Rewrite(genericTypeParameter);
        }

        public override ITypeReference Rewrite(IGenericTypeParameterReference genericTypeParameterReference)
        {if(Process(genericTypeParameterReference)){var genericTypeParameterReferenceNew = rewriter.Rewrite(genericTypeParameterReference); return base.Rewrite(genericTypeParameterReferenceNew);}
            return base.Rewrite(genericTypeParameterReference);
        }

        public override IGlobalFieldDefinition Rewrite(IGlobalFieldDefinition globalFieldDefinition)
        {if(Process(globalFieldDefinition)){var globalFieldDefinitionNew = rewriter.Rewrite(globalFieldDefinition); return base.Rewrite(globalFieldDefinitionNew);}
            return base.Rewrite(globalFieldDefinition);
        }

        public override IGlobalMethodDefinition Rewrite(IGlobalMethodDefinition globalMethodDefinition)
        {if(Process(globalMethodDefinition)){var globalMethodDefinitionNew = rewriter.Rewrite(globalMethodDefinition); return base.Rewrite(globalMethodDefinitionNew);}
            return base.Rewrite(globalMethodDefinition);
        }

        public override ILocalDefinition Rewrite(ILocalDefinition localDefinition)
        {if(Process(localDefinition)){var localDefinitionNew = rewriter.Rewrite(localDefinition); return base.Rewrite(localDefinitionNew);}
            return base.Rewrite(localDefinition);
        }

        public override IManagedPointerTypeReference Rewrite(IManagedPointerTypeReference managedPointerTypeReference)
        {if(Process(managedPointerTypeReference)){var managedPointerTypeReferenceNew = rewriter.Rewrite(managedPointerTypeReference); return base.Rewrite(managedPointerTypeReferenceNew);}
            return base.Rewrite(managedPointerTypeReference);
        }

        public override IMarshallingInformation Rewrite(IMarshallingInformation marshallingInformation)
        {if(Process(marshallingInformation)){var marshallingInformationNew = rewriter.Rewrite(marshallingInformation); return base.Rewrite(marshallingInformationNew);}
            return base.Rewrite(marshallingInformation);
        }

        public override IMetadataConstant Rewrite(IMetadataConstant constant)
        {if(Process(constant)){var constantNew = rewriter.Rewrite(constant); return base.Rewrite(constantNew);}
            return base.Rewrite(constant);
        }

        public override IMetadataCreateArray Rewrite(IMetadataCreateArray metadataCreateArray)
        {if(Process(metadataCreateArray)){var metadataCreateArrayNew = rewriter.Rewrite(metadataCreateArray); return base.Rewrite(metadataCreateArrayNew);}
            return base.Rewrite(metadataCreateArray);
        }

        public override IMetadataExpression Rewrite(IMetadataExpression metadataExpression)
        {if(Process(metadataExpression)){var metadataExpressionNew = rewriter.Rewrite(metadataExpression); return base.Rewrite(metadataExpressionNew);}
            return base.Rewrite(metadataExpression);
        }

        public override IMetadataNamedArgument Rewrite(IMetadataNamedArgument namedArgument)
        {if(Process(namedArgument)){var namedArgumentNew = rewriter.Rewrite(namedArgument); return base.Rewrite(namedArgumentNew);}
            return base.Rewrite(namedArgument);
        }

        public override IMetadataTypeOf Rewrite(IMetadataTypeOf metadataTypeOf)
        {if(Process(metadataTypeOf)){var metadataTypeOfNew = rewriter.Rewrite(metadataTypeOf); return base.Rewrite(metadataTypeOfNew);}
            return base.Rewrite(metadataTypeOf);
        }

        public override IMethodDefinition Rewrite(IMethodDefinition method)
        {if(Process(method)){var methodNew = rewriter.Rewrite(method); return base.Rewrite(methodNew);}
            return base.Rewrite(method);
        }

        public override IMethodImplementation Rewrite(IMethodImplementation methodImplementation)
        {if(Process(methodImplementation)){var methodImplementationNew = rewriter.Rewrite(methodImplementation); return base.Rewrite(methodImplementationNew);}
            return base.Rewrite(methodImplementation);
        }

        public override IMethodReference Rewrite(IMethodReference methodReference)
        {if(Process(methodReference)){var methodReferenceNew = rewriter.Rewrite(methodReference); return base.Rewrite(methodReferenceNew);}
            return base.Rewrite(methodReference);
        }

        public override IMethodReference RewriteUnspecialized(IMethodReference methodReference)
        {if(Process(methodReference)){var methodReferenceNew = rewriter.Rewrite(methodReference); return base.Rewrite(methodReferenceNew);}
            return base.RewriteUnspecialized(methodReference);
        }

        public override IModifiedTypeReference Rewrite(IModifiedTypeReference modifiedTypeReference)
        {if(Process(modifiedTypeReference)){var modifiedTypeReferenceNew = rewriter.Rewrite(modifiedTypeReference); return base.Rewrite(modifiedTypeReferenceNew);}
            return base.Rewrite(modifiedTypeReference);
        }

        public override IModule Rewrite(IModule module)
        {if(Process(module)){var moduleNew = rewriter.Rewrite(module); return base.Rewrite(moduleNew);}
            return base.Rewrite(module);
        }

        public override IModuleReference Rewrite(IModuleReference moduleReference)
        {if(Process(moduleReference)){var moduleReferenceNew = rewriter.Rewrite(moduleReference); return base.Rewrite(moduleReferenceNew);}
            return base.Rewrite(moduleReference);
        }

        public override INamedTypeDefinition Rewrite(INamedTypeDefinition namedTypeDefinition)
        {if(Process(namedTypeDefinition)){var namedTypeDefinitionNew = rewriter.Rewrite(namedTypeDefinition); return base.Rewrite(namedTypeDefinitionNew);}
            return base.Rewrite(namedTypeDefinition);
        }

        public override INamedTypeReference Rewrite(INamedTypeReference typeReference)
        {if(Process(typeReference)){var typeReferenceNew = rewriter.Rewrite(typeReference); return base.Rewrite(typeReferenceNew);}
            return base.Rewrite(typeReference);
        }

        public override INamespaceAliasForType Rewrite(INamespaceAliasForType namespaceAliasForType)
        {if(Process(namespaceAliasForType)){var namespaceAliasForTypeNew = rewriter.Rewrite(namespaceAliasForType); return base.Rewrite(namespaceAliasForTypeNew);}
            return base.Rewrite(namespaceAliasForType);
        }

        public override INamespaceDefinition Rewrite(INamespaceDefinition namespaceDefinition)
        {if(Process(namespaceDefinition)){var namespaceDefinitionNew = rewriter.Rewrite(namespaceDefinition); return base.Rewrite(namespaceDefinitionNew);}
            return base.Rewrite(namespaceDefinition);
        }

        public override INamespaceMember Rewrite(INamespaceMember namespaceMember)
        {if(Process(namespaceMember)){var namespaceMemberNew = rewriter.Rewrite(namespaceMember); return base.Rewrite(namespaceMemberNew);}
            return base.Rewrite(namespaceMember);
        }

        public override INamespaceTypeDefinition Rewrite(INamespaceTypeDefinition namespaceTypeDefinition)
        {if(Process(namespaceTypeDefinition)){var namespaceTypeDefinitionNew = rewriter.Rewrite(namespaceTypeDefinition); return base.Rewrite(namespaceTypeDefinitionNew);}
            return base.Rewrite(namespaceTypeDefinition);
        }

        public override INamespaceTypeReference Rewrite(INamespaceTypeReference namespaceTypeReference)
        {if(Process(namespaceTypeReference)){var namespaceTypeReferenceNew = rewriter.Rewrite(namespaceTypeReference); return base.Rewrite(namespaceTypeReferenceNew);}
            return base.Rewrite(namespaceTypeReference);
        }

        public override INestedAliasForType Rewrite(INestedAliasForType nestedAliasForType)
        {if(Process(nestedAliasForType)){var nestedAliasForTypeNew = rewriter.Rewrite(nestedAliasForType); return base.Rewrite(nestedAliasForTypeNew);}
            return base.Rewrite(nestedAliasForType);
        }

        public override INestedTypeDefinition Rewrite(INestedTypeDefinition namespaceTypeDefinition)
        {if(Process(namespaceTypeDefinition)){var namespaceTypeDefinitionNew = rewriter.Rewrite(namespaceTypeDefinition); return base.Rewrite(namespaceTypeDefinitionNew);}
            return base.Rewrite(namespaceTypeDefinition);
        }

        public override INestedTypeReference Rewrite(INestedTypeReference nestedTypeReference)
        {if(Process(nestedTypeReference)){var nestedTypeReferenceNew = rewriter.Rewrite(nestedTypeReference); return base.Rewrite(nestedTypeReferenceNew);}
            return base.Rewrite(nestedTypeReference);
        }

        public override INestedTypeReference RewriteUnspecialized(INestedTypeReference nestedTypeReference)
        {if(Process(nestedTypeReference)){var nestedTypeReferenceNew = rewriter.Rewrite(nestedTypeReference); return base.Rewrite(nestedTypeReferenceNew);}
            return base.RewriteUnspecialized(nestedTypeReference);
        }

        public override INestedUnitNamespace Rewrite(INestedUnitNamespace nestedUnitNamespace)
        {if(Process(nestedUnitNamespace)){var nestedUnitNamespaceNew = rewriter.Rewrite(nestedUnitNamespace); return base.Rewrite(nestedUnitNamespaceNew);}
            return base.Rewrite(nestedUnitNamespace);
        }

        public override INestedUnitNamespaceReference Rewrite(INestedUnitNamespaceReference nestedUnitNamespaceReference)
        {if(Process(nestedUnitNamespaceReference)){var nestedUnitNamespaceReferenceNew = rewriter.Rewrite(nestedUnitNamespaceReference); return base.Rewrite(nestedUnitNamespaceReferenceNew);}
            return base.Rewrite(nestedUnitNamespaceReference);
        }

        public override IOperation Rewrite(IOperation operation)
        {if(Process(operation)){var operationNew = rewriter.Rewrite(operation); return base.Rewrite(operationNew);}
            return base.Rewrite(operation);
        }

        public override IOperationExceptionInformation Rewrite(IOperationExceptionInformation operationExceptionInformation)
        {if(Process(operationExceptionInformation)){var operationExceptionInformationNew = rewriter.Rewrite(operationExceptionInformation); return base.Rewrite(operationExceptionInformationNew);}
            return base.Rewrite(operationExceptionInformation);
        }

        public override IParameterDefinition Rewrite(IParameterDefinition parameterDefinition)
        {if(Process(parameterDefinition)){var parameterDefinitionNew = rewriter.Rewrite(parameterDefinition); return base.Rewrite(parameterDefinitionNew);}
            return base.Rewrite(parameterDefinition);
        }

        public override IParameterTypeInformation Rewrite(IParameterTypeInformation parameterTypeInformation)
        {if(Process(parameterTypeInformation)){var parameterTypeInformationNew = rewriter.Rewrite(parameterTypeInformation); return base.Rewrite(parameterTypeInformationNew);}
            return base.Rewrite(parameterTypeInformation);
        }

        public override IPESection Rewrite(IPESection peSection)
        {if(Process(peSection)){var peSectionNew = rewriter.Rewrite(peSection); return base.Rewrite(peSectionNew);}
            return base.Rewrite(peSection);
        }

        public override IPlatformInvokeInformation Rewrite(IPlatformInvokeInformation platformInvokeInformation)
        {if(Process(platformInvokeInformation)){var platformInvokeInformationNew = rewriter.Rewrite(platformInvokeInformation); return base.Rewrite(platformInvokeInformationNew);}
            return base.Rewrite(platformInvokeInformation);
        }

        public override IPointerTypeReference Rewrite(IPointerTypeReference pointerTypeReference)
        {if(Process(pointerTypeReference)){var pointerTypeReferenceNew = rewriter.Rewrite(pointerTypeReference); return base.Rewrite(pointerTypeReferenceNew);}
            return base.Rewrite(pointerTypeReference);
        }

        public override IPropertyDefinition Rewrite(IPropertyDefinition propertyDefinition)
        {if(Process(propertyDefinition)){var propertyDefinitionNew = rewriter.Rewrite(propertyDefinition); return base.Rewrite(propertyDefinitionNew);}
            return base.Rewrite(propertyDefinition);
        }

        public override IResourceReference Rewrite(IResourceReference resourceReference)
        {if(Process(resourceReference)){var resourceReferenceNew = rewriter.Rewrite(resourceReference); return base.Rewrite(resourceReferenceNew);}
            return base.Rewrite(resourceReference);
        }

        public override IRootUnitNamespace Rewrite(IRootUnitNamespace rootUnitNamespace)
        {if(Process(rootUnitNamespace)){var rootUnitNamespaceNew = rewriter.Rewrite(rootUnitNamespace); return base.Rewrite(rootUnitNamespaceNew);}
            return base.Rewrite(rootUnitNamespace);
        }

        public override IRootUnitNamespaceReference Rewrite(IRootUnitNamespaceReference rootUnitNamespaceReference)
        {if(Process(rootUnitNamespaceReference)){var rootUnitNamespaceReferenceNew = rewriter.Rewrite(rootUnitNamespaceReference); return base.Rewrite(rootUnitNamespaceReferenceNew);}
            return base.Rewrite(rootUnitNamespaceReference);
        }

        public override ISecurityAttribute Rewrite(ISecurityAttribute securityAttribute)
        {if(Process(securityAttribute)){var securityAttributeNew = rewriter.Rewrite(securityAttribute); return base.Rewrite(securityAttributeNew);}
            return base.Rewrite(securityAttribute);
        }

        public override ISpecializedFieldReference Rewrite(ISpecializedFieldReference specializedFieldReference)
        {if(Process(specializedFieldReference)){var specializedFieldReferenceNew = rewriter.Rewrite(specializedFieldReference); return base.Rewrite(specializedFieldReferenceNew);}
            return base.Rewrite(specializedFieldReference);
        }

        public override IMethodReference Rewrite(ISpecializedMethodReference specializedMethodReference)
        {if(Process(specializedMethodReference)){var specializedMethodReferenceNew = rewriter.Rewrite(specializedMethodReference); return base.Rewrite(specializedMethodReferenceNew);}
            return base.Rewrite(specializedMethodReference);
        }

        public override INestedTypeReference Rewrite(ISpecializedNestedTypeReference specializedNestedTypeReference)
        {if(Process(specializedNestedTypeReference)){var specializedNestedTypeReferenceNew = rewriter.Rewrite(specializedNestedTypeReference); return base.Rewrite(specializedNestedTypeReferenceNew);}
            return base.Rewrite(specializedNestedTypeReference);
        }

        public override ITypeDefinition Rewrite(ITypeDefinition typeDefinition)
        {if(Process(typeDefinition)){var typeDefinitionNew = rewriter.Rewrite(typeDefinition); return base.Rewrite(typeDefinitionNew);}
            return base.Rewrite(typeDefinition);
        }

        public override ITypeDefinitionMember Rewrite(ITypeDefinitionMember typeMember)
        {if(Process(typeMember)){var typeMemberNew = rewriter.Rewrite(typeMember); return base.Rewrite(typeMemberNew);}
            return base.Rewrite(typeMember);
        }

        public override ITypeReference Rewrite(ITypeReference typeReference)
        {if(Process(typeReference)){var typeReferenceNew = rewriter.Rewrite(typeReference); return base.Rewrite(typeReferenceNew);}
            return base.Rewrite(typeReference);
        }

        public override IUnit Rewrite(IUnit unit)
        {if(Process(unit)){var unitNew = rewriter.Rewrite(unit); return base.Rewrite(unitNew);}
            return base.Rewrite(unit);
        }

        public override IUnitNamespace Rewrite(IUnitNamespace unitNamespace)
        {if(Process(unitNamespace)){var unitNamespaceNew = rewriter.Rewrite(unitNamespace); return base.Rewrite(unitNamespaceNew);}
            return base.Rewrite(unitNamespace);
        }

        public override IUnitNamespaceReference Rewrite(IUnitNamespaceReference unitNamespaceReference)
        {if(Process(unitNamespaceReference)){var unitNamespaceReferenceNew = rewriter.Rewrite(unitNamespaceReference); return base.Rewrite(unitNamespaceReferenceNew);}
            return base.Rewrite(unitNamespaceReference);
        }

        public override IUnitReference Rewrite(IUnitReference unitReference)
        {if(Process(unitReference)){var unitReferenceNew = rewriter.Rewrite(unitReference); return base.Rewrite(unitReferenceNew);}
            return base.Rewrite(unitReference);
        }

        public override IWin32Resource Rewrite(IWin32Resource win32Resource)
        {if(Process(win32Resource)){var win32ResourceNew = rewriter.Rewrite(win32Resource); return base.Rewrite(win32ResourceNew);}
            return base.Rewrite(win32Resource);
        }

        public override List<IAliasForType> Rewrite(List<IAliasForType> aliasesForTypes)
        {if(Process(aliasesForTypes)){var aliasesForTypesNew = rewriter.Rewrite(aliasesForTypes); return base.Rewrite(aliasesForTypesNew);}
            return base.Rewrite(aliasesForTypes);
        }

        public override List<IAliasMember> Rewrite(List<IAliasMember> aliasMembers)
        {if(Process(aliasMembers)){var aliasMembersNew = rewriter.Rewrite(aliasMembers); return base.Rewrite(aliasMembersNew);}
            return base.Rewrite(aliasMembers);
        }

        public override List<IAssemblyReference> Rewrite(List<IAssemblyReference> assemblyReferences)
        {if(Process(assemblyReferences)){var assemblyReferencesNew = rewriter.Rewrite(assemblyReferences); return base.Rewrite(assemblyReferencesNew);}
            return base.Rewrite(assemblyReferences);
        }

        public override List<ICustomAttribute> Rewrite(List<ICustomAttribute> customAttributes)
        {if(Process(customAttributes)){var customAttributesNew = rewriter.Rewrite(customAttributes); return base.Rewrite(customAttributesNew);}
            return base.Rewrite(customAttributes);
        }

        public override List<ICustomModifier> Rewrite(List<ICustomModifier> customModifiers)
        {if(Process(customModifiers)){var customModifiersNew = rewriter.Rewrite(customModifiers); return base.Rewrite(customModifiersNew);}
            return base.Rewrite(customModifiers);
        }

        public override List<IEventDefinition> Rewrite(List<IEventDefinition> events)
        {if(Process(events)){var eventsNew = rewriter.Rewrite(events); return base.Rewrite(eventsNew);}
            return base.Rewrite(events);
        }

        public override List<IFieldDefinition> Rewrite(List<IFieldDefinition> fields)
        {if(Process(fields)){var fieldsNew = rewriter.Rewrite(fields); return base.Rewrite(fieldsNew);}
            return base.Rewrite(fields);
        }

        public override List<IFileReference> Rewrite(List<IFileReference> fileReferences)
        {if(Process(fileReferences)){var fileReferencesNew = rewriter.Rewrite(fileReferences); return base.Rewrite(fileReferencesNew);}
            return base.Rewrite(fileReferences);
        }

        public override List<IGenericMethodParameter> Rewrite(List<IGenericMethodParameter> genericParameters)
        {if(Process(genericParameters)){var genericParametersNew = rewriter.Rewrite(genericParameters); return base.Rewrite(genericParametersNew);}
            return base.Rewrite(genericParameters);
        }

        public override List<IGenericTypeParameter> Rewrite(List<IGenericTypeParameter> genericParameters)
        {if(Process(genericParameters)){var genericParametersNew = rewriter.Rewrite(genericParameters); return base.Rewrite(genericParametersNew);}
            return base.Rewrite(genericParameters);
        }

        public override List<ILocalDefinition> Rewrite(List<ILocalDefinition> localDefinitions)
        {if(Process(localDefinitions)){var localDefinitionsNew = rewriter.Rewrite(localDefinitions); return base.Rewrite(localDefinitionsNew);}
            return base.Rewrite(localDefinitions);
        }

        public override List<IMetadataExpression> Rewrite(List<IMetadataExpression> expressions)
        {if(Process(expressions)){var expressionsNew = rewriter.Rewrite(expressions); return base.Rewrite(expressionsNew);}
            return base.Rewrite(expressions);
        }

        public override List<IMetadataNamedArgument> Rewrite(List<IMetadataNamedArgument> namedArguments)
        {if(Process(namedArguments)){var namedArgumentsNew = rewriter.Rewrite(namedArguments); return base.Rewrite(namedArgumentsNew);}
            return base.Rewrite(namedArguments);
        }

        public override List<IMethodDefinition> Rewrite(List<IMethodDefinition> methods)
        {if(Process(methods)){var methodsNew = rewriter.Rewrite(methods); return base.Rewrite(methodsNew);}
            return base.Rewrite(methods);
        }

        public override List<IMethodImplementation> Rewrite(List<IMethodImplementation> methodImplementations)
        {if(Process(methodImplementations)){var methodImplementationsNew = rewriter.Rewrite(methodImplementations); return base.Rewrite(methodImplementationsNew);}
            return base.Rewrite(methodImplementations);
        }

        public override List<IMethodReference> Rewrite(List<IMethodReference> methodReferences)
        {if(Process(methodReferences)){var methodReferencesNew = rewriter.Rewrite(methodReferences); return base.Rewrite(methodReferencesNew);}
            return base.Rewrite(methodReferences);
        }

        public override List<IModule> Rewrite(List<IModule> modules)
        {if(Process(modules)){var modulesNew = rewriter.Rewrite(modules); return base.Rewrite(modulesNew);}
            return base.Rewrite(modules);
        }

        public override List<IModuleReference> Rewrite(List<IModuleReference> moduleReferences)
        {if(Process(moduleReferences)){var moduleReferencesNew = rewriter.Rewrite(moduleReferences); return base.Rewrite(moduleReferencesNew);}
            return base.Rewrite(moduleReferences);
        }

        public override List<INamedTypeDefinition> Rewrite(List<INamedTypeDefinition> types)
        {if(Process(types)){var typesNew = rewriter.Rewrite(types); return base.Rewrite(typesNew);}
            return base.Rewrite(types);
        }

        public override List<INamespaceMember> Rewrite(List<INamespaceMember> namespaceMembers)
        {if(Process(namespaceMembers)){var namespaceMembersNew = rewriter.Rewrite(namespaceMembers); return base.Rewrite(namespaceMembersNew);}
            return base.Rewrite(namespaceMembers);
        }

        public override List<INestedTypeDefinition> Rewrite(List<INestedTypeDefinition> nestedTypes)
        {if(Process(nestedTypes)){var nestedTypesNew = rewriter.Rewrite(nestedTypes); return base.Rewrite(nestedTypesNew);}
            return base.Rewrite(nestedTypes);
        }

        public override List<IOperation> Rewrite(List<IOperation> operations)
        {if(Process(operations)){var operationsNew = rewriter.Rewrite(operations); return base.Rewrite(operationsNew);}
            return base.Rewrite(operations);
        }

        public override List<IOperationExceptionInformation> Rewrite(List<IOperationExceptionInformation> operationExceptionInformations)
        {if(Process(operationExceptionInformations)){var operationExceptionInformationsNew = rewriter.Rewrite(operationExceptionInformations); return base.Rewrite(operationExceptionInformationsNew);}
            return base.Rewrite(operationExceptionInformations);
        }

        public override List<IParameterDefinition> Rewrite(List<IParameterDefinition> parameters)
        {if(Process(parameters)){var parametersNew = rewriter.Rewrite(parameters); return base.Rewrite(parametersNew);}
            return base.Rewrite(parameters);
        }

        public override List<IParameterTypeInformation> Rewrite(List<IParameterTypeInformation> parameterTypeInformations)
        {if(Process(parameterTypeInformations)){var parameterTypeInformationsNew = rewriter.Rewrite(parameterTypeInformations); return base.Rewrite(parameterTypeInformationsNew);}
            return base.Rewrite(parameterTypeInformations);
        }

        public override List<IPESection> Rewrite(List<IPESection> peSections)
        {if(Process(peSections)){var peSectionsNew = rewriter.Rewrite(peSections); return base.Rewrite(peSectionsNew);}
            return base.Rewrite(peSections);
        }

        public override List<IPropertyDefinition> Rewrite(List<IPropertyDefinition> properties)
        {if(Process(properties)){var propertiesNew = rewriter.Rewrite(properties); return base.Rewrite(propertiesNew);}
            return base.Rewrite(properties);
        }

        public override List<IResourceReference> Rewrite(List<IResourceReference> resourceReferences)
        {if(Process(resourceReferences)){var resourceReferencesNew = rewriter.Rewrite(resourceReferences); return base.Rewrite(resourceReferencesNew);}
            return base.Rewrite(resourceReferences);
        }

        public override List<ISecurityAttribute> Rewrite(List<ISecurityAttribute> securityAttributes)
        {if(Process(securityAttributes)){var securityAttributesNew = rewriter.Rewrite(securityAttributes); return base.Rewrite(securityAttributesNew);}
            return base.Rewrite(securityAttributes);
        }

        public override List<ITypeDefinitionMember> Rewrite(List<ITypeDefinitionMember> typeMembers)
        {if(Process(typeMembers)){var typeMembersNew = rewriter.Rewrite(typeMembers); return base.Rewrite(typeMembersNew);}
            return base.Rewrite(typeMembers);
        }

        public override List<ITypeReference> Rewrite(List<ITypeReference> typeReferences)
        {if(Process(typeReferences)){var typeReferencesNew = rewriter.Rewrite(typeReferences); return base.Rewrite(typeReferencesNew);}
            return base.Rewrite(typeReferences);
        }

        public override List<IWin32Resource> Rewrite(List<IWin32Resource> win32Resources)
        {if(Process(win32Resources)){var win32ResourcesNew = rewriter.Rewrite(win32Resources); return base.Rewrite(win32ResourcesNew);}
            return base.Rewrite(win32Resources);
        }

    }
}