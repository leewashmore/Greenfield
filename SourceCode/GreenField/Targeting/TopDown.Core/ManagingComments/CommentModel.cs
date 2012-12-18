using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingComments
{
    public class CommentModel
    {
        [DebuggerStepThrough]
        public CommentModel(IChangeInfoBase changeInfo, ChangesetInfoBase changesetInfo)
        {
            this.ChangeInfo = changeInfo;
            this.ChangesetInfo = changesetInfo;
        }

        public IChangeInfoBase ChangeInfo { get; private set; }
        public ChangesetInfoBase ChangesetInfo { get; private set; }
    }
}
