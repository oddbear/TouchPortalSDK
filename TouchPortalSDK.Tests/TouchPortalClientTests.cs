using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Sockets;

namespace TouchPortalSDK.Tests
{
    public class TouchPortalClientTests
    {
        private Mock<ITouchPortalSocket> _touchPortalSocketMock;
        
        private TouchPortalClient _client;

        [SetUp]
        public void Setup()
        {
            _touchPortalSocketMock = new Mock<ITouchPortalSocket>(MockBehavior.Strict);
            _touchPortalSocketMock.SetupProperty(mock => mock.OnMessage);
            _touchPortalSocketMock.SetupProperty(mock => mock.OnClose);
            
            _client = new TouchPortalClient(default, _touchPortalSocketMock.Object);
        }

        [Test]
        public void CreateState_NoId()
        {
            var result = _client.CreateState(null, "displayName", "defaultValue");
            Assert.False(result);
        }

        [Test]
        public void CreateState_WithId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .Returns(true);

            var result = _client.CreateState("stateId", "displayName", "defaultValue");
            Assert.True(result);
            Assert.AreEqual("createState", parameter["type"]);
            Assert.AreEqual("stateId", parameter["id"]); 
            Assert.AreEqual("displayName", parameter["desc"]);
            Assert.AreEqual("defaultValue", parameter["defaultValue"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public void RemoveState_NoId()
        {
            var result = _client.RemoveState(null);
            Assert.False(result);
        }

        [Test]
        public void RemoveState_WithId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .Returns(true);

            var result = _client.RemoveState("stateId");
            Assert.True(result);
            Assert.AreEqual("removeState", parameter["type"]);
            Assert.AreEqual("stateId", parameter["id"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public void UpdateState_NoId()
        {
            var result = _client.UpdateState(null, "value");
            Assert.False(result);
        }

        [Test]
        public void UpdateState_WithId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .Returns(true);

            var result = _client.UpdateState("stateId", "value");
            Assert.True(result);
            Assert.AreEqual("stateUpdate", parameter["type"]);
            Assert.AreEqual("stateId", parameter["id"]);
            Assert.AreEqual("value", parameter["value"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public void UpdateChoices_NoId()
        {
            var result = _client.UpdateChoice(null, new [] { "value" }, null);
            Assert.False(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public void UpdateState_WithId_WithOutInstanceId(string instanceId)
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .Returns(true);

            var result = _client.UpdateChoice("listId", new[] { "value" }, instanceId);
            Assert.True(result);
            Assert.AreEqual("choiceUpdate", parameter["type"]);
            Assert.AreEqual("listId", parameter["id"]);
            CollectionAssert.AreEquivalent(new [] { "value" }, (IEnumerable)parameter["value"]);
            //This is removed if null or empty during:
            Assert.False(parameter.ContainsKey("instanceId"));

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public void UpdateState_WithId_WithInstanceId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .Returns(true);

            var result = _client.UpdateChoice("listId", new[] { "value" }, "instanceId");
            Assert.True(result);
            Assert.AreEqual("choiceUpdate", parameter["type"]);
            Assert.AreEqual("listId", parameter["id"]);
            CollectionAssert.AreEquivalent(new[] { "value" }, (IEnumerable)parameter["value"]);
            Assert.AreEqual("instanceId", parameter["instanceId"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public void Close_WithException()
        {
            var expected = new Exception();
            _touchPortalSocketMock.Setup(mock => mock.CloseSocket());
            Exception result = null;
            _client.OnClosed = exception => result = exception;
            _client.Close(expected);
            Assert.AreSame(expected, result);
        }

        [Test]
        public void Close_WithoutException()
        {
            _touchPortalSocketMock.Setup(mock => mock.CloseSocket());
            Exception result = null;
            _client.OnClosed = exception => result = exception;
            _client.Close(null);
            Assert.Null(result);
        }

        [Test]
        public void Connect_Success()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).Returns(true);
            _touchPortalSocketMock.Setup(mock => mock.Pair()).Returns("{}");
            _touchPortalSocketMock.Setup(mock => mock.Listen()).Returns(true);

            var result = _client.Connect();
            Assert.True(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Pair(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Listen(), Times.Once);
        }

        [Test]
        public void Connect_CouldNotListen()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).Returns(true);
            _touchPortalSocketMock.Setup(mock => mock.Pair()).Returns("{}");
            _touchPortalSocketMock.Setup(mock => mock.Listen()).Returns(false);

            var result = _client.Connect();
            Assert.False(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Pair(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Listen(), Times.Once);
        }

        [Test]
        public void Connect_CouldNotPair()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).Returns(true);
            _touchPortalSocketMock.Setup(mock => mock.Pair()).Returns(null as string);

            var result = _client.Connect();
            Assert.False(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Pair(), Times.Once);
        }

        [Test]
        public void Connect_CouldNotConnect()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).Returns(false);

            var result = _client.Connect();
            Assert.False(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
        }
    }
}
