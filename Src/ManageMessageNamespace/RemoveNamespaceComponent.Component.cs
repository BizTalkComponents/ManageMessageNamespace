using System;
using System.ComponentModel;

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    public partial class RemoveNamespaceComponent
    {
        #region IBaseComponent members

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "Remove Namespace Component";
            }
        }

        [Browsable(false)]
        public string Version
        {
            get
            {
                return "2.2";
            }
        }

        [Browsable(false)]
        public string Description
        {
            get
            {
                return @"Removes namespace from a message.";
            }
        }

        #endregion

        #region IComponentUI members

        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        public System.Collections.IEnumerator Validate(object obj)
        {
            return null;
        }

        #endregion
    }
}