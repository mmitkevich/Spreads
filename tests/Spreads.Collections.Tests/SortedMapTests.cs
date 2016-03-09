﻿using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Spreads.Collections.Tests {

    [TestFixture]
	public class SortedMapTests {

		[SetUp]
		public void Init() {
		}
		
		[Test]
		public void CouldEnumerateGrowingSM() {
            var count = 1000000;
            var sw = new Stopwatch();
            sw.Start();
            var sm = new SortedMap<DateTime, double>();
            var c = sm.GetCursor();

            for (int i = 0; i < count; i++) {
                sm.Add(DateTime.UtcNow.Date.AddSeconds(i), i);
                c.MoveNext();
                Assert.AreEqual(i, c.CurrentValue);
            }
            sw.Stop();
            Console.WriteLine("Elapsed msec: {0}", sw.ElapsedMilliseconds - 50);
            Console.WriteLine("Ops: {0}", Math.Round(0.000001 * count * 1000.0 / (sw.ElapsedMilliseconds * 1.0), 2));

        }


        [Test]
        public void CouldEnumerateChangingSM() {
            var count = 1000000;
            var sw = new Stopwatch();
            sw.Start();
            var sm = new SortedMap<DateTime, double>();
            var c = sm.GetCursor();

            for (int i = 0; i < count; i++) {
                sm.Add(DateTime.UtcNow.Date.AddSeconds(i), i);
                var version = sm.Version;
                if (i > 10)
                {
                    sm[DateTime.UtcNow.Date.AddSeconds(i - 10)] = i - 10 + 1;
                    Assert.IsTrue(sm.Version > version);
                }
                c.MoveNext();
                Assert.AreEqual(i, c.CurrentValue);
            }
            sw.Stop();
            Console.WriteLine("Elapsed msec: {0}", sw.ElapsedMilliseconds - 50);
            Console.WriteLine("Ops: {0}", Math.Round(0.000001 * count * 1000.0 / (sw.ElapsedMilliseconds * 1.0), 2));

        }

        [Test]
        public void CouldMoveAtGE() {
            var scm = new SortedMap<int, int>(50);
            for (int i = 0; i < 100; i++) {
                scm[i] = i;
            }

            var cursor = scm.GetCursor();

            cursor.MoveAt(-100, Lookup.GE);

            Assert.AreEqual(0, cursor.CurrentKey);
            Assert.AreEqual(0, cursor.CurrentValue);

            var shouldBeFalse = cursor.MoveAt(-100, Lookup.LE);
            Assert.IsFalse(shouldBeFalse);


        }


        [Test]
        public void CouldMoveAtLE() {
            var scm = new SortedMap<long, long>();
            for (long i = int.MaxValue; i < int.MaxValue*4L; i = i + int.MaxValue) {
                scm[i] = i;
            }

            var cursor = scm.GetCursor();

            var shouldBeFalse = cursor.MoveAt(0, Lookup.LE);
            Assert.IsFalse(shouldBeFalse);

        }

        [Test]
        public void CouldSerializeSMWithSingleElement() {
            var sm = new SortedMap<long, long>();
            sm.Add(1, 1);

            var sm2 = Serialization.Serializer.Deserialize<SortedMap<long, long>>(Serialization.Serializer.Serialize(sm));
            Assert.AreEqual(1, sm2.Count);
            Assert.AreEqual(1, sm2.First.Value);
            Assert.AreEqual(1, sm2.First.Key);

        }

    }
}
