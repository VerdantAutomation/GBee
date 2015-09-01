using System;
using NUnit.Framework;

namespace NETMF.OpenSource.XBee.Util
{
    [TestFixture]
    class UshortUtilsTests
    {
        [Test]
        public void from_byte_array_to_ushort_test()
        {
            Assert.AreEqual(0x0102, UshortUtils.ToUshort(new byte[] { 0x01, 0x02 }));
        }

        [Test]
        public void from_msb_lsb_to_ushort_test()
        {
            Assert.AreEqual(0x0102, UshortUtils.ToUshort(0x01, 0x02));
        }

        [Test]
        public void from_chars_to_ushort_test()
        {
            Assert.AreEqual(0x4142, UshortUtils.FromAscii("AB"));
        }

        [Test]
        public void get_msb_test()
        {
            Assert.AreEqual(0x01, UshortUtils.Msb(0x0102));
        }

        [Test]
        public void get_lsb_test()
        {
            Assert.AreEqual(0x02, UshortUtils.Lsb(0x0102));
        }

        [Test]
        public void from_ushort_to_chars_test()
        {
            Assert.AreEqual("AB", UshortUtils.ToAscii(0x4142));
        }

        [Test]
        public void parse_10bit_analog_from_bytes_test()
        {
            const byte msb = 0xF1;
            const byte lsb = 0x23;
            Assert.AreEqual(0x0123, UshortUtils.Parse10BitAnalog(msb, lsb));
        }

        [Test]
        public void parse_10bit_analog_from_stream_test()
        {
            var value = new byte[] { 0xF1, 0x23 };
            Assert.AreEqual(0x0123, UshortUtils.Parse10BitAnalog(value));
        }

        [Test]
        public void from_single_byte_to_ushort_test()
        {
            Assert.AreEqual(0x0001, UshortUtils.ToUshort(0x01));
        }
        
        [Test]
        public void get_lsb_bit_test()
        {
            const byte lsbIndex = 0;
            const ushort value = 1 << lsbIndex;
            Assert.AreEqual(true, UshortUtils.GetBit(value, lsbIndex));
        }

        [Test]
        public void get_msb_bit_test()
        {
            const byte msbIndex = 15;
            const ushort value = 1 << msbIndex;
            Assert.AreEqual(true, UshortUtils.GetBit(value, msbIndex));
        }

        [Test]
        public void get_bit_out_of_range_test()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
                UshortUtils.GetBit(0x10, 16));
        }
    }
}