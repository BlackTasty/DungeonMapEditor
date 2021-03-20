using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Patcher
{
    public class UpdateFailedEventArgs : EventArgs
    {
        private string errMsg;
        private Exception exception;

        public string ErrorMessage => errMsg;

        public Exception Exception => exception;

        public UpdateFailedEventArgs(Exception exception, string errMsg)
        {
            this.errMsg = errMsg;
            this.exception = exception;
        }
    }
}
