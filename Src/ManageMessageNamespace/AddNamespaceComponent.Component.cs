using System;
using System.ComponentModel;
using System.Linq;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents.ManageMessageNamespace
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
            return ValidationHelper.Validate(this, false).ToArray().GetEnumerator();
        }

        public bool Validate(out string errorMessage)
        {
            var errors = ValidationHelper.Validate(this, true).ToArray();

            if (errors.Any())
            {
                errorMessage = string.Join(",", errors);

                return false;
            }

            errorMessage = string.Empty;

            return true;
        }


        #endregion
    }
}