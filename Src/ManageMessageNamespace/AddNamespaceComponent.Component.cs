using System;
using System.ComponentModel;

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    public partial class AddNamespaceComponent
    {
        #region IBaseComponent members

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "Add Namespace Component";
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
                return @"Adds a namespace to message.";
            }
        }

        #endregion 
   
        public void GetClassID(out Guid classid)
        {
            classid = new Guid("F961D046-8F95-455E-96CC-A30B41EDD1D9");
        }

        public void InitNew() { }


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