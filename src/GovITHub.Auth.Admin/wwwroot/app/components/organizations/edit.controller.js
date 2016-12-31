(function () {
    'use strict';
    /*eslint angular/di: [2,"array"]*/
    angular
        .module('authAdminPanel')
        .controller('OrganizationsEditController', ["Organization", "$state", "$scope", 'status', 'id',

        function (Organization, $state, $scope, status, id) {
            var vm = this,
                vmLocal = {};

            vm.data = {
            };
            vm.status = status;
            vm.id = id;

            var create = function () {
                if ($scope.currentUser.organizationId >= 1)
                    vm.data.parentOrganizationId = $scope.currentUser.organizationId;
                Organization
                    .save(vm.data).$promise
                    .then(function (result) {
                        $scope.currentUser.organizationId = result.id;
                        $scope.currentUser.organizationName = result.name;
                        $state.go('index.organizations');
                    }).catch(function (err) {
                        vm.error = err;
                        console.error(err);
                    });
            };

            var update = function () {
                Organization
                    .update({ id: vm.id }, vm.data).$promise
                    .then(function (result) {
                        $state.go('index.organizations');
                    }).catch(function (err) {
                        vm.error = err;
                        console.error(err);
                    });
            }

            vm.save = function () {
                if (vm.status.edit && vm.id) {
                    update();
                } else {
                    create();
                }
            };

            vm.init = function () {
                Organization.get({ id: vm.id }).$promise
                    .then(function (result) {
                        vm.data = result;
                    }).catch(function (err) {
                        vm.error = err;
                        console.error(err);
                    });
            };

            if (vm.status.edit) {
                vm.init();
            };

            // -->End
            $scope.$on('$destroy', function () {
                vm.data = {};
                vmLocal = null;
            });
        }
        ]);
})();
