namespace VisualMutator.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommonUtilityInfrastructure;

    public static class UserMessages
    {
       

public static string ErrorPretest_TestsFailed(string firstTestThatFailedFullName)
{
    return @"One or more tests failed on unmodified source assemblies. 
All tests must pass before starting mutation testing process. 
Mutation session cannot continue.

First test that failed: {0}"
        .Formatted(firstTestThatFailedFullName);


}
public static string ErrorPretest_UnknownError(string exceptionMessage)
{
    return 
@"Error occurred while pre-testing source assemblies. Mutation session cannot continue.

Exception:
{0}"
        .Formatted(exceptionMessage);


}

public static string ErrorPretest_VerificationFailure(string exceptionMessage)
{
    return
@"Some of source project assemblies failed veryfication.
Mutant verification will be disabled, also errors may occur during testing and while decompiling code for preview."
        .Formatted(exceptionMessage);


}















    }
}