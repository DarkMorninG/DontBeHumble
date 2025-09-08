using System.Collections.Generic;
using Dont_Be_Humble.Attributes;
using Dont_Be_Humble.Controllers;
using Dont_Be_Humble.TestEnvs;
using NSubstitute.Core;
using UnityEngine;

namespace Dont_Be_Humble.Injection {
    public class MockInjector {
        private static readonly HashSet<Injectable> _mocks = new HashSet<Injectable>();
        private static readonly List<Component> _components = new List<Component>();

        public static IEnumerable<object> Mocks => _mocks;


        public static void Purge() {
            _mocks.Clear();
            _components.Clear();
        }

        public static void InjectAll(DBHUnitTest dbhUnitTest) {
            InjectControllerMocks(dbhUnitTest);
            InjectMockFields(dbhUnitTest);
            InjectComponentMocks(dbhUnitTest);
        }

        private static void InjectControllerMocks(DBHUnitTest dbhUnitTest) {
            var injectAbleFields = Injector.GetFieldsWithAttribute<MockGrab>(dbhUnitTest);
            foreach (var injectAbleField in injectAbleFields) {
                var currentSubstituteFactory = SubstitutionContext.Current.SubstituteFactory;
                var substitute = currentSubstituteFactory.Create(new[] {injectAbleField.FieldType}, null);
                _mocks.Add(new Injectable(substitute));
            }

            Injector.InjectField<MockGrab>(dbhUnitTest, _mocks);
        }

        private static void InjectMockFields(DBHUnitTest dbhUnitTest) {
            var injectAbleFields = Injector.GetFieldsWithAttribute<Mock>(dbhUnitTest);
            var toBeMocked = new HashSet<Injectable>();
            foreach (var injectAbleField in injectAbleFields) {
                var currentSubstituteFactory = SubstitutionContext.Current.SubstituteFactory;
                var substitute = currentSubstituteFactory.Create(new[] {injectAbleField.FieldType}, null);
                toBeMocked.Add(new Injectable(substitute));
            }

            Injector.InjectField<Mock>(dbhUnitTest, toBeMocked);
        }

        private static void InjectComponentMocks(DBHUnitTest dbhUnitTest) {
            var injectAbleFields = Injector.GetFieldsWithAttribute<MockComponent>(dbhUnitTest);
            var toBeMocked = new HashSet<Injectable>();
            foreach (var injectAbleField in injectAbleFields) {
                var currentSubstituteFactory = SubstitutionContext.Current.SubstituteFactory;
                var substitute = currentSubstituteFactory.Create(new[] {injectAbleField.FieldType}, null);
                ComponentMockController.AddMock(injectAbleField.FieldType, substitute);
                toBeMocked.Add(new Injectable(substitute));
            }

            Injector.InjectField<MockComponent>(dbhUnitTest, toBeMocked);
        }
    }
}