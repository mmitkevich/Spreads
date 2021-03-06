﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using System.Runtime.InteropServices;
using System.Threading;
using Spreads.Serialization;


namespace Spreads.Core.Tests {

    internal static class ArrayConvertorFactory {
        public static IBinaryConverter<TElement[]> GenericCreate<TElement>() {
            // check if element is fixed size
            // then, if pinnable - use pointer directly to pinned array
            // else, copy element-by-element to a buffer

            return new ArrayBinaryConverter<TElement>();
        }

        public static object Create(Type type) {
            MethodInfo method = typeof(ArrayConvertorFactory).GetMethod("GenericCreate");
            MethodInfo generic = method.MakeGenericMethod(type);
            return generic.Invoke(null, null);
        }
    }

    internal class ArrayBinaryConverter<TElement> : IBinaryConverter<TElement[]> {
        public bool IsFixedSize => false;
        public int Size => 0;
        public int Version => TypeHelper<TElement>.Version;

        private static int _itemSize = TypeHelper<TElement>.Size;

        public int SizeOf(TElement[] value) {
            if (_itemSize > 0) {
                return _itemSize * value.Length;
            }
            throw new NotImplementedException();
        }

        public void ToPtr(TElement[] value, IntPtr ptr) {
            throw new NotImplementedException();
        }

        public TElement[] FromPtr(IntPtr ptr) {
            throw new NotImplementedException();
        }
    }


    [TestFixture]
    public class TypeHelperTests {
        
        [Test]
        public void CouldGetSizeOfDoubleArray()
        {
            TypeHelper.ArrayConvertorFactory = ArrayConvertorFactory.Create;
            Console.WriteLine(TypeHelper<double[]>.Size);

        }

        [Test]
        public void CouldGetSizeOfReferenceType() {

            Console.WriteLine(TypeHelper<string>.Size);

        }
    }
}
