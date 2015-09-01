using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AtString = NETMF.OpenSource.XBee.Api.Common.AtString;

namespace NETMF.OpenSource.XBee.Api.Zigbee
{
    [TestFixture]
    public class AtCmdTests
    {
        private IEnumerable<AtCmd> _commands;

        [SetUp]
        public void set_up()
        {
            _commands = Enum.GetValues(typeof(AtCmd)).Cast<AtCmd>();
        }

        [Test]
        public void all_commands_are_unique()
        {
            var allCommands = _commands.Select(c => (ushort)c);

            foreach (var command in allCommands)
                Assert.AreEqual(1, allCommands.Count(c => c == command),
                    string.Format("AtCmd.{0} has a duplicate value in enum", (AtCmd)command));
        }

        [Test]
        public void all_commands_have_at_string_attribute()
        {
            var fieldInfos = _commands.Select(c => typeof(AtCmd).GetField(c.ToString()));

            foreach (var fieldInfo in fieldInfos)
                Assert.IsNotEmpty(fieldInfo.GetCustomAttributes(typeof(AtString), false),
                    string.Format("AtCmd.{0} is missing AtString attribute", fieldInfo.Name));
        }

        [Test]
        public void all_command_values_are_equal_to_string_attribute()
        {
            all_commands_have_at_string_attribute();

            var fieldInfo = _commands.ToDictionary(k => k, k => typeof(AtCmd).GetField(k.ToString()));

            foreach (var command in _commands)
            {
                var atString = ((AtString)fieldInfo[command].GetCustomAttributes(typeof(AtString), false)[0]);
                var atUshort = (ushort)command;
                var atBytes = new[] { (byte)(atUshort >> 8), (byte)atUshort };
                Assert.AreEqual(atString.Value, Encoding.UTF8.GetString(atBytes));
            }
        }
    }
}