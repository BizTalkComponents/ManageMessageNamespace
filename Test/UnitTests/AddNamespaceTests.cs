using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.PipelineComponents.ManageMessageNamespace.Tests.Constants;
using Shared.PipelineComponents.ManageMessageNamespace.Tests.Extensions;
using Winterdom.BizTalk.PipelineTesting;

namespace Shared.PipelineComponents.ManageMessageNamespace.Tests
{
    [TestClass]
    public class AddNamespaceTests
    {
        [TestMethod]
        public void AddUnqualifiedNamespace()
        {
            var addNamespaceComponent = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Unqualified
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.NoNamespaceXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);

                reader.MoveToNextElement();
                TestHelper.AssertEmptyNamespace(reader);
            }
        }

        [TestMethod]
        public void AddUnqualifiedNamespaceWithContextUpdate()
        {
            var addNamespaceComponent = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Unqualified,
                ShouldUpdateMessageTypeContext = true
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.NoNamespaceXmlFilePath, components);

            Assert.IsTrue(result[0].Context.Read(Misc.SystemPropertyName, Misc.SystemPropertyNamespace).ToString() == string.Concat(Misc.NamespaceToAdd, "#", "Tests"), "Incorrect or missing context in message");

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);

                reader.MoveToNextElement();
                TestHelper.AssertEmptyNamespace(reader);
            }
        }

        [TestMethod]
        public void AddQualifiedNamespace()
        {
            var addNamespaceComponent = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Qualified
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.NoNamespaceXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);
                TestHelper.AssertPrefix(reader);

                reader.MoveToNextElement();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);
                TestHelper.AssertPrefix(reader);
            }
        }

        [TestMethod]
        public void AddQualifiedNamespaceToChildNode()
        {
            var addNamespaceComponentToSubNode = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Qualified,
                XPath = "Tests/Test1"
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponentToSubNode, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.NoNamespaceXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertEmptyNamespace(reader);

                reader.MoveToNextElement();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);
                TestHelper.AssertPrefix(reader);

                reader.MoveToNextElement();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);
                TestHelper.AssertPrefix(reader);
            }
        }

        [TestMethod]
        public void AddUnqualifiedNamespaceToChildNode()
        {
            var addNamespaceComponentToSubNode = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Unqualified,
                XPath = "Tests/Test1"
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponentToSubNode, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.NoNamespaceXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertEmptyNamespace(reader);

                reader.MoveToNextElement();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);
                TestHelper.AssertPrefix(reader);

                reader.MoveToNextElement();
                TestHelper.AssertEmptyNamespace(reader);
            }
        }

        [TestMethod]
        public void AddUnqualifiedNamespaceToChildNodeWithExistingParentNamespace()
        {
            var addNamespaceComponentToSubNode = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Unqualified,
                XPath = "http://test:Tests/Test1"
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponentToSubNode, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.UnqualifiedXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertNamespaceValue(reader, Misc.ExistingNamespace);

                reader.MoveToNextElement();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);
                TestHelper.AssertPrefix(reader);

                reader.MoveToNextElement();
                TestHelper.AssertEmptyNamespace(reader);
            }
        }

        [TestMethod]
        public void AddMultipleNamespaces()
        {
            var addNamespaceComponentToRootNode = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Unqualified
            };

            var addNamespaceComponentToSubNode1 = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd2,
                NamespaceForm = NamespaceFormEnum.Qualified,
                XPath = "http://testAdd:Tests/Test1"
            };

            var addNamespaceComponentToSubNode2 = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd3,
                NamespaceForm = NamespaceFormEnum.Unqualified,
                XPath = "http://testAdd:Tests/Test2"
            };


            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponentToRootNode, PipelineStage.Validate),
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponentToSubNode1, PipelineStage.Validate),
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponentToSubNode2, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.NoNamespaceXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);

                Assert.IsTrue(reader.ReadToFollowing("Test1", Misc.NamespaceToAdd2));
                TestHelper.AssertPrefix(reader);

                Assert.IsTrue(reader.ReadToFollowing("Test2", Misc.NamespaceToAdd3));
                reader.MoveToNextElement();
                TestHelper.AssertEmptyNamespace(reader);
            }
        }

        [TestMethod]
        public void AddDefaultNamespace()
        {
            var addNamespaceComponent = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Default
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponent, PipelineStage.Validate)
            };

            var result = TestHelper.ExecuteReceivePipeline(TestFiles.NoNamespaceXmlFilePath, components);

            using (var reader = XmlReader.Create(result[0].BodyPart.Data))
            {
                reader.MoveToContent();
                TestHelper.AssertNamespaceValue(reader, Misc.NamespaceToAdd);
                TestHelper.AssertEmptyPrefix(reader);

                reader.MoveToNextElement();
                TestHelper.AssertEmptyPrefix(reader);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Exception should be thrown as namespace exists")]
        public void ExistingNamespaceException()
        {
            var addNamespaceComponent = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
                NamespaceForm = NamespaceFormEnum.Unqualified
            };

            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(addNamespaceComponent, PipelineStage.Validate)
            };

            TestHelper.ExecuteReceivePipeline(TestFiles.UnqualifiedXmlFilePath, components);
        }

        [TestMethod]
        public void NonXmlContent()
        {
            var addNamespaceComponent = new AddNamespaceComponent
            {
                NewNamespace = Misc.NamespaceToAdd,
            };

            TestHelper.TestFlatFile(addNamespaceComponent);
        }
    }
}
