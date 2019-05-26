using System;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets.Common
{
    public interface ICmdletWriterContainer
    {
        ICmdletWriter CmdletWriter { get; }
    }

    public interface ICmdletWriter
    {
        void WriteVerbose(string text);
        void WriteError(string message);
        void WriteError(string message, ErrorCategory category);
        void WriteError(Exception ex, string errorId, ErrorCategory category);
        void WriteError(Exception ex, string errorId, ErrorCategory category, PSCmdlet source);
        void WriteDebug(string text);
        void WriteInformation(string source, object data);
    }
}