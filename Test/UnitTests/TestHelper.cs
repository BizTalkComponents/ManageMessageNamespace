using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.PipelineComponents.ManageMessageNamespace.Tests.Constants;
using Shared.PipelineComponents.ManageMessageNamespace.Tests.Models;
using Winterdom.BizTalk.PipelineTesting;

namespace BizTalkComponents.ManageMessageNamespace.Tests
{
    static class TestHelper
    {
        public static MessageCollection ExecuteReceivePipeline(string path, IList<Tuple<IBaseComponent, PipelineStage>> components, IEnumerable<ContextProperty> properties = null)
        {
            using (var file = File.Open(path, FileMode.Open))
            {
                IBaseMessage message = MessageHelper.CreateFromStream(file);

                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        message.Context.Promote(property.Name, property.Namespace, property.Value);
                    }
                }

                var receivePipeline = PipelineFactory.CreateEmptyReceivePipeline();

                foreach (var component in components)
                {
                    receivePipeline.AddComponent(component.Item1, component.Item2);
                }

                return receivePipeline.Execute(message);
            }
        }

        public static bool HasUtf8Bom(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[3];
            stream.Read(buffer, 0, 3);
            
            stream.Seek(0, SeekOrigin.Begin);

            return Encoding.UTF8.GetPreamble().SequenceEqual(buffer);
        }

        public static void TestFlatFile(IBaseComponent component)
        {
            var components = new List<Tuple<IBaseComponent, PipelineStage>>
            {
                new Tuple<IBaseComponent, PipelineStage>(component, PipelineStage.Validate)
            };

            var result = ExecuteReceivePipeline(TestFiles.FlatFileFilePath, components);

            using (var reader = new StreamReader(result[0].BodyPart.Data))
            {
                var line = reader.ReadLine();
                Assert.IsTrue(line != null && line.Equals("test test"), "Text file is not unchanged");
            }
        }

        public static void AssertEmptyPrefix(XmlReader reader)
        {
            Assert.IsTrue(reader.Prefix == string.Empty, "Node is not without with prefix");
        }

        public static void AssertPrefix(XmlReader reader)
        {
            Assert.IsTrue(reader.Prefix != string.Empty, "Node is not qualified with prefix");
        }

        public static void AssertEmptyNamespace(XmlReader reader)
        {
            Assert.IsTrue(reader.NamespaceURI == string.Empty, "Node node is not unqualified");
        }

        public static void AssertNamespaceValue(XmlReader reader, string namespaceToAdd)
        {
            Assert.IsTrue(reader.NamespaceURI == namespaceToAdd, "Child node is not qualified within {0}", namespaceToAdd);
        }
    }
}
