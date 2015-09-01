using System;
using NUnit.Framework;

namespace NETMF.OpenSource.XBee.Util
{
    [TestFixture]
    class ByteUtilsTests
    {
        [Test]
        public void byte_array_to_base16_test()
        {
            var value = new byte[] {0x01, 0xFF, 0x10};
            Assert.AreEqual("01FF10", ByteUtils.ToBase16(value));
        }

        [Test]
        public void empty_byte_array_to_base16_test()
        {
            var value = new byte[0];
            Assert.AreEqual(string.Empty, ByteUtils.ToBase16(value));
        }

        [Test]
        public void byte_to_base16_test()
        {
            const byte value = 0x12;
            Assert.AreEqual("12", ByteUtils.ToBase16(value));
        }

        [Test]
        public void ushort_to_base16_test()
        {
            const ushort value = 0x0123;
            Assert.AreEqual("0123", ByteUtils.ToBase16(value));
        }

        [Test]
        public void one_digit_from_base16_test()
        {
            const string value = "F";
            Assert.AreEqual(15, ByteUtils.FromBase16(value));
        }

        [Test]
        public void two_digit_from_base16_test()
        {
            const string value = "10";
            Assert.AreEqual(16, ByteUtils.FromBase16(value));
        }

        [Test]
        public void get_lsb_test()
        {
            const byte lsbIndex = 0;
            const byte value = 1 << lsbIndex;
            Assert.IsTrue(ByteUtils.GetBit(value, lsbIndex));
        }

        [Test]
        public void get_msb_test()
        {
            const byte msbIndex = 7;
            const byte value = 1 << msbIndex;
            Assert.IsTrue(ByteUtils.GetBit(value, msbIndex));
        }

        [Test]
        public void get_bit_out_of_range_test()
        {
            Assert.Throws<IndexOutOfRangeException>(() => 
                ByteUtils.GetBit(0x10, 8));
        }

        [Test]
        public void format_byte_test()
        {
            const byte value = 0x80;
            Assert.AreEqual("0x80", ByteUtils.FormatByte(value));
        }
    }
}