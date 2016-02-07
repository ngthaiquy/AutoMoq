﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMoq.Unity;
using Moq;

namespace AutoMoq
{
    // ReSharper disable once InconsistentNaming
    public interface Mocking
    {
        //IDictionary<Type, object> RegisteredMocks { get; }
        bool AMockHasNotBeenRegisteredFor(Type type);
        void RegisterThisMock(object mock, Type type);
        object GetTheMockFor(Type type);
        MockCreationResult CreateAMockObjectFor(Type type);
    }

    public class MockingWithMoq : Mocking
    {
        private MockRepository mockRepository;

        public MockingWithMoq(Config config)
        {
            RegisteredMocks = new Dictionary<Type, object>();
            mockRepository = new MockRepository(config.MockBehavior);
        }

        public IDictionary<Type, object> RegisteredMocks { get; }

        public bool AMockHasNotBeenRegisteredFor(Type type)
        {
            return RegisteredMocks.ContainsKey(type) == false;
        }

        public void RegisterThisMock(object mock, Type type)
        {
            RegisteredMocks.Add(type, mock);
        }

        public object GetTheMockFor(Type type)
        {
            return RegisteredMocks.First(x => x.Key == type).Value;
        }

        public MockCreationResult CreateAMockObjectFor(Type type)
        {
            var createMethod = GenerateAnInterfaceMockCreationMethod(type);

            return InvokeTheMockCreationMethod(createMethod);
        }

        private MethodInfo GenerateAnInterfaceMockCreationMethod(Type type)
        {
            var createMethodWithNoParameters = mockRepository.GetType().GetMethod("Create", EmptyArgumentList());
            return createMethodWithNoParameters.MakeGenericMethod(type);
        }

        private MockCreationResult InvokeTheMockCreationMethod(MethodInfo createMethod)
        {
            var mock = (Mock) createMethod.Invoke(mockRepository, new object[] {new List<object>().ToArray()});

            return new MockCreationResult()
            {
                ActualObject = mock.Object,
                MockObject = mock
            };
        }

        private static Type[] EmptyArgumentList()
        {
            return new[] {typeof (object[])};
        }
    }
}