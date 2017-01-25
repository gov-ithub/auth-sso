(function () {
    'use strict';

    angular
        .module('authAdminPanel')
        .controller('UsersEditController', UsersEditController);

    UsersEditController.$inject = ["User", "$state", "$scope", 'status', 'id'];

    function UsersEditController(User, $state, $scope, status, id) {
        var vm = this;
        vm.data = {};
        vm.status = status;
        vm.id = id;

        vm.save = function () {
            if (vm.status.edit && vm.id) {
                update();
            } else {
                create();
            }
        };

        vm.init = function () {
            User.get({ id: vm.id, organizationId: $scope.currentUser.organizationId }).$promise
                .then(function (result) {
                    vm.data = result;
                }).catch(function (err) {
                    vm.error = err;
                    console.error(err);
                });
        };

        var create = function () {
            User
                .save({ organizationId: $scope.currentUser.organizationId }, vm.data).$promise
                .then(function (result) {
                    $state.go('index.users');
                }).catch(function (err) {
                    vm.error = err;
                    console.error(err);
                });
        };

        var update = function () {
            User
                .update({ id: vm.id, organizationId: $scope.currentUser.organizationId }, vm.data).$promise
                .then(function (result) {
                    $state.go('index.users');
                }).catch(function (err) {
                    vm.error = err;
                    console.error(err);
                });
        }

        if (vm.status.edit) {
            vm.init();
        };

        $scope.$on('$destroy', function () {
            vm.data = {};
        });
    };
})();