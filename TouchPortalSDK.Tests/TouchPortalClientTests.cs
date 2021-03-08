using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TouchPortalSDK.Models.Enums;
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

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task UpdateActionData_NoDataId(string dataId)
        {
            var result = await _client.UpdateActionData(dataId, default, default, DataType.Number);
            Assert.False(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task UpdateActionData_WithDataId_NoInstanceId(string instanceId)
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.UpdateActionData("dataId", default, default, DataType.Number, instanceId);
            Assert.True(result);
            Assert.AreEqual("updateActionData", parameter["type"]);
            CollectionAssert.DoesNotContain(parameter.Keys, "instanceId");

            var data = (Dictionary<string, object>)parameter["data"];
            Assert.AreEqual("dataId", data["id"]);
            Assert.AreEqual(0.0, data["maxValue"]);
            Assert.AreEqual(0.0, data["minValue"]);
            Assert.AreEqual("Number", data["type"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public async Task UpdateActionData_WithDataId_WithInstanceId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.UpdateActionData("dataId", default, default, DataType.Number, "instanceId");
            Assert.True(result);
            Assert.AreEqual("updateActionData", parameter["type"]);
            Assert.AreEqual("instanceId", parameter["instanceId"]);

            var data = (Dictionary<string, object>)parameter["data"];
            Assert.AreEqual("dataId", data["id"]);
            Assert.AreEqual(0.0, data["maxValue"]);
            Assert.AreEqual(0.0, data["minValue"]);
            Assert.AreEqual("Number", data["type"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task SettingUpdate_NoName(string name)
        {
            var result = await _client.SettingUpdate(name, "value");
            Assert.False(result);
        }

        [Test]
        public async Task SettingUpdate_WithName()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.SettingUpdate("name", "value");
            Assert.True(result);
            Assert.AreEqual("settingUpdate", parameter["type"]);
            Assert.AreEqual("name", parameter["name"]);
            Assert.AreEqual("value", parameter["value"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task CreateState_NoId(string stateId)
        {
            var result = await _client.CreateState(stateId, "displayName", "defaultValue");
            Assert.False(result);
        }

        [Test]
        public async Task CreateState_WithId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.CreateState("stateId", "displayName", "defaultValue");
            Assert.True(result);
            Assert.AreEqual("createState", parameter["type"]);
            Assert.AreEqual("stateId", parameter["id"]); 
            Assert.AreEqual("displayName", parameter["desc"]);
            Assert.AreEqual("defaultValue", parameter["defaultValue"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task RemoveState_NoId(string stateId)
        {
            var result = await _client.RemoveState(stateId);
            Assert.False(result);
        }

        [Test]
        public async Task RemoveState_WithId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.RemoveState("stateId");
            Assert.True(result);
            Assert.AreEqual("removeState", parameter["type"]);
            Assert.AreEqual("stateId", parameter["id"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task StateUpdate_NoId(string stateId)
        {
            var result = await _client.StateUpdate(stateId, "value");
            Assert.False(result);
        }

        [Test]
        public async Task StateUpdate_WithId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.StateUpdate("stateId", "value");
            Assert.True(result);
            Assert.AreEqual("stateUpdate", parameter["type"]);
            Assert.AreEqual("stateId", parameter["id"]);
            Assert.AreEqual("value", parameter["value"]);

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task ChoiceUpdate_NoId(string listId)
        {
            var result = await _client.ChoiceUpdate(listId, new [] { "value" }, null);
            Assert.False(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t ")]
        public async Task ChoiceUpdate_WithId_WithOutInstanceId(string instanceId)
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.ChoiceUpdate("listId", new[] { "value" }, instanceId);
            Assert.True(result);
            Assert.AreEqual("choiceUpdate", parameter["type"]);
            Assert.AreEqual("listId", parameter["id"]);
            CollectionAssert.AreEquivalent(new [] { "value" }, (IEnumerable)parameter["value"]);
            CollectionAssert.DoesNotContain(parameter.Keys, "instanceId");

            _touchPortalSocketMock.Verify(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Test]
        public async Task ChoiceUpdate_WithId_WithInstanceId()
        {
            Dictionary<string, object> parameter = null;
            _touchPortalSocketMock.Setup(mock => mock.SendMessage(It.IsAny<Dictionary<string, object>>()))
                .Callback<Dictionary<string, object>>(dict => parameter = dict)
                .ReturnsAsync(true);

            var result = await _client.ChoiceUpdate("listId", new[] { "value" }, "instanceId");
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
        public async Task Connect_Success()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).ReturnsAsync(true);
            _touchPortalSocketMock.Setup(mock => mock.Pair()).ReturnsAsync("{}");
            _touchPortalSocketMock.Setup(mock => mock.Listen()).Returns(true);

            var result = await _client.Connect();
            Assert.True(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Pair(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Listen(), Times.Once);
        }

        [Test]
        public async Task Connect_CouldNotListen()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).ReturnsAsync(true);
            _touchPortalSocketMock.Setup(mock => mock.Pair()).ReturnsAsync("{}");
            _touchPortalSocketMock.Setup(mock => mock.Listen()).Returns(false);

            var result = await _client.Connect();
            Assert.False(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Pair(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Listen(), Times.Once);
        }

        [Test]
        public async Task Connect_CouldNotPair()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).ReturnsAsync(true);
            _touchPortalSocketMock.Setup(mock => mock.Pair()).ReturnsAsync(null as string);

            var result = await _client.Connect();
            Assert.False(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
            _touchPortalSocketMock.Verify(mock => mock.Pair(), Times.Once);
        }

        [Test]
        public async Task Connect_CouldNotConnect()
        {
            _touchPortalSocketMock.Setup(mock => mock.Connect()).ReturnsAsync(false);

            var result = await _client.Connect();
            Assert.False(result);

            _touchPortalSocketMock.Verify(mock => mock.Connect(), Times.Once);
        }
    }
}
