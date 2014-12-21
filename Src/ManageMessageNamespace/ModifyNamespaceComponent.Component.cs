using System;
using System.ComponentModel;

namespace Shared.PipelineComponents.ManageMessageNamespace
{
    public partial class ModifyNamespaceComponent
    {
        #region IBaseComponent members

        [Browsable(false)]
        public string Name
        {
            get
            {
                return "Modify Namespace Component";
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
                return @"Modifies a message namespace from one namespace to another.";
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

        public void GetClassID(out Guid classid)
        {
            classid = new Guid("D4123F35-ED40-46BA-85AF-01B8D5CBBFED");
        }

        public void InitNew() { }
    }
}