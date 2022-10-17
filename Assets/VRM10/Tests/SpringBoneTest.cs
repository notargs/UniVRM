using System;
using NUnit.Framework;
using Unity.Collections;
using UniVRM10.FastSpringBones.Utilities;

namespace UniVRM10
{
    public class SpringBoneTest
    {
        [Test]
        public void NativeListTest()
        {
            var nativeList = new NativeList<int>(3, Allocator.Persistent);

            Assert.That(nativeList.Capacity, Is.EqualTo(4));
            Assert.That(nativeList.Length, Is.EqualTo(0));

            for (var i = 0; i < 7; ++i)
            {
                nativeList.Add(i);
            }

            Assert.That(nativeList.Capacity, Is.EqualTo(8));
            Assert.That(nativeList.Length, Is.EqualTo(7));

            nativeList.Remove(3);
            nativeList.Remove(3);

            Assert.That(nativeList.Capacity, Is.EqualTo(8));
            Assert.That(nativeList.Length, Is.EqualTo(5));

            nativeList[1] = 100;
            Assert.That(() => nativeList[-1] = 100, Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(() => nativeList[5] = 100, Throws.TypeOf<IndexOutOfRangeException>());

            Assert.That(nativeList, Is.EqualTo(new[] {0, 100, 2, 5, 6}));
            Assert.That(nativeList.GetNativeSlice(), Is.EqualTo(new[] {0, 100, 2, 5, 6}));

            nativeList.Dispose();
        }
    }
}