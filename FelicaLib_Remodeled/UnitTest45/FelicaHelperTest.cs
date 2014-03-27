using FelicaLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest45
{
    [TestClass]
    public class FelicaHelperTest
    {
        [TestMethod]
        public void ToInt32()
        {
            var data = new byte[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(0, data.ToInt32(0, 0));
            Assert.AreEqual(0, data.ToInt32(0, 0, true));
            Assert.AreEqual(1, data.ToInt32(0, 1));
            Assert.AreEqual(1, data.ToInt32(0, 1, true));
            Assert.AreEqual(5, data.ToInt32(4, 1));
            Assert.AreEqual(5, data.ToInt32(4, 1, true));
            Assert.AreEqual(258, data.ToInt32(0, 2));
            Assert.AreEqual(513, data.ToInt32(0, 2, true));
            Assert.AreEqual(131844, data.ToInt32(1, 3));
            Assert.AreEqual(262914, data.ToInt32(1, 3, true));
            Assert.AreEqual(1029, data.ToInt32(3, 2));
            Assert.AreEqual(1284, data.ToInt32(3, 2, true));
            Assert.AreEqual(16909060, data.ToInt32(0, 4));
            Assert.AreEqual(67305985, data.ToInt32(0, 4, true));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToInt32_Null()
        {
            var data = default(byte[]);
            data.ToInt32(0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToInt32_OutOfRange1()
        {
            var data = new byte[] { 1, 2, 3, 4 };
            data.ToInt32(0, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToInt32_OutOfRange2()
        {
            var data = new byte[] { 1, 2, 3, 4 };
            data.ToInt32(0, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void ToInt32_OutOfRange3()
        {
            var data = new byte[] { 1, 2, 3, 4 };
            data.ToInt32(-1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void ToInt32_OutOfRange4()
        {
            var data = new byte[] { 1, 2, 3, 4 };
            data.ToInt32(3, 2);
        }

        [TestMethod]
        public void ToHexString()
        {
            var data = new byte[] { 0x03, 0x6C, 0xF9, 0xAB };
            Assert.AreEqual("036CF9AB", data.ToHexString());
            Assert.AreEqual("036cf9ab", data.ToHexString(true));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToHexString_Null()
        {
            var data = default(byte[]);
            data.ToHexString();
        }
    }
}
