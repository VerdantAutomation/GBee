using NETMF.OpenSource.XBee.Api;
using NUnit.Framework;

namespace NETMF.OpenSource.XBee.Util
{
    [TestFixture]
    class ArraysTests
    {
        [Test]
        public void equal_arrays_test()
        {
            var a1 = new byte[] { 1, 6, 9, 34 };
            var a2 = new byte[] { 1, 6, 9, 34 };
            Assert.IsTrue(Arrays.AreEqual(a1, a2));
        }

        [Test]
        public void empty_arrays_are_equal_test()
        {
            var a1 = new byte[0];
            var a2 = new byte[0];
            Assert.IsTrue(Arrays.AreEqual(a1, a2));
        }

        [Test]
        public void different_lenght_test()
        {
            var a1 = new byte[] { 1, 6, 9, 34, 33 };
            var a2 = new byte[] { 1, 6, 9, 34 };
            Assert.IsFalse(Arrays.AreEqual(a1, a2));
        }

        [Test]
        public void same_lenght_different_order_test()
        {
            var a1 = new byte[] { 6, 1, 34, 9 };
            var a2 = new byte[] { 1, 6, 9, 34 };
            Assert.IsFalse(Arrays.AreEqual(a1, a2));
        }

        [Test]
        public void equal_arrays_equal_hashcode_test()
        {
            var a1 = (XBeeAddress16.Broadcast as XBeeAddress).Address;
            var a2 = (XBeeAddress16.Broadcast as XBeeAddress).Address;
            Assert.AreEqual(Arrays.HashCode(a1), Arrays.HashCode(a2));
        }

        [Test]
        public void different_arrays_different_hashcode_test()
        {
            var a1 = (XBeeAddress16.Broadcast as XBeeAddress).Address;
            var a2 = XBeeAddress64.Broadcast.Address;
            Assert.AreNotEqual(Arrays.HashCode(a1), Arrays.HashCode(a2));
        }

        [Test]
        public void ushort_to_array_test()
        {
            const ushort value = 0x0102;
            CollectionAssert.AreEqual(new byte[]{0x01, 0x02}, Arrays.ToByteArray(value));
        }

        [Test]
        public void string_and_byte_array_test()
        {
            const string testString = "This is a test";
            var result = Arrays.ToByteArray(testString);
            Assert.AreEqual(testString.Length, result.Length);
            Assert.AreEqual(testString, Arrays.ToString(result));
        }

        [Test]
        public void offset_string_and_byte_array_test()
        {
            const string testString = "This is a test";
            const int offset = 5;
            const int count = 5;
            var result = Arrays.ToByteArray(testString, offset, count);
            Assert.AreEqual(count, result.Length);
            Assert.AreEqual(testString.Substring(offset, count), Arrays.ToString(result));
        }

        [Test]
        public void ulong_to_array_test()
        {
            const ulong value = 0x1122334455667788;
            var result = Arrays.ToByteArray(value);
            Assert.AreEqual(8, result.Length);
            Assert.AreEqual(new byte[]{0x11,0x22,0x33,0x44,0x55,0x66,0x77,0x88}, result);
        }
    }
}
