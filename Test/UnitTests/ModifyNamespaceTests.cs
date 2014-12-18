using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.PipelineComponents.ManageMessageNamespace.Tests.Constants;
using Shared.PipelineComponents.ManageMessageNamespace.Tests.Extensions;
using Shared.PipelineComponents.ManageMessageNamespace.Tests.Models;
using Winterdom.BizTalk.PipelineTesting;

namespace Shared.PipelineComponents.ManageMessageNamespace.Tests
{
    [TestClass]
    public class ModifyNamespaceTests
    {
        [TestMethod]
        public void ModifyQualifiedDefaultNamespace()
        {
            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = Misc.ExistingNamespace,
                NewNamespace = Misc.ModifiedNamespace
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(modifyNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.QualifiedDefaultXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                Assert.IsTrue(reader.NamespaceURI == Misc.ModifiedNamespace, "Root element is not qualified within {0}", Misc.ModifiedNamespace);

                reader.MoveToNextElement();
                Assert.IsTrue(reader.NamespaceURI == Misc.ModifiedNamespace, "Child element is not qualified within {0}", Misc.ModifiedNamespace);
            }
        }

        [TestMethod]
        public void ModifyDefaultUnqualifiedNamespace()
        {
            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = Misc.ExistingNamespace,
                NewNamespace = Misc.ModifiedNamespace
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(modifyNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.UnqualifiedXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                Assert.IsTrue(reader.NamespaceURI == Misc.ModifiedNamespace, "Root element is not qualified within {0}", Misc.ModifiedNamespace);

                reader.MoveToNextElement();
                Assert.IsTrue(reader.NamespaceURI == string.Empty, "Child element is not unqaulified");
            }
        }

        [TestMethod]
        public void ModifyQualifiedNamespace()
        {
            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = Misc.ExistingNamespace,
                NewNamespace = Misc.ModifiedNamespace
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(modifyNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.QualifiedXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                Assert.IsTrue(reader.NamespaceURI == Misc.ModifiedNamespace, "Root element is not qualified within {0}", Misc.ModifiedNamespace);

                reader.MoveToNextElement();
                Assert.IsTrue(reader.NamespaceURI == Misc.ModifiedNamespace, "Child element is not qualified within {0}", Misc.ModifiedNamespace);
            }
        }

        [TestMethod]
        public void ModifyUnqualifiedNamespace()
        {
            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = Misc.ExistingNamespace,
                NewNamespace = Misc.ModifiedNamespace
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(modifyNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.UnqualifiedXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                Assert.IsTrue(reader.NamespaceURI == Misc.ModifiedNamespace, "Root element is not qualified within {0}", Misc.ModifiedNamespace);

                reader.MoveToNextElement();

                Assert.IsTrue(reader.NamespaceURI == string.Empty, "Child element should is not unqualified");
            }
        }

        [TestMethod]
        public void SkipNonMatchingNamespace()
        {
            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = "NoMatch",
                NewNamespace = Misc.ModifiedNamespace
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(modifyNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.UnqualifiedXmlFilePath, components);

            var file = File.Open(TestFiles.UnqualifiedXmlFilePath, FileMode.Open);

            Assert.IsTrue(file.ToByteArray().SequenceEqual(result[0].BodyPart.Data.ToByteArray()), "Non matching file is changed");
        }

        [TestMethod]
        public void ModifyNamespaceWithContextUpdate()
        {
            var messageTypeToAdd = string.Concat(Misc.ModifiedNamespace, "#", "Tests");

            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = Misc.ExistingNamespace,
                NewNamespace = Misc.ModifiedNamespace,
                ShouldUpdateMessageTypeContext = true
            };

            var properties = new List<ContextProperty>{ new ContextProperty
            {
                Name = Misc.SystemPropertyName,
                Namespace = Misc.SystemPropertyNamespace,
                Value = string.Concat(Misc.ModifiedNamespace,"#","Test")
                
            }};

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(modifyNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.QualifiedXmlFilePath, components, properties);

            Assert.IsTrue(result[0].Context.Read(Misc.SystemPropertyName, Misc.SystemPropertyNamespace).ToString() == messageTypeToAdd, "Context is missing new message type {0}", messageTypeToAdd);
            Assert.IsTrue(result[0].Context.IsPromoted(Misc.SystemPropertyName, Misc.SystemPropertyNamespace), "Message type is not promoted in context");
        }

        [TestMethod]
        public void HandleMissingBom()
        {
            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = Misc.ExistingNamespace,
                NewNamespace = Misc.ModifiedNamespace
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(modifyNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.MissingBomXmlFilePath, components);

            Assert.IsFalse(TestHelper.HasUtf8Bom(result[0].BodyPart.Data), "BOM has been added to file");
        }

        [TestMethod]
        public void NonXmlContent()
        {
            var modifyNamespaceComponent = new ModifyNamespaceComponent
            {
                NamespaceToModify = Misc.ExistingNamespace,
                NewNamespace = Misc.ModifiedNamespace
            };

            TestHelper.TestFlatFile(modifyNamespaceComponent);
        }
    }
}
