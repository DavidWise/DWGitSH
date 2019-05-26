using System;
using System.Management.Automation;

namespace DWGitsh.Extensions.Cmdlets.Common
{
    public class GitCmdletWriter : ICmdletWriter
    {
        private PSCmdlet _cmd;
        public GitCmdletWriter(PSCmdlet command)
        {
            _cmd = command;
        }

        public void WriteVerbose(string text)
        {
            _cmd?.WriteVerbose(text);
        }

        public void WriteError(string message)
        {
            var ex = new Exception(message);
            WriteError(ex, "Exception", ErrorCategory.InvalidOperation, _cmd);
        }

        public void WriteError(string message, ErrorCategory category)
        {
            var ex = new Exception(message);
            WriteError(ex, "Exception", category, _cmd);
        }
        public void WriteError(Exception ex, string errorId, ErrorCategory category)
        {
            WriteError(ex, errorId, category, _cmd);
        }
        public void WriteError(Exception ex, string errorId, ErrorCategory category, PSCmdlet source)
        {
            if (_cmd != null && !string.IsNullOrWhiteSpace(errorId))
            {
                var rec = new ErrorRecord(ex, errorId, category, source);
                _cmd?.WriteError(rec);
            }
        }

        public void WriteDebug(string text)
        {
            _cmd?.WriteDebug(text);
        }

        public void WriteInformation(string source, object data)
        {
            if (!string.IsNullOrEmpty(source))
            {
                var rec = new InformationRecord(data, source);
                _cmd?.WriteInformation(rec);
            }
        }
    }
}